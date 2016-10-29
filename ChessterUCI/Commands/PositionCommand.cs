using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessterUci.Commands
{
    /// <summary>
    /// Set up the position described in fenstring on the internal board and play the moves on 
    /// the internal chess board. If the game was played from the start position the string 
    /// "startpos" will be sent  Note: no "new" command is needed. However, if this position is 
    /// from a different game than the last position sent to the engine, the GUI should have 
    /// sent a <see cref="UciNewGame"/> in between.
    /// </summary>
    public class PositionCommand : ChessCommand
    {
        private string _commandText;

        /// <summary>
        /// Describes the moves to the chess engine using a FEN (Forsyth–Edwards Notation) string.
        /// </summary>
        public string FenString { get; set; }

        /// <summary>
        /// Determines if this is the start position.
        /// </summary>
        public bool IsStartPosition { get; set; }

        /// <summary>
        /// Command used to communicate with the chess engine ("position").
        /// </summary>
        public override string CommandText
        {
            get
            {
                return _commandText;
            }
        }

        internal override void SendCommand()
        {
            _commandText = string.Format("position{0} moves {1}", IsStartPosition 
                ? " startpos" : string.Empty, FenString);
            base.SendCommand();
        }

    }
}
