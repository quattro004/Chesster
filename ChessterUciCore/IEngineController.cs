using ChessterUciCore.Commands;
using System;

namespace ChessterUciCore
{
    /// <summary>
    /// Controls the chess engine process. Any UCI compliant chess engine can be used.
    /// </summary>
    public interface IEngineController : IDisposable
    {
        /// <summary>
        /// Event which is published when data is received from the chess engine's standard output stream.
        /// </summary>
        event EventHandler<ChessCommandReceivedEventArgs> DataReceived;
        /// <summary>
        /// Event which is published when data is received from the chess engine's standard error stream.
        /// </summary>
        event EventHandler<ChessCommandReceivedEventArgs> ErrorReceived;
        /// <summary>
        /// Determines whether the chess engine process is running.
        /// </summary>
        bool IsEngineRunning { get; }
        /// <summary>
        /// Sends the specified command to the chess engine.
        /// </summary>
        void SendCommand(string command);
        /// <summary>
        /// Kills the process that is hosting the chess engine.
        /// </summary>
        void KillEngine();
    }
}
