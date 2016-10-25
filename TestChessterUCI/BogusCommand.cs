using ChessterUci.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessterUci;

namespace TestChessterUCI
{
    /// <summary>
    /// Used to test sending an invalid command to the chess engine.
    /// </summary>
    public class BogusCommand : ChessCommand
    {
        public BogusCommand() : base()
        {
        }

        public override string CommandText
        {
            get
            {
                return "Bogus";
            }
        }
    }
}
