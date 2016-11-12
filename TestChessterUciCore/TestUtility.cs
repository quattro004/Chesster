using ChessterUciCore;
using Microsoft.Extensions.Configuration;
using System;
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
            var configFile = Path.Combine(currentDirectory, "TestChessterUciCore", "config.json");
            configurationBuilder.AddJsonFile(configFile);
            Config = configurationBuilder.Build();
            var opSys = Environment.GetEnvironmentVariable("OS");
            string chessEnginePath = null;
            if (!string.IsNullOrWhiteSpace(opSys) && opSys.ToLower().Contains("windows"))
            {
                chessEnginePath = Path.Combine(currentDirectory, "TestChessterUciCore", Config["ChessEnginePathWindows"]);
            }
            else
            {
                chessEnginePath = Path.Combine(currentDirectory, "TestChessterUciCore", Config["ChessEnginePathLinux"]);
            }
            EngineController = new EngineController(chessEnginePath);
        }

        public IConfigurationRoot Config { get; private set; }

        public IEngineController EngineController { get; private set; }
    }
}
