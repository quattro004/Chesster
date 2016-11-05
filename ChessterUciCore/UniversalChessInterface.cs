using ChessterUciCore.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ChessterUciCore
{
    /// <summary>
    /// Implementation of the Universal Chess Interface http://wbec-ridderkerk.nl/html/UCIProtocol.html
    /// </summary>
    public class UniversalChessInterface : IDisposable
    {
        private TraceSource _universalChessInterfaceTraceSource;
        private bool _disposedValue = false; // To detect redundant calls

        #region Properties

        /// <summary>
        /// Performs initialization for the chess interface to the engine.
        /// <param name="engineController"><see cref="IEngineController"/> which controls the chess
        /// engine process.</param>
        /// </summary>
        /// <exception cref="ChessterEngineException">Thrown if the <paramref name="engineController"/>
        /// is null.</exception>
        public UniversalChessInterface(IEngineController engineController)
        {
            _universalChessInterfaceTraceSource = new TraceSource("UniversalChessInterfaceTraceSource");

            if (null == engineController)
            {
                throw new ChessterEngineException(Messages.NullEngineController);
            }
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
                    _universalChessInterfaceTraceSource.TraceInformation("Killing the chess engine process");
                    ChessEngineController.KillEngine();
                    _universalChessInterfaceTraceSource.TraceEvent(TraceEventType.Critical, 1,
                        Messages.ChessEngineDidntInitialize);
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
