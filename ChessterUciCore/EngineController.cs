﻿using ChessterUciCore.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace ChessterUciCore
{
    /// <summary>
    /// Controls the chess engine process. Any UCI compliant chess engine can be used.
    /// </summary>
    public class EngineController : IDisposable, IEngineController
    {
        private bool _disposed;
        private Process _chessEngineProcess;

        /// <summary>
        /// Event which is published when data is received from the chess engine's standard output stream.
        /// </summary>
        public event EventHandler<ChessCommandReceivedEventArgs> DataReceived;
        /// <summary>
        /// Event which is published when data is received from the chess engine's standard error stream.
        /// </summary>
        public event EventHandler<ChessCommandReceivedEventArgs> ErrorReceived;

        /// <summary>
        /// Logger for this class.
        /// </summary>
        ILogger Logger { get; } =  ChessterLogging.CreateLogger<EngineController>();

        /// <summary>
        /// Initializes the controller and creates the chess engine process.
        /// </summary>
        /// <param name="chessterOptionsAccessor">Options such as the path to the engine exe.</param>
        public EngineController(IOptions<ChessterOptions> chessterOptionsAccessor)
        {
            if(null == chessterOptionsAccessor)
            {
                throw new ArgumentNullException(nameof(chessterOptionsAccessor));
            }
            var chessEnginePath = default(string);
            var chessterOptions = chessterOptionsAccessor.Value;
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                chessEnginePath = chessterOptions.ChessEnginePathWindows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                chessEnginePath = chessterOptions.ChessEnginePathLinux;
            }
            StartChessEngine(chessEnginePath);
        }

        
        /// <summary>
        /// Initializes the controller and creates the chess engine process.
        /// </summary>
        /// <param name="chessEnginePath">Path to the engine exe.</param>
        public EngineController(string chessEnginePath)
        {
            StartChessEngine(chessEnginePath);
        }

        /// <summary>
        /// Determines whether the chess engine process is running.
        /// </summary>
        public bool IsEngineRunning
        {
            get
            {
                return _chessEngineProcess != null && !_chessEngineProcess.HasExited;
            }
        }

        /// <summary>
        /// Frees up managed and unmanaged resources and suppresses the finalizer.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Provides the implementation for disposing resources.
        /// </summary>
        /// <param name="disposing"><see cref="bool"/> specifying whether to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Free any managed objects here. 
                //
                if (_chessEngineProcess != null)
                {
                    _chessEngineProcess.OutputDataReceived -= _chessEngineProcess_OutputDataReceived;
                    _chessEngineProcess.ErrorDataReceived -= _chessEngineProcess_ErrorDataReceived;
                    _chessEngineProcess.CancelErrorRead();
                    _chessEngineProcess.CancelOutputRead();
                    _chessEngineProcess.Dispose();
                    _chessEngineProcess = null;
                }
            }

            // Free any unmanaged objects here. 
            //
            _disposed = true;
        }

        /// <summary>
        /// Fires when data is received from the chess engine.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments received from the chess engine.</param>
        protected virtual void OnRaiseDataReceived(object sender, ChessCommandReceivedEventArgs e)
        {
            DataReceived?.Invoke(sender, e);
        }

        /// <summary>
        /// Fires when an error is received from the chess engine.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments received from the chess engine.</param>
        protected virtual void OnRaiseErrorReceived(object sender, ChessCommandReceivedEventArgs e)
        {
            ErrorReceived?.Invoke(sender, e);
        }

        /// <summary>
        /// Sends the specified <paramref name="command"/> command to the chess engine's standard
        /// input stream asynchronously. The <see cref="DataReceived"/> or <see cref="ErrorReceived"/>
        /// events will fire when the engine responds.
        /// </summary>
        /// <param name="command"></param>
        public void SendCommand(string command)
        {
            Logger.LogInformation($"Sending the {command} command");
            _chessEngineProcess.StandardInput.WriteLine(command);
        }

        /// <summary>
        /// Shuts down the actual process hosting the chess engine.
        /// </summary>
        public void KillEngine()
        {
            Logger.LogInformation("Killing the chess engine process");

            if (_chessEngineProcess != null)
            {
                _chessEngineProcess.Kill();
            }
        }

        #region Private Methods

        private void _chessEngineProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                OnRaiseErrorReceived(sender, new ChessCommandReceivedEventArgs(e.Data));
            }
        }

        private void _chessEngineProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                var chessCommandArgs = new ChessCommandReceivedEventArgs(e.Data);

                if (e.Data.StartsWith("Unknown command"))
                {
                    OnRaiseErrorReceived(sender, chessCommandArgs);
                }
                else
                {
                    OnRaiseDataReceived(sender, chessCommandArgs);
                }
            }
        }

        /// <summary>
        /// Starts the chess engine process and redirects standard error, input and output since UCI chess engines
        /// use standard input and output to communicate via text commands.
        /// </summary>
        private void StartChessEngine(string chessEnginePath)
        {
            if (string.IsNullOrWhiteSpace(chessEnginePath))
            {
                throw new ChessterEngineException(Messages.ChessEnginePathNotSupplied);
            }
            Logger.LogInformation($"Starting the chess engine process, path is {chessEnginePath}");
            _chessEngineProcess = new Process();
            _chessEngineProcess.StartInfo.FileName = chessEnginePath;
            _chessEngineProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(_chessEngineProcess.StartInfo.FileName);
            _chessEngineProcess.StartInfo.UseShellExecute = false;
            _chessEngineProcess.StartInfo.CreateNoWindow = true;
            _chessEngineProcess.StartInfo.RedirectStandardError = true;
            _chessEngineProcess.StartInfo.RedirectStandardInput = true;
            _chessEngineProcess.StartInfo.RedirectStandardOutput = true;
            _chessEngineProcess.OutputDataReceived += _chessEngineProcess_OutputDataReceived;
            _chessEngineProcess.ErrorDataReceived += _chessEngineProcess_ErrorDataReceived;
            _chessEngineProcess.Start();
            _chessEngineProcess.StandardInput.AutoFlush = true;
            _chessEngineProcess.BeginErrorReadLine();
            _chessEngineProcess.BeginOutputReadLine();
            Logger.LogInformation($"The chess engine process is started, process id {_chessEngineProcess.Id}");
        }
        
        #endregion
    }
}
