using System;

namespace ChessterUciCore.Commands
{
    /// <summary>
    /// Used to send chess command data.
    /// </summary>
    public class ChessCommandReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Chess command response data.
        /// </summary>
        public string Data { get; private set; }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        /// <param name="data">Chess engine command data.</param>
        public ChessCommandReceivedEventArgs(string data)
        {
            Data = data;
        }
    }
}
