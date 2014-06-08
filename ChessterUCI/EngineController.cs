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
    public class EngineController
    {
        /// <summary>
        /// Starts the chess engine process and redirects standard error, input and output since chess engines
        /// use standard input and output to communicate via text commands.
        /// </summary>
        /// <returns>Running <see cref="Process"/> of the chess engine specified in the ChessEnginePath application setting.</returns>
        public Process StartChessEngine()
        {
            var chessEngine = new Process();
            chessEngine.StartInfo.FileName = ConfigurationManager.AppSettings["ChessEnginePath"];
            chessEngine.StartInfo.UseShellExecute = false;
            chessEngine.StartInfo.RedirectStandardError = true;
            chessEngine.StartInfo.RedirectStandardInput = true;
            chessEngine.StartInfo.RedirectStandardOutput = true;
            chessEngine.Start();

            return chessEngine;
        }
    }
}
