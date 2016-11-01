using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessterUciCore.Commands
{
    /// <summary>
    /// Quit the program as soon as possible.
    /// </summary>
    public class QuitCommand : ChessCommand
    {
        /// <summary>
        /// Command to send to the engine "quit".
        /// </summary>
        public override string CommandText
        {
            get
            {
                return "quit";
            }
        }
    }
}
