using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessterUCI
{
    /// <summary>
    /// Implementation of the Universal Chess Interface http://wbec-ridderkerk.nl/html/UCIProtocol.html
    /// </summary>
    public class UniversalChessInterface
    {
        /// <summary>
        /// <see cref="EngineController"/> which controls and communicates with the chess engine process.
        /// </summary>
        public EngineController ChessEngineController { get; set; }

        /// <summary>
        /// Performs initialization for the chess interface.
        /// </summary>
        public UniversalChessInterface()
        {
            ChessEngineController = new EngineController();
        }
   
        /// <summary>
        /// Starts the chess engine process.
        /// </summary>
        /// <returns><see cref="bool"/> specifying whether the chess engine process started or not.</returns>
        public bool InitializeEngine()
        {
            ChessEngineController.StartChessEngine();
            return true;
        }
    }
}
