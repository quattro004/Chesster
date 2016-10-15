using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChessterUci
{
    /// <summary>
    /// Defines exception information specfic to Chesster's interactions with the 
    /// underlying chess engine process.
    /// </summary>
    [Serializable]
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

        /// <summary>
        /// Initializes an exception with serialization info and a streaming context.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ChessterEngineException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}
