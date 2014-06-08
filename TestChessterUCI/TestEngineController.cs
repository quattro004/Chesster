using ChessterUCI;
using System;
using System.Collections.Generic;
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
                Assert.NotNull(engineController.ChessEngineProcess);
                Assert.True(engineController.ChessEngineProcess.StartInfo.RedirectStandardInput, "Standard input has not been redirected.");
                Assert.True(engineController.ChessEngineProcess.StartInfo.RedirectStandardError, "Standard error has not been redirected.");
                Assert.True(engineController.ChessEngineProcess.StartInfo.RedirectStandardOutput, "Standard output has not been redirected.");
                engineController.ChessEngineProcess.Close();
            }
        }
    }
}
