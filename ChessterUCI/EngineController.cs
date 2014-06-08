using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ChessterUCI
{
    /// <summary>
    /// Controls the chess engine process. Any UCI compliant chess engine can be used.
    /// </summary>
    public class EngineController : IDisposable
    {
        private bool _disposed;

        public Process ChessEngineProcess { get; set; }

        /// <summary>
        /// Starts the chess engine process and redirects standard error, input and output since UCI chess engines
        /// use standard input and output to communicate via text commands.
        /// </summary>
        /// <returns>Running <see cref="Process"/> of the chess engine specified in the ChessEnginePath application setting.</returns>
        public void StartChessEngine()
        {
            ChessEngineProcess = new Process();
            ChessEngineProcess.StartInfo.FileName = ConfigurationManager.AppSettings["ChessEnginePath"];
            ChessEngineProcess.StartInfo.UseShellExecute = false;
            ChessEngineProcess.StartInfo.RedirectStandardError = true;
            ChessEngineProcess.StartInfo.RedirectStandardInput = true;
            ChessEngineProcess.StartInfo.RedirectStandardOutput = true;
            ChessEngineProcess.Start();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Free any managed objects here. 
                //
                ChessEngineProcess.Dispose();
            }

            // Free any unmanaged objects here. 
            //
            _disposed = true;
        }

    }
}
