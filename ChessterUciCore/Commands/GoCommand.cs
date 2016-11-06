using System.Text;

namespace ChessterUciCore.Commands
{
    /// <summary>
    /// Start calculating on the current position set up with the <see cref="PositionCommand"/> command.
    /// There are a number of commands that can follow this command, all will be sent in the same string.
    /// If one command is not sent its value shouldn't be interpreted as it would not influence the search.
    /// * searchmoves - restrict search to this moves only Example: After "position startpos" and 
    ///     "go infinite searchmoves e2e4 d2d4" the engine should only search the two moves e2e4 and d2d4
    ///     in the initial position.
    /// * ponder - start searching in pondering mode. Do not exit the search in ponder mode, even if it's mate!
    ///     This means that the last move sent in the position string is the ponder move. The engine can do 
    ///     what it wants to do, but after a "ponderhit" command it should execute the suggested move to ponder 
    ///     on. This means that the ponder move sent by the GUI can be interpreted as a recommendation about which 
    ///     move to ponder. However, if the engine decides to ponder on a different move, it should not display 
    ///     any mainlines as they are likely to be misinterpreted by the GUI because the GUI expects the engine to 
    ///     ponder on the suggested move.
    /// * wtime - white has x msec left on the clock
    /// * btime - black has x msec left on the clock
    /// * winc - white increment per move in mseconds if x > 0	
    /// * binc - black increment per move in mseconds if x > 0
    /// * movestogo - there are x moves to the next time control, this will only be sent if x > 0, if you don't set 
    ///     this and set the wtime and btime it's sudden death
    /// * depth - search x plies only.
    /// * nodes - search x nodes only,
    /// * mate - search for a mate in x moves
    /// * movetime - search exactly x mseconds
    /// * infinite - search until the <see cref="StopCommand"/> command. Do not exit the search without being told 
    ///     so in this mode!
    /// </summary>
    public class GoCommand : ChessCommand
    {
        private string _commandText;
        
        #region Properties

        /// <summary>
        /// Restrict search to these moves only.
        /// </summary>
        public string SearchMoves { get; set; }

        /// <summary>
        /// Start searching in pondering mode. Do not exit the search in ponder mode, even if it's mate!
        /// This means that the last move sent in the <see cref="PositionCommand"/> is the ponder move.
        /// </summary>
        /// <remarks>The engine can do what it wants to do, but after a <see cref="PonderHitCommand"/> 
        /// command it should execute the suggested move to ponder on. This means that the ponder move 
        /// sent by the GUI can be interpreted as a recommendation about which move to ponder.</remarks>
        public bool Ponder { get; set; }

        /// <summary>
        /// White has x milliseconds left on the clock.
        /// </summary>
        public double WhiteTime { get; set; }

        /// <summary>
        /// Black has x milliseconds left on the clock.
        /// </summary>
        public double BlackTime { get; set; }

        /// <summary>
        /// White increment per move in milliseconds if x > 0.
        /// </summary>
        public double WhiteIncrement { get; set; }

        /// <summary>
        /// Black increment per move in milliseconds if x > 0.
        /// </summary>
        public double BlackIncrement { get; set; }

        /// <summary>
        /// There are x moves to the next time control, this will only be sent if x > 0, 
        /// if you don't set this and set the wtime and btime it's sudden death.
        /// </summary>
        public int MovesToGo { get; set; }

        /// <summary>
        /// Search x plies only.
        /// </summary>
        public long Depth { get; set; }

        /// <summary>
        /// Search x nodes only.
        /// </summary>
        public long Nodes { get; set; }

        /// <summary>
        /// Search for a mate in x moves.
        /// </summary>
        public int Mate { get; set; }

        /// <summary>
        /// Search exactly x milliseconds.
        /// </summary>
        public double MoveTime { get; set; }

        /// <summary>
        /// Search until the <see cref="StopCommand"/> command.
        /// </summary>
        public bool Infinite { get; set; }
        
        /// <summary>
        /// Command(s) to send to the engine.
        /// </summary>
        public override string CommandText
        {
            get
            {
                _commandText = "go{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}";
                return string.Format(_commandText, GetInfiniteCommand(), GetSearchMovesCommand(),
                    GetPonderCommand(), GetWhiteTimeCommand(), GetBlackTimeCommand(), GetWhiteIncrementCommand(),
                    GetBlackIncrementCommand(), GetMovesToGoCommand(), GetDepthCommand(), GetNodesCommand(),
                    GetMateCommand(), GetMoveTimeCommand());
            }
        }

        /// <summary>
        /// Information sent back from the chess engine regarding its search.
        /// </summary>
        public StringBuilder InfoResponse { get; } = new StringBuilder();

        /// <summary>
        /// The best move the engine came up with while searching.
        /// </summary>
        public string BestMove { get; private set; }

        #endregion

        #region Private Methods

        private string GetInfiniteCommand()
        {
            return Infinite ? " infinite" : string.Empty;
        }

        private string GetSearchMovesCommand()
        {
            return string.IsNullOrWhiteSpace(SearchMoves) 
                ? string.Empty 
                : string.Format(" searchmoves {0}", SearchMoves.ToString());
        }

        private string GetPonderCommand()
        {
            return Ponder ? " ponder" : string.Empty;
        }

        private string GetWhiteTimeCommand()
        {
            return string.Format(" wtime {0}", WhiteTime);
        }

        private string GetBlackTimeCommand()
        {
            return string.Format(" btime {0}", BlackTime);
        }

        private string GetWhiteIncrementCommand()
        {
            return WhiteIncrement > 0 
                ? string.Format(" winc {0}", WhiteIncrement)
                : string.Empty;
        }

        private string GetBlackIncrementCommand()
        {
            return BlackIncrement > 0
                ? string.Format(" binc {0}", BlackIncrement)
                : string.Empty;
        }

        private string GetMovesToGoCommand()
        {
            return MovesToGo > 0
                ? string.Format(" movestogo {0}", MovesToGo)
                : string.Empty;
        }

        private string GetDepthCommand()
        {
            return Depth > 0
                ? string.Format(" depth {0}", Depth)
                : string.Empty;
        }

        private string GetNodesCommand()
        {
            return Nodes > 0
                ? string.Format(" nodes {0}", Nodes)
                : string.Empty;
        }

        private string GetMateCommand()
        {
            return Mate > 0
                ? string.Format(" mate {0}", Mate)
                : string.Empty;
        }

        private string GetMoveTimeCommand()
        {
            return MoveTime > 0
                ? string.Format(" movetime {0}", MoveTime)
                : string.Empty;
        }

        #endregion

        internal override void SendCommand()
        {
            InfoResponse.Clear();
            base.SendCommand();
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

        private void ChessEngineController_DataReceived(object sender, ChessCommandReceivedEventArgs e)
        {
            var data = e.Data;
            if (ResponseIsNotNullOrEmpty(data))
            {
                if (data.StartsWith("bestmove"))
                {
                    BestMove = data;
                    CommandResponseReceived = true;
                    return;
                }
                InfoResponse.AppendLine(data);
            }
        }
    }
}
