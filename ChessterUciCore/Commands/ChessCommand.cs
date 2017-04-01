using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChessterUciCore.Commands
{
    /// <summary>
    /// Interface for sending commands to the chess engine.
    /// </summary>
    public abstract class ChessCommand : IDisposable
    {
        private IEngineController _engineController;
        private Timer _commandTimer;
        private TimeSpan _commandResponsePeriod;
        private TimeSpan _timerInterval = new TimeSpan(0, 0, 0, 0, 50); // 50 milliseconds
        private bool disposedValue = false; // To detect redundant calls
        private bool _commandResponseReceived;
        private DateTime _commandStartTime;

        /// <summary>
        /// Initializes the command for use with the engine controller.
        /// </summary>
        protected ChessCommand()
        {
            _commandResponsePeriod = new TimeSpan(0, 0, 10); // 10 second default.
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
        /// Logger for this class.
        /// </summary>
        ILogger Logger { get; } = ChessterLogging.CreateLogger<ChessCommand>();

        /// <summary>
        /// Time period to wait until a response to this command is received from the chess engine.
        /// Default is 10 seconds.
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
            get
            {
                var elapsedCommandSendTime = DateTime.Now.Subtract(_commandStartTime);
                return elapsedCommandSendTime >= CommandResponsePeriod;
            }
        }

        /// <summary>
        /// Amount of time to wait between timer callbacks after sending a command
        /// to check for a response from the chess engine. The default is 50 milliseconds.
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
                    Logger.LogInformation($"Changing the timer interval to {_timerInterval.Milliseconds} milliseconds.");
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
            Logger.LogTrace($"Data received: {chessEngineResponse}.");

            return !string.IsNullOrWhiteSpace(chessEngineResponse);
        }

        /// <summary>
        /// Timer callback that is invoked when sending commands to the chess engine. Used to determine
        /// if a response was received within the specified time period.
        /// </summary>
        /// <param name="state"></param>
        protected virtual void CommandTimerCallback(object state)
        {
            Logger.LogTrace("CommandTimerCallback()");

            if (CommandTimeoutElapsed || CommandResponseReceived)
            {
                StopTimer();
            }
        }

        /// <summary>
        /// Stops the send command timer.
        /// </summary>
        private void StopTimer()
        {
            if (_commandTimer != null)
            {
                Logger.LogTrace("Stopping the command timer.");
                if(!_commandTimer.Change(Timeout.Infinite, Timeout.Infinite)) // Stop the timer.
                {
                    Logger.LogWarning("Unable to stop the timer using the change method.");
                }
            }
        }

        /// <summary>
        /// Sends this command to the chess engine and waits for a response.
        /// </summary>
        public virtual async Task SendAsync()
        {
            CommandResponseReceived = false;
            EnsureEngineIsRunning();
            ErrorText = default(string);
            Logger.LogTrace($"Sending the {CommandText} command to the chess engine.");
            ChessEngineController.SendCommand(CommandText);
            _commandStartTime = DateTime.Now;
            _commandTimer = new Timer(CommandTimerCallback, null, TimerInterval, TimerInterval);
            await WaitForResponseAsync();
        }

        /// <summary>
        /// Waits for the command's response to be returned or the timeout period to expire.
        /// </summary>
        private async Task WaitForResponseAsync()
        {
            while (!CommandResponseReceived && !CommandTimeoutElapsed)
            {
                await Task.Delay(TimerInterval);
            }
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
        private void EngineController_ErrorReceived(object sender, ChessCommandReceivedEventArgs e)
        {
            Logger.LogError($"ErrorReceived: {e.Data}.");

            if (ResponseIsNotNullOrEmpty(e.Data))
            {
                CommandResponseReceived = true;
                ErrorText = e.Data;
            }
        }

        /// <summary>
        /// Gets the <see cref="RegistrationStatus"/> given data from the chess engine.
        /// </summary>
        /// <param name="data">Registration message from the chess engine.</param>
        /// <returns><see cref="RegistrationStatus"/> indicating the current registration status.</returns>
        internal RegistrationStatus GetRegistrationStatus(string data)
        {
            RegistrationStatus status = RegistrationStatus.NotInitiated;

            if (data == "registration checking")
            {
                status = RegistrationStatus.Checking;
            }
            else if (data == "registration ok")
            {
                status = RegistrationStatus.Ok;
            }
            else if (data == "registration error")
            {
                status = RegistrationStatus.Error;
            }

            return status;
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
                    if (_commandTimer != null)
                    {
                        _commandTimer.Dispose();
                    }
                    if(ChessEngineController != null)
                    {
                        ChessEngineController.ErrorReceived -= EngineController_ErrorReceived;
                        ChessEngineController = null;
                    }
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
