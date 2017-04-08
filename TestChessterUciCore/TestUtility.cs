using System;
using System.IO;
using ChessterUciCore;
using Microsoft.Extensions.Configuration;
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
        private static IConfigurationRoot _config;

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
        
        /// <summary>
        /// Used to access the app settings.
        /// </summary>
        public static IConfigurationRoot Configuration 
        { 
            get
            {
                if(null == _config)
                {
                    var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json");

                    _config = builder.Build();
                }
                return _config;
            }
        }

        /// <summary>
        /// Path to the chess engine executable.
        /// </summary>
        public static string ChessEnginePath
        { 
            get
            {
                var opSys = Environment.GetEnvironmentVariable("OS");
                var currentDirectory = Directory.GetCurrentDirectory();

                if (!string.IsNullOrWhiteSpace(opSys) && opSys.ToLower().Contains("windows"))
                {
                    return Path.Combine(currentDirectory, Configuration["ChessEnginePathWindows"]);
                }
                else
                {
                    return Path.Combine(currentDirectory, Configuration["ChessEnginePathLinux"]);
                }
            } 
        }
    }
}