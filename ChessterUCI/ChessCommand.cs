using System.Diagnostics;

namespace ChessterUci
{
    /// <summary>
    /// Interface for sending commands to the chess engine.
    /// </summary>
    public abstract class ChessCommand
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
        public void SendCommand()
        {
            EnsureEngineIsRunning();
            ChessCommandTraceSource.TraceInformation($"Sending the {CommandText} command to the chess engine.");
            _engineController.SendCommand(CommandText);
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
    }
}
