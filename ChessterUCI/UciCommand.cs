using System;
using System.Diagnostics;

namespace ChessterUci
{
    /// <summary>
    /// Tell engine to use the uci (universal chess interface),	this will be sent
    /// once as a first command after program boot to tell the engine to switch to uci mode.
    /// After receiving the uci command the engine must identify itself with the "id" command
    /// and send the "option" commands to tell the GUI which engine settings the engine 
    /// supports if any. After that the engine should send "uciok" to acknowledge the uci mode.
    /// If no uciok is sent within a certain time period, the engine task will be killed by 
    /// the client.
    /// </summary>
    public class UciCommand : ChessCommand
    {
        /// <summary>
        /// Initializes the "uci" command for use with the engine controller.
        /// </summary>
        /// <param name="engineController">Engine controller which manages the chess engine
        /// process.</param>
        public UciCommand(IEngineController engineController) : base(engineController)
        {
            if (engineController == null)
            {
                throw new ChessterEngineException(Messages.NullEngineController);
            }
            engineController.DataReceived += EngineController_DataReceived;
            engineController.ErrorReceived += EngineController_ErrorReceived;
        }

        /// <summary>
        /// Command used to communicate with the chess engine ("uci").
        /// </summary>
        public override string CommandText
        {
            get
            {
                return "uci";
            }
        }

        /// <summary>
        /// Occurs when an error is received from the engine controller after sending this command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EngineController_ErrorReceived(object sender, DataReceivedEventArgs e)
        {
            ChessCommandTraceSource.TraceEvent(TraceEventType.Error, 0, $"EngineController_ErrorReceived data is {e.Data}.");
        }

        /// <summary>
        /// Occurs when data is received from the engine controller after sending this command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EngineController_DataReceived(object sender, DataReceivedEventArgs e)
        {
            ChessCommandTraceSource.TraceInformation($"EngineController_DataReceived data is {e.Data}.");
        }
    }
}
