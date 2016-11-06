using Microsoft.Extensions.Logging;

namespace ChessterUciCore
{
    /// <summary>
    /// Provides logging for the Chesster library.
    /// </summary>
    public class ChessterLogging
    {
        /// <summary>
        /// <see cref="LoggerFactory"/> used to support logging.
        /// </summary>
        public static ILoggerFactory LoggerFactory { get; } = new LoggerFactory()
            .AddDebug();

        /// <summary>
        /// Creates a logger of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of logger.</typeparam>
        /// <returns>Instance of <see cref="ILogger"/>.</returns>
        public static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();

    }
}
