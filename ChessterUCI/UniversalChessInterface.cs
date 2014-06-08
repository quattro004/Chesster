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
        public EngineController ChessEngineController { get; set; }

        public UniversalChessInterface()
        {
            ChessEngineController = new EngineController();
        }
   
        public bool InitializeEngine()
        {
            ChessEngineController.StartChessEngine();
            return true;
        }
    }
}
