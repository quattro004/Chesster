using System;
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
        private ChessCommandFactory _commandFactory;
        
        /// <summary>
        /// Logger for this class.
        /// </summary>
        private ILogger Logger { get; } = ChessterLogging.CreateLogger<UniversalChessInterface>();

        /// <summary>
        /// Performs initialization for the chess interface to the engine. Creates a default
        /// implementation of the engine controller <see cref="EngineController" />.
        /// <param name="chessEnginePath">Path to the chess engine exe.</param>
        /// </summary>
        public UniversalChessInterface(string chessEnginePath)
        {
            Logger.LogTrace("UniversalChessInterface()");
            if(string.IsNullOrWhiteSpace(chessEnginePath))
            {
                throw new ArgumentException(nameof(chessEnginePath));
            }
            ChessEngineController = CreateEngineController(chessEnginePath);
            Initialize();
        }

        /// <summary>
        /// Performs initialization for the chess interface to the engine given the specified
        /// <paramref name="engineController" />.
        /// </summary>
        public UniversalChessInterface(IEngineController engineController)
        {
            Logger.LogTrace("UniversalChessInterface() with an injected engineController");
            ChessEngineController = engineController;
            Initialize();
        }

        #region Properties

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
        public IDictionary<string, OptionData> ChessEngineOptions { get; private set; }

        /// <summary>
        /// Implementation of the <see cref="IEngineController"/> which manages the chess
        /// engine process.
        /// </summary>
        public IEngineController ChessEngineController { get; private set; }
        
        /// <summary>
        /// Root of the configuration, used to read in the chess engine path from config.json.
        /// </summary>
        public IConfigurationRoot Config { get; private set; }

        #endregion

        /// <summary>
        /// Performs common initialization.
        /// </summary>
        private void Initialize()
        {
            _commandFactory = new ChessCommandFactory(ChessEngineController);
            SetUciMode();
        }

        /// <summary>
        /// Sends an asynchronous command to the chess engine to enable UCI mode.
        /// When successful the <see cref="ChessEngineOptions"/> property will contain
        /// the options that the engine supports.
        /// </summary>
        /// <remarks><see cref="UciCommand"/> for more information.</remarks>
        private void SetUciMode()
        {
            using (var uciCommand = _commandFactory.CreateCommand<UciCommand>())
            {
                var uciCommandTask = uciCommand.SendAsync();
                uciCommandTask.Wait(uciCommand.CommandResponsePeriod);
                
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
        /// Creates a <see cref="ChessCommand"/> of the type specified.
        /// </summary>
        /// <typeparam name="T">Type of chess command to create.</typeparam>
        /// <returns>Instance of <see cref="ChessCommand"/> using the specified type.</returns>
        public T CreateCommand<T>() where T : ChessCommand, new()
        {
            return _commandFactory.CreateCommand<T>();
        }

        #region Private Methods

        private IEngineController CreateEngineController(string chessEnginePath)
        {
            Logger.LogTrace("CreateEngineController()");
            Logger.LogTrace($"chessEnginePath is {chessEnginePath}");
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
                    if(ChessEngineController != null)
                    {
                        ChessEngineController.Dispose();
                        ChessEngineController = null;                        
                    }
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
