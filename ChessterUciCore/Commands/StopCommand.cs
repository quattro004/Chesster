namespace ChessterUciCore.Commands
{
    /// <summary>
    /// Tell the engine to stop calculating as soon as possible, don't forget the 
    /// "bestmove" and possibly the "ponder" token when finishing the search.
    /// </summary>
    public class StopCommand : ChessCommand
    {
        /// <summary>
        /// Command to send to the engine "stop".
        /// </summary>
        public override string CommandText
        {
            get
            {
                return "stop";
            }
        }
    }
}
