namespace ChessterUciCore.Commands
{
    /// <summary>
    /// Registration status within the chess engine.
    /// </summary>
    public enum RegistrationStatus
    {
        /// <summary>
        /// The registration process hasn't started yet.
        /// </summary>
        NotInitiated,
        /// <summary>
        /// The engine is checking registration.
        /// </summary>
        Checking,
        /// <summary>
        /// Registration was successful.
        /// </summary>
        Ok,
        /// <summary>
        /// Registration failed.
        /// </summary>
        Error
    }
}
