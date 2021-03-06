﻿using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ChessterUciCore.Commands
{
    /// <summary>
    /// Used to get and set options on the chess engine. After setting options the GUI should send an 
    /// <see cref="IsReadyCommand"/> to wait for the engine to finish intializing.
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
        private string _commandText;
        /// <summary>
        /// Initializes the command used to get and set options on the chess engine.
        /// </summary>
        public OptionCommand() : base()
        {
            OptionValues = new Collection<OptionData>();
        }

        /// <summary>
        /// Command used to communicate with the chess engine ("setoption").
        /// </summary>
        public override string CommandText { get { return _commandText; } }

        /// <summary>
        /// List of <see cref="OptionData"/> used when getting or setting the options.
        /// </summary>
        public Collection<OptionData> OptionValues { get; }

        /// <summary>
        /// Overridden to send for each option in OptionValues.
        /// </summary>
        public override async Task SendAsync()
        {
            foreach (var option in OptionValues)
            {
                _commandText = string.Format("setoption name {0} value {1}", option.Name, option.Default);
                await base.SendAsync();
            }
        }
    }
}
