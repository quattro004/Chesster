using ChessterUciCore.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestChessterUciCore
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
