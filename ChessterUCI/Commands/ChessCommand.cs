using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ChessterUci.Commands
{
    /// <summary>
    /// Interface for sending commands to the chess engine.
    /// </summary>
    public abstract class ChessCommand : IDisposable
    {
        private IEngineController _engineController;
        private Timer _commandTimer;
        private TimeSpan _commandResponsePeriod;
        private TimeSpan _elapsedCommandSendTime; // How long it's been since the command was sent.
        private TimeSpan _timerInterval = new TimeSpan(0, 0, 0, 0, 10); // 10 milliseconds
        private bool disposedValue = false; // To detect redundant calls
        private bool _commandResponseReceived;

        /// <summary>
        /// Initializes the command for use with the engine controller.
        /// </summary>
        protected ChessCommand()
        {
            _commandTimer = new Timer(CommandTimerCallback, null, Timeout.Infinite, Timeout.Infinite); // Don't start timer yet
            _commandResponsePeriod = new TimeSpan(0, 0, 1); // 1 second default.
        }

        /// <summary>
        /// All communication to the chess engine is done via standard input and output
        /// with text commands.
        /// </summary>
        public virtual string CommandText { get; }

        /// <summary>
        /// Text returned from the chess engine when an error occurs with this command.
        /// </summary>
        public string ErrorText { get; private set; }

        /// <summary>
        /// Used to trace information at runtime regarding chess command execution.
        /// </summary>
        protected TraceSource ChessCommandTraceSource { get; } = new TraceSource("ChessCommandTraceSource");

        /// <summary>
        /// Time period to wait until a response to this command is received from the chess engine.
        /// Default is 1 second.
        /// </summary>
        public TimeSpan CommandResponsePeriod
        {
            get
            {
                return _commandResponsePeriod;
            }

            set
            {
                _commandResponsePeriod = value;
                if (_commandResponsePeriod.TotalMilliseconds <= 0)
                {
                    throw new ChessterEngineException(Messages.InvalidTimePeriod);
                }
            }
        }

        /// <summary>
        /// Determines if a response was received from the chess engine.
        /// </summary>
        public bool CommandResponseReceived
        {
            get
            {
                return _commandResponseReceived;
            }
            protected set
            {
                _commandResponseReceived = value;
                if (_commandResponseReceived)
                {
                    StopTimer();
                }
            }
        }

        /// <summary>
        /// Determines if the command response period has elapsed.
        /// </summary>
        public bool CommandTimeoutElapsed
        {
            get { return _elapsedCommandSendTime >= CommandResponsePeriod; }
        }

        /// <summary>
        /// Amount of time to wait between timer callbacks after sending a command
        /// to check for a response from the chess engine. The default is 10 milliseconds.
        /// </summary>
        public TimeSpan TimerInterval
        {
            get
            {
                return _timerInterval;
            }
            set
            {
                _timerInterval = value;
                if(_commandTimer != null)
                {
                    _commandTimer.Change(_timerInterval, _timerInterval);
                }
            }
        }

        /// <summary>
        /// Reference to the chess engine controller which manages the actual process.
        /// </summary>
        internal virtual IEngineController ChessEngineController
        {
            get
            {
                return _engineController;
            }
            set
            {
                _engineController = value;
                if (_engineController != null)
                {
                    _engineController.ErrorReceived += EngineController_ErrorReceived;
                }
            }
        }

        /// <summary>
        /// Checks to see if the chess engine response is null or blank.
        /// </summary>
        /// <param name="chessEngineResponse">Response to a command from the chess engine.</param>
        /// <returns><see cref="bool"/> indicating a valid response.</returns>
        protected bool ResponseIsNotNullOrEmpty(string chessEngineResponse)
        {
            ChessCommandTraceSource.TraceInformation($"Data received: {chessEngineResponse}.");

            return !string.IsNullOrWhiteSpace(chessEngineResponse);
        }

        /// <summary>
        /// Timer callback that is invoked when sending commands to the chess engine. Used to determine
        /// if a response was received within the specified time period.
        /// </summary>
        /// <param name="state"></param>
        protected virtual void CommandTimerCallback(object state)
        {
            if (CommandTimeoutElapsed || CommandResponseReceived)
            {
                StopTimer();
                // Clear the elapsed time for next time a command is sent.
                _elapsedCommandSendTime = new TimeSpan(0, 0, 0, 0, 0);
            }
            else
            {
                _elapsedCommandSendTime.Add(TimerInterval);
            }
        }

        /// <summary>
        /// Stops the send command timer.
        /// </summary>
        private void StopTimer()
        {
            if (_commandTimer != null)
            {
                _commandTimer.Change(Timeout.Infinite, Timeout.Infinite); // Stop the timer.
            }
        }

        /// <summary>
        /// Sends this command to the chess engine.
        /// </summary>
        internal void SendCommand()
        {
            EnsureEngineIsRunning();
            ErrorText = default(string);
            ChessCommandTraceSource.TraceInformation($"Sending the {CommandText} command to the chess engine.");
            ChessEngineController.SendCommand(CommandText);
            _commandTimer.Change(TimerInterval, TimerInterval); // Start the timer.
        }

        /// <summary>
        /// Throws a <see cref="ChessterEngineException"/> exception if the engine is not currently
        /// running.
        /// </summary>
        private void EnsureEngineIsRunning()
        {
            if (ChessEngineController != null && !ChessEngineController.IsEngineRunning)
            {
                throw new ChessterEngineException(Messages.ChessEngineNotRunning);
            }
        }

        /// <summary>
        /// Occurs when an error is received from the engine controller after sending this command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EngineController_ErrorReceived(object sender, DataReceivedEventArgs e)
        {
            ChessCommandTraceSource.TraceEvent(TraceEventType.Error, 0, $"ErrorReceived: {e.Data}.");

            if (ResponseIsNotNullOrEmpty(e.Data))
            {
                CommandResponseReceived = true;
                ErrorText = e.Data;
            }
        }

        #region IDisposable Support

        /// <summary>
        /// Disposes of this object and cleans up any dependent resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    StopTimer(); // In case it's running.
                    _commandTimer.Dispose();
                    ChessEngineController.ErrorReceived -= EngineController_ErrorReceived;
                    ChessEngineController = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ChessCommand() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        /// <summary>
        ///  This code added to correctly implement the disposable pattern.
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
