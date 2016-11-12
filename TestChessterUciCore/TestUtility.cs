using ChessterUciCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;

namespace TestChessterUciCore
{
    public class TestUtility
    {

        public TestUtility()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.SetBasePath(currentDirectory);
            configurationBuilder.AddJsonFile("config.json");
            Config = configurationBuilder.Build();
            var opSys = Environment.GetEnvironmentVariable("OS");
            if (!string.IsNullOrWhiteSpace(opSys) && opSys.ToLower().Contains("windows"))
            {
               EngineController = new EngineController(Config["ChessEnginePathWindows"]);
            }
            else
            {
                EngineController = new EngineController(Config["ChessEnginePathLinux"]);
            }
        }

        public IConfigurationRoot Config { get; private set; }

        public IEngineController EngineController { get; private set; }
    }
}
