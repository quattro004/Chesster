using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessterUci.Commands
{
    /// <summary>
    /// Switch the debug mode of the engine on and off.
	/// In debug mode the engine should send additional infos to the GUI, e.g.with the "info string" command,
    /// to help debugging, e.g.the commands that the engine has received etc. This mode should be switched 
    /// off by default and this command can be sent any time, also when the engine is thinking.
    /// </summary>
    public class DebugCommand : ChessCommand
    {
        /// <summary>
        /// Initializes the "debug" command for use with the engine controller.
        /// </summary>
        public DebugCommand() : base()
        {
        }

        /// <summary>
        /// Specifies whether debug mode is on.
        /// </summary>
        public bool DebugModeOn { get; set; }

        /// <summary>
        /// Command used to communicate with the chess engine ("debug").
        /// </summary>
        public override string CommandText
        {
            get
            {
                return DebugModeOn ? "debug" : "debug off";
            }
        }
    }
}
