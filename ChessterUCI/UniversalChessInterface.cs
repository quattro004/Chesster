using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ChessterUci
{
    /// <summary>
    /// Implementation of the Universal Chess Interface http://wbec-ridderkerk.nl/html/UCIProtocol.html
    /// </summary>
    public class UniversalChessInterface : IDisposable
    {
        private TraceSource _universalChessInterfaceTraceSource;
        private Timer _initializationTimer;
        private int _initializationPeriod = 10000; // 10 seconds
        private bool _initializationComplete;

        #region Properties

        /// <summary>
        /// Performs initialization for the chess interface to the engine.
        /// </summary>
        public UniversalChessInterface(string chessEnginePath)
        {
            _universalChessInterfaceTraceSource = new TraceSource("UniversalChessInterfaceTraceSource");
            ChessEngineController = new EngineController(chessEnginePath);
        }

        /// <summary>
        /// Determines whether or not the chess engine is currently running.
        /// </summary>
        public bool IsEngineRunning
        {
            get
            {
                return ChessEngineController != null && ChessEngineController.IsEngineRunning;
            }
        }
        /// <summary>
        /// Specifies whether the chess engine has finished initializing.
        /// </summary>
        public bool InitializationComplete
        {
            get
            {
                return _initializationComplete;
            }
            private set
            {
                _initializationComplete = value;
                if (_initializationComplete)
                {
                    if (_initializationTimer != null)
                    {
                        _initializationTimer.Change(Timeout.Infinite, _initializationPeriod); // Stop the timer.
                    }
                }
            }
        }

        /// <summary>
        /// Amount of time in seconds to wait if the chess engine doesn't respond with uciok before 
        /// terminating the process and throwing an exception. The default is 10 seconds.
        /// </summary>
        public int InitializationPeriod
        {
            get
            {
                return _initializationPeriod / 1000;
            }
            set
            {
                _initializationPeriod = value * 1000;
                if(_initializationPeriod <= 0)
                {
                    throw new ChessterEngineException(Messages.InvalidInitializationPeriod);
                }
            }
        }

        /// <summary>
        /// Options received after initialization.
        /// </summary>
        public Dictionary<string, OptionData> ChessEngineOptions { get; private set; }

        /// <summary>
        /// Implementation of the <see cref="IEngineController"/> which manages the chess
        /// engine process.
        /// </summary>
        public IEngineController ChessEngineController { get; private set; }

        #endregion

        /// <summary>
        /// Performs initialization of the chess engine by sending the uci command.
        /// </summary>
        public async Task InitializeEngine()
        {
            await SetUciMode();
        }

        #region Private Methods

        /// <summary>
        /// Sends an asynchronous command to the chess engine to enable UCI mode.
        /// </summary>
        private async Task SetUciMode()
        {
            var uciCommand = new UciCommand(ChessEngineController);
            _universalChessInterfaceTraceSource.TraceInformation("Sending the UCI command");
            await uciCommand.SendCommand();

            _universalChessInterfaceTraceSource.TraceInformation("Starting InitializationPeriod timer");
            _initializationTimer = new Timer(new TimerCallback(InitializationTimerCallback), uciCommand, 
                _initializationPeriod, _initializationPeriod);
        }

        /// <summary>
        /// Fires then the InitializationPeriod has expired.
        /// </summary>
        /// <param name="state"></param>
        private void InitializationTimerCallback(object state)
        {
            _universalChessInterfaceTraceSource.TraceInformation("InitializationTimerCallback()");
            _initializationTimer.Change(Timeout.Infinite, _initializationPeriod); // Stop the timer.

            var uciCommand = state as UciCommand;
            if (uciCommand != null)
            {
                InitializationComplete = uciCommand.ReceivedUciOk;
                _universalChessInterfaceTraceSource.TraceInformation("InitializationComplete = {0}", InitializationComplete);

                if (!InitializationComplete)
                {
                    // Initialization wasn't completed by the chess engine within the specified
                    // time period so kill the process.
                     _universalChessInterfaceTraceSource.TraceInformation("Killing the chess engine");
                    ChessEngineController.KillEngine();
                    throw new ChessterEngineException(Messages.ChessEngineDidntInitialize);
                }
                else
                {
                    ChessEngineOptions = uciCommand.Options;
                }
            }
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Disposes of resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    ChessEngineController.Dispose();
                    if (_initializationTimer != null)
                    {
                        _initializationTimer.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
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
