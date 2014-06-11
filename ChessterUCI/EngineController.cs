using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;

namespace ChessterUCI
{
    /// <summary>
    /// Controls the chess engine process. Any UCI compliant chess engine can be used.
    /// </summary>
    public class EngineController : IDisposable
    {
        private bool _disposed;
        private Process _chessEngineProcess;

        /// <summary>
        /// Event which is published when data is received from the chess engine's standard output stream.
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> DataReceived;
        /// <summary>
        /// Event which is published when data is received from the chess engine's standard error stream.
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> ErrorReceived;

        /// <summary>
        /// Determines whether the chess engine process is running.
        /// </summary>
        public bool IsEngineRunning
        {
            get
            {
                return _chessEngineProcess != null && !_chessEngineProcess.HasExited;
            }
        }

        /// <summary>
        /// Starts the chess engine process and redirects standard error, input and output since UCI chess engines
        /// use standard input and output to communicate via text commands.
        /// </summary>
        /// <returns>Running <see cref="Process"/> of the chess engine specified in the ChessEnginePath application setting.</returns>
        public void StartChessEngine()
        {
            _chessEngineProcess = new Process();
            _chessEngineProcess.StartInfo.FileName = ConfigurationManager.AppSettings["ChessEnginePath"];
            _chessEngineProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(_chessEngineProcess.StartInfo.FileName);
            _chessEngineProcess.StartInfo.UseShellExecute = false;
            _chessEngineProcess.StartInfo.CreateNoWindow = true;
            _chessEngineProcess.StartInfo.RedirectStandardError = true;
            _chessEngineProcess.StartInfo.RedirectStandardInput = true;
            _chessEngineProcess.StartInfo.RedirectStandardOutput = true;
            _chessEngineProcess.OutputDataReceived += _chessEngineProcess_OutputDataReceived;
            _chessEngineProcess.ErrorDataReceived += _chessEngineProcess_ErrorDataReceived;
            _chessEngineProcess.Start();
            _chessEngineProcess.BeginErrorReadLine();
            _chessEngineProcess.BeginOutputReadLine();
        }

        private void _chessEngineProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnRaiseErrorReceived(sender, e);
        }

        private void _chessEngineProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnRaiseDataReceived(sender, e);
        }

        /// <summary>
        /// Frees up managed and unmanaged resources and suppresses the finalizer.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Provides the implementation for disposing resources.
        /// </summary>
        /// <param name="disposing"><see cref="bool"/> specifying whether to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Free any managed objects here. 
                //
                _chessEngineProcess.Close();
                _chessEngineProcess.Dispose();
            }

            // Free any unmanaged objects here. 
            //
            _disposed = true;
        }

        /// <summary>
        /// Fires when data is received from the chess engine.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments received from the chess engine.</param>
        protected virtual void OnRaiseDataReceived(object sender, DataReceivedEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of 
            // a race condition if the last subscriber unsubscribes 
            // immediately after the null check and before the event is raised.
            var handler = DataReceived;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        /// <summary>
        /// Fires when an error is received from the chess engine.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments received from the chess engine.</param>
        protected virtual void OnRaiseErrorReceived(object sender, DataReceivedEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of 
            // a race condition if the last subscriber unsubscribes 
            // immediately after the null check and before the event is raised.
            var handler = ErrorReceived;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        /// <summary>
        /// Sends the specified <paramref name="command"/> command to the chess engine's standard
        /// input stream asynchronously. The <see cref="DataReceived"/> or <see cref="ErrorReceived"/>
        /// events will fire when the engine responds.
        /// </summary>
        /// <param name="command"></param>
        public void SendCommand(string command)
        {
            _chessEngineProcess.StandardInput.WriteLineAsync(command);
        }
    }
}
