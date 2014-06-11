using ChessterUCI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestChessterUCI
{
    public class TestEngineController
    {
        [Fact]
        public void start_engine_process()
        {
            using (var engineController = new EngineController())
            {
                engineController.StartChessEngine();
                Assert.True(engineController.IsEngineRunning, "The engine failed to start.");
            }
        }

        [Fact(Timeout=2000)]
        public void set_uci_mode()
        {
            using (var engineController = new EngineController())
            {
                var waitingForResponse = true;
                engineController.ErrorReceived += (sender, e) => 
                {
                    Assert.True(false, string.Format("Received an error from the chess engine\r\n{0}", e.Data));
                };
                engineController.DataReceived += (sender, e) =>
                {
                    Debug.WriteLine("Output received from chess engine");
                    Debug.WriteLine("\t{0}", e.Data);
                    if (e.Data.StartsWith("uciok")) { waitingForResponse = false; }
                };
                engineController.StartChessEngine();
                engineController.SendCommand("uci");
                while(waitingForResponse)
                {
                    System.Threading.Thread.Sleep(10);
                }
            }
        }
    }
}
