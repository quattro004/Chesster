using ChessterUciCore;
using System;
using ChessterUciCore.Commands;
using System.Threading;

namespace TestChessterUciCore.Fakes
{
    public class FakeEngineController : IEngineController
    {
        public bool IsEngineRunning
        {
            get
            {
                return true;
            }
        }

        public event EventHandler<ChessCommandReceivedEventArgs> DataReceived;
        public event EventHandler<ChessCommandReceivedEventArgs> ErrorReceived;

        public void Dispose()
        {
        }

        public void KillEngine()
        {
        }

        public void SendCommand(string command)
        {
            if (command.StartsWith("register name"))
            {
                OnRaiseDataReceived(this, new ChessCommandReceivedEventArgs("registration checking"));
                // Simulate the engine thinking
                Thread.Sleep(100);
                OnRaiseDataReceived(this, new ChessCommandReceivedEventArgs("registration ok"));
            }
            else if (command == "register later")
            {
                OnRaiseDataReceived(this, new ChessCommandReceivedEventArgs("registration ok"));
            }
            else if (command == "uci")
            {
                OnRaiseDataReceived(this, new ChessCommandReceivedEventArgs("uciok"));
                if (IsCopyProtected)
                {
                    OnRaiseDataReceived(this, new ChessCommandReceivedEventArgs("copyprotection checking"));
                }
            }
        }

        /// <summary>
        /// Fires when data is received from the chess engine.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments received from the chess engine.</param>
        protected virtual void OnRaiseDataReceived(object sender, ChessCommandReceivedEventArgs e)
        {
            DataReceived?.Invoke(sender, e);
        }

        /// <summary>
        /// Fires when data is received from the chess engine.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments received from the chess engine.</param>
        protected virtual void OnRaiseErrorReceived(object sender, ChessCommandReceivedEventArgs e)
        {
            ErrorReceived?.Invoke(sender, e);
        }

        /// <summary>
        /// Used to simulate a copy protected engine.
        /// </summary>
        public bool IsCopyProtected { get; set; }
    }
}
