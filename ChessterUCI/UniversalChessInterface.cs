using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessterUci
{
    /// <summary>
    /// Implementation of the Universal Chess Interface http://wbec-ridderkerk.nl/html/UCIProtocol.html
    /// </summary>
    public class UniversalChessInterface : IDisposable
    {
        /// <summary>
        /// <see cref="IEngineController"/> which controls and communicates with the chess engine process.
        /// </summary>
        private IEngineController ChessEngineController { get; set; }

        /// <summary>
        /// Performs initialization for the chess interface to the engine.
        /// </summary>
        public UniversalChessInterface(string chessEnginePath)
        {
            ChessEngineController = new EngineController(chessEnginePath);
            SetUciMode();
        }

        /// <summary>
        /// Determines whether or not the chess engine is currently running.
        /// </summary>
        public bool IsEngineRunning
        {
            get
            {
                return ChessEngineController != null && ChessEngineController.IsEngineRunning;
            }
        }

        /// <summary>
        /// Sends an asynchronous command to the chess engine to enable UCI mode.
        /// </summary>
        private void SetUciMode()
        {
            var uciCommand = new UciCommand(ChessEngineController);
            uciCommand.SendCommand();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Disposes of resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    ChessEngineController.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UniversalChessInterface() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        /// <summary>
        /// This code added to correctly implement the disposable pattern.
        /// </summary>
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
