using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using ChessterUciCore.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ChessterUciCore
{
    /// <summary>
    /// Implementation of the Universal Chess Interface http://wbec-ridderkerk.nl/html/UCIProtocol.html
    /// </summary>
    public class UniversalChessInterface : IDisposable
    {
        private bool _disposedValue = false; // To detect redundant calls
        /// <summary>
        /// Logger for this class.
        /// </summary>
        ILogger Logger { get; } = ChessterLogging.CreateLogger<UniversalChessInterface>();

        #region Properties

        /// <summary>
        /// Performs initialization for the chess interface to the engine. Creates a default
        /// implementation of the engine controller <see cref="EngineController" />.
        /// </summary>
        public UniversalChessInterface()
        {
            ChessEngineController = CreateEngineController();
            CommandFactory = new ChessCommandFactory(ChessEngineController);
        }

        /// <summary>
        /// Performs initialization for the chess interface to the engine given the specified
        /// <paramref name="engineController" />.
        /// </summary>
        public UniversalChessInterface(IEngineController engineController)
        {
            ChessEngineController = engineController;
            CommandFactory = new ChessCommandFactory(ChessEngineController);
        }

        /// <summary>
        /// Determines whether or not the chess engine process is currently running.
        /// </summary>
        public bool IsEngineProcessRunning
        {
            get
            {
                return ChessEngineController != null && ChessEngineController.IsEngineRunning;
            }
        }
        /// <summary>
        /// Specifies whether the chess engine has finished setting up UCI mode.
        /// </summary>
        public bool UciModeComplete { get; private set; }

        /// <summary>
        /// Options received after setting up UCI mode.
        /// </summary>
        public Dictionary<string, OptionData> ChessEngineOptions { get; private set; }

        /// <summary>
        /// Implementation of the <see cref="IEngineController"/> which manages the chess
        /// engine process.
        /// </summary>
        public IEngineController ChessEngineController { get; private set; }

        /// <summary>
        /// Factory for creating chess commands.
        /// </summary>
        public ChessCommandFactory CommandFactory { get; private set; }
        
        /// <summary>
        /// Root of the configuration, used to read in the chess engine path from config.json.
        /// </summary>
        public IConfigurationRoot Config { get; private set; }

        #endregion

        /// <summary>
        /// Sends an asynchronous command to the chess engine to enable UCI mode.
        /// When successful the <see cref="ChessEngineOptions"/> property will contain
        /// the options that the engine supports.
        /// </summary>
        /// <remarks><see cref="UciCommand"/> for more information.</remarks>
        public void SetUciMode()
        {
            using (var uciCommand = CommandFactory.CreateCommand<UciCommand>())
            {
                SendCommand(uciCommand);
                WaitForResponse(uciCommand);
                if (uciCommand.CommandResponseReceived)
                {
                    ChessEngineOptions = uciCommand.Options;
                    UciModeComplete = true;
                }
                else
                {
                    // Initialization wasn't completed by the chess engine within the specified
                    // time period so kill the process.
                    Logger.LogInformation("Killing the chess engine process");
                    ChessEngineController.KillEngine();
                    Logger.LogCritical(Messages.ChessEngineDidntInitialize);
                    throw new ChessterEngineException(Messages.ChessEngineDidntInitialize);
                }
            }
        }

        /// <summary>
        /// Waits for the command's response to be returned or the timeout period to expire.
        /// </summary>
        /// <param name="command"></param>
        public void WaitForResponse(ChessCommand command)
        {
            if(null == command)
            {
                throw new ChessterEngineException(Messages.NullCommand);
            }
            while (!command.CommandResponseReceived && !command.CommandTimeoutElapsed)
            {
                Thread.Sleep(command.TimerInterval);
            }
        }

        /// <summary>
        /// Sends the <paramref name="command"/> to the chess engine.
        /// </summary>
        /// <param name="command"><see cref="ChessCommand"/></param>
        /// <returns><see cref="Task"/></returns>
        public void SendCommand(ChessCommand command)
        {
            if(null == command)
            {
                throw new ChessterEngineException(Messages.NullCommand);
            }
            command.SendCommand();
        }

        #region Private Methods

        private IEngineController CreateEngineController()
        {
            Logger.LogInformation($"CreateEngineController()");
            var currentDirectory = Directory.GetCurrentDirectory();
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.SetBasePath(currentDirectory);
            var configFile = Path.Combine(currentDirectory, "ChessterUciCore", "config.json");
            configurationBuilder.AddJsonFile(configFile);
            Config = configurationBuilder.Build();
            var opSys = Environment.GetEnvironmentVariable("OS");
            string chessEnginePath = null;
            if (!string.IsNullOrWhiteSpace(opSys) && opSys.ToLower().Contains("windows"))
            {
                chessEnginePath = Path.Combine(currentDirectory, Config["ChessEnginePathWindows"]);
            }
            else
            {
                chessEnginePath = Path.Combine(currentDirectory, Config["ChessEnginePathLinux"]);
            }
            Logger.LogInformation($"chessEnginePath is {chessEnginePath}");
            return new EngineController(chessEnginePath);
        }
        
        #endregion

        #region IDisposable Support
        
        /// <summary>
        /// Disposes of resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    ChessEngineController.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UniversalChessInterface() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        /// <summary>
        /// This code added to correctly implement the disposable pattern.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
