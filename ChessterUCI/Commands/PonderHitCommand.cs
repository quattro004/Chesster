using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessterUci.Commands
{
    /// <summary>
    /// The user has played the expected move. This will be sent if the engine was told to ponder 
    /// on the same move the user has played. The engine should continue searching but switch from
    /// pondering to normal search.
    /// </summary>
    public class PonderHitCommand : ChessCommand
    {
        /// <summary>
        /// Command to send to the engine "ponderhit".
        /// </summary>
        public override string CommandText
        {
            get
            {
                return "ponderhit";
            }
        }
    }
}
