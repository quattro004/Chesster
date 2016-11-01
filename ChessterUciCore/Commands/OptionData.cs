namespace ChessterUciCore.Commands
{
    /// <summary>
    /// Contains data for getting and setting options in the chess engine.
    /// <seealso cref="OptionCommand"/>
    /// </summary>
    public struct OptionData
    {
        /// <summary>
        /// Name of the option.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Type of the option: check, string, spin, etc.
        /// </summary>
        public string OptionType { get; set; }
        /// <summary>
        /// Default value of the option.
        /// </summary>
        public string Default { get; set; }
        /// <summary>
        /// Minimum value of the option.
        /// </summary>
        public string Min { get; set; }
        /// <summary>
        /// Maximum value of the option.
        /// </summary>
        public string Max { get; set; }
        /// <summary>
        /// Name and author of the chess engine.
        /// </summary>
        public IdData Id { get; set; }
    }
}