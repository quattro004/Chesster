using ChessterUciCore;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace TestChessterUciCore
{
    /// <summary>
    /// Contains common test functionality.
    /// </summary>
    public class TestUtility
    {
        private static ILoggerFactory _loggerFactory;

        /// <summary>
        /// Used to setup NLog for use with the ChessterLogging class.
        /// </summary>
        public static ILoggerFactory LoggerFactory 
        {
            get
            {
                if (null == _loggerFactory) 
                {
                    _loggerFactory = ChessterLogging.LoggerFactory.AddNLog();
                    _loggerFactory.ConfigureNLog("nlog.config");
                }
                return _loggerFactory;
            }
        }
    }
}