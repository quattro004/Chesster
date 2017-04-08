namespace ChessterUciCore
{
    /// <summary>
    /// Options for the api.
    /// </summary>
    public class ChessterOptions
    {
        /// <summary>
        /// Path to the chess engine executable for Windows.
        /// </summary>
        public string ChessEnginePathWindows { get; set; }

        /// <summary>
        /// Path to the chess engine executable for Linux.
        /// </summary>
        public string ChessEnginePathLinux { get; set; }
    }
}