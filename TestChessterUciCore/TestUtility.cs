using ChessterUciCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace TestChessterUciCore
{
    public class TestUtility : IDisposable
    {
        private bool disposedValue = false; // To detect redundant calls

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

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if(EngineController != null)
                    {
                        EngineController.Dispose();
                        EngineController = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~TestUtility() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
