using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessterUciCore.Commands
{
    /// <summary>
    /// This is used to synchronize the engine with the GUI. When the GUI has sent a command or
	/// multiple commands that can take some time to complete, this command can be used to wait 
    /// for the engine to be ready again or to ping the engine to find out if it is still alive.
    /// E.g. this should be sent after setting the path to the tablebases as this can take some time.
    /// This command is also required once before the engine is asked to do any search to wait for 
    /// the engine to finish initializing. This command must always be answered with "readyok" and 
    /// can be sent also when the engine is calculating in which case the engine should also immediately 
    /// answer with "readyok" without stopping the search.
    /// </summary>
    public class IsReadyCommand : ChessCommand
    {
        private const string READYOK = "readyok";
        /// <summary>
        /// Initializes the "isready" command for use with the engine controller.
        /// </summary>
        public IsReadyCommand() : base()
        {
        }

        /// <summary>
        /// Command used to communicate with the chess engine ("isready").
        /// </summary>
        public override string CommandText
        {
            get
            {
                return "isready";
            }
        }

        /// <summary>
        /// Reference to the chess engine controller which manages the actual process.
        /// </summary>
        internal override IEngineController ChessEngineController
        {
            get
            {
                return base.ChessEngineController;
            }

            set
            {
                base.ChessEngineController = value;
                if (base.ChessEngineController != null)
                {
                    base.ChessEngineController.DataReceived += EngineController_DataReceived;
                }
            }
        }

        /// <summary>
        /// Performs disposal for this command.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (ChessEngineController != null)
                {
                    ChessEngineController.DataReceived -= EngineController_DataReceived;
                }
            }
            base.Dispose(disposing);
        }

        private void EngineController_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (ResponseIsNotNullOrEmpty(e.Data))
            {
                if (e.Data.StartsWith(READYOK))
                {
                    CommandResponseReceived = true;
                }
            }
        }
    }
}
