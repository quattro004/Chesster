using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessterUciCore.Commands
{
    /// <summary>
    /// This is the command to try to register an engine or to tell the engine that registration
    /// will be done later.This command should always be sent if the engine has send 
    /// "registration error" at program startup. The following tokens are allowed:
    ///     * later - the user doesn't want to register the engine now.
    ///     * name - the engine should be registered with the name
    ///     * code - the engine should be registered with the code
    /// Example:
    ///     "register later"
    ///     "register name Stefan MK code 4359874324"
    /// </summary>
    public class RegisterCommand : ChessCommand
    {
        private string _commandText;

        /// <summary>
        /// Performs initialization.
        /// </summary>
        /// <param name="later">If true register later.</param>
        /// <param name="name">Name of the user.</param>
        /// <param name="code">Code to send for registration.</param>
        public RegisterCommand(bool later, string name, string code)
        {
            if (later)
            {
                _commandText = "register later";
            }
            else
            {
                _commandText = string.Format("register name {0} code {1}",  name, code);
            }
        }

        /// <summary>
        /// Command text to send to the chess engine.
        /// </summary>
        public override string CommandText
        {
            get
            {
                return _commandText;
            }
        }
    }
}
