using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ChessterUci
{
    /// <summary>
    /// Interface for sending commands to the chess engine.
    /// </summary>
    public abstract class ChessCommand : IDisposable
    {
        private IEngineController _engineController;

        /// <summary>
        /// Initializes the command for use with the engine controller.
        /// </summary>
        /// <param name="engineController">Engine controller which manages the chess engine
        /// process.</param>
        /// <exception cref="ChessterEngineException">will be thrown if the engine controller is null.</exception>
        protected ChessCommand(IEngineController engineController)
        {
            if (engineController == null)
            {
                throw new ChessterEngineException(Messages.NullEngineController);
            }
            _engineController = engineController;
            _engineController.ErrorReceived += EngineController_ErrorReceived;
        }

        /// <summary>
        /// All communication to the chess engine is done via standard input and output
        /// with text commands.
        /// </summary>
        public virtual string CommandText { get; }

        /// <summary>
        /// Used to trace information at runtime regarding chess command execution.
        /// </summary>
        public TraceSource ChessCommandTraceSource { get; } = new TraceSource("ChessCommandTraceSource");

        /// <summary>
        /// Sends this command to the chess engine.
        /// </summary>
        public async Task SendCommand()
        {
            EnsureEngineIsRunning();
            ChessCommandTraceSource.TraceInformation($"Sending the {CommandText} command to the chess engine.");
            await _engineController.SendCommand(CommandText);
        }

        /// <summary>
        /// Throws a <see cref="ChessterEngineException"/> exception if the engine is not currently
        /// running.
        /// </summary>
        private void EnsureEngineIsRunning()
        {
            if (!_engineController.IsEngineRunning)
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
            ChessCommandTraceSource.TraceEvent(TraceEventType.Error, 0, $"EngineController_ErrorReceived data is {e.Data}.");
            throw new ChessterEngineException(e.Data);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

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
                    _engineController.ErrorReceived -= EngineController_ErrorReceived;
                    _engineController = null;
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
