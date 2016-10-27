using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessterUci
{
    /// <summary>
    /// Controls the chess engine process. Any UCI compliant chess engine can be used.
    /// </summary>
    public interface IEngineController : IDisposable
    {
        /// <summary>
        /// Event which is published when data is received from the chess engine's standard output stream.
        /// </summary>
        event EventHandler<DataReceivedEventArgs> DataReceived;
        /// <summary>
        /// Event which is published when data is received from the chess engine's standard error stream.
        /// </summary>
        event EventHandler<DataReceivedEventArgs> ErrorReceived;
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
