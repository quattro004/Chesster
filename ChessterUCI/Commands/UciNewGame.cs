using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessterUci.Commands
{
    /// <summary>
    /// This is sent to the engine when the next search (started with "position" and "go") will be from
    /// a different game.This can be a new game the engine should play or a new game it should analyse but
    /// also the next position from a testsuite with positions only. If the GUI hasn't sent a "ucinewgame"
    /// before the first "position" command, the engine shouldn't expect any further ucinewgame commands as
    /// the GUI is probably not supporting the ucinewgame command. So the engine should not rely on this 
    /// command even though all new GUIs should support it. As the engine's reaction to "ucinewgame" can 
    /// take some time the GUI should always send <see cref="IsReadyCommand"/> after "ucinewgame" to wait
    /// for the engine to finish its operation.
    /// </summary>
    public class UciNewGame : ChessCommand
    {
        /// <summary>
        /// Command used to communicate with the chess engine ("ucinewgame").
        /// </summary>
        public override string CommandText
        {
            get
            {
                return "ucinewgame";
            }
        }
    }
}
