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
        /// Performs registration or defers it to later.
        /// </summary>
        /// <param name="later">If true register later.</param>
        /// <param name="name">Name of the user.</param>
        /// <param name="code">Code to send for registration.</param>
        /// <exception cref="ChessterEngineException">Thrown when <paramref name="later"/> is false
        /// and <paramref name="name"/> or <paramref name="code"/> are null.</exception>
        public void SetRegistration(bool later, string name = null, string code = null)
        {
            if (later)
            {
                _commandText = "register later";
            }
            else
            {
                if(string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(code))
                {
                    throw new ChessterEngineException(Messages.NameAndCodeRequiredForRegistration);
                }
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

        /// <summary>
        /// Current status of the registration.
        /// </summary>
        public RegistrationStatus Status { get; private set; }

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
                    ChessEngineController.DataReceived -= ChessEngineController_DataReceived;
                }
            }
            base.Dispose(disposing);
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
                    base.ChessEngineController.DataReceived += ChessEngineController_DataReceived;
                }
            }
        }

        /// <summary>
        /// Occurs when data is received from the engine controller after sending this command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChessEngineController_DataReceived(object sender, ChessCommandReceivedEventArgs e)
        {
            var data = e.Data;

            if (ResponseIsNotNullOrEmpty(data))
            {
                Status = GetRegistrationStatus(data);
                if (Status == RegistrationStatus.Error || Status == RegistrationStatus.Ok)
                {
                    CommandResponseReceived = true;
                }
            }
        }
    }
}
