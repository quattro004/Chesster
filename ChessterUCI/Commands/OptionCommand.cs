using System.Collections.Generic;
using System.Diagnostics;

namespace ChessterUci.Commands
{
    /// <summary>
    /// Used to get and set options on the chess engine.
    /// </summary>
    /// <remarks>
    /// Setoption name [value ] this is sent to the engine when the user wants to change 
    /// the internal parameter of the engine. For the "button" type no value is needed.
    /// One string will be sent for each parameter and this will only be sent when the engine
    /// is waiting. The name of the option in should not be case sensitive and can inlude
    /// spaces like also the value. The substrings "value" and "name" should be avoided in 
    /// and to allow unambiguous parsing, for example do not use = "draw value". Here are 
    /// some strings for the example below:
    ///     "setoption name Nullmove value true\n"
    ///     "setoption name Selectivity value 3\n"
    ///     "setoption name Style value Risky\n"
    ///     "setoption name Clear Hash\n"
    ///     "setoption name NalimovPath value c:\chess\tb\4;c:\chess\tb\5\n"
    ///</remarks>
    public class OptionCommand : ChessCommand
    {
        /// <summary>
        /// Initializes the command used to get and set options on the chess engine.
        /// </summary>
        public OptionCommand() : base()
        {
        }

        /// <summary>
        /// Command used to communicate with the chess engine ("setoption").
        /// </summary>
        public override string CommandText
        {
            get
            {
                return "setoption ";
            }
        }
        /// <summary>
        /// List of <see cref="OptionData"/> used when getting or setting the options.
        /// </summary>
        public IEnumerable<OptionData> OptionValues { get; set; }

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
        /// Occurs when data is received from the engine controller after sending this command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EngineController_DataReceived(object sender, DataReceivedEventArgs e)
        {
            ChessCommandTraceSource.TraceInformation($"EngineController_DataReceived data is {e.Data}.");
        }
    }
}
