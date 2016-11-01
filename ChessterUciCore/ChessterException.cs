using System;

namespace ChessterUciCore
{
    /// <summary>
    /// Defines exception information specfic to Chesster's interactions with the 
    /// underlying chess engine process.
    /// </summary>
    public class ChessterEngineException : Exception
    {
        /// <summary>
        /// Initializes an exception.
        /// </summary>
        public ChessterEngineException()
        {
        }

        /// <summary>
        /// Initializes an exception with a message.
        /// </summary>
        /// <param name="message">User friendly message to send with the exception.</param>
        public ChessterEngineException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes an exception with a message and inner exception
        /// </summary>
        /// <param name="message">User friendly message to send with the exception.</param>
        /// <param name="inner">Inner exception that likely caused this exception to throw.</param>
        public ChessterEngineException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
