using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessterUci
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
        /// <param name="engineController">Engine controller which manages the chess engine
        /// process.</param>
        public IsReadyCommand(IEngineController engineController) : base(engineController)
        {
            engineController.DataReceived += EngineController_DataReceived;
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
        /// Sent when the engine has received an "isready" command and has
        /// processed all input and is ready to accept new commands now.
        /// </summary>
        public bool ReceivedReadyOk { get; private set; }

        private void EngineController_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (ResponseIsNotNullOrEmpty(e.Data))
            {
                if (e.Data.StartsWith(READYOK))
                {
                    ReceivedReadyOk = true;
                }
            }
        }
    }
}
