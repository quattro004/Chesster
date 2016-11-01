namespace ChessterUciCore.Commands
{
    /// <summary>
    /// Contains data that identifies the chess engine.
    /// <seealso cref="UciCommand"/>
    /// </summary>
    public class IdData
    {
        /// <summary>
        /// This must be sent after receiving the "uci" command to identify the engine,
        /// e.g. "id name Shredder X.Y\n"
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// This must be sent after receiving the "uci" command to identify the engine,
        /// e.g. "id author Stefan MK\n"
        /// </summary>
        public string Author { get; set; }
    }
}