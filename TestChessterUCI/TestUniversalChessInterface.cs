using ChessterUCI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestChessterUCI
{
    public class TestUniversalChessInterface
    {
        [Fact]
        public void initialize_engine()
        {
            var uciClient = new UniversalChessInterface();
            var engineInitialized = uciClient.InitializeEngine();
            Assert.True(engineInitialized, "The chess engine was not initialized properly.");
        }

        [Fact]
        public void start_engine_process()
        {
            var engineController = new EngineController();
            using (var engineProcess = engineController.StartChessEngine())
            {
                Assert.NotNull(engineProcess);
                Assert.True(engineProcess.StartInfo.RedirectStandardInput, "Standard input has not been redirected.");
                Assert.True(engineProcess.StartInfo.RedirectStandardError, "Standard error has not been redirected.");
                Assert.True(engineProcess.StartInfo.RedirectStandardOutput, "Standard output has not been redirected.");
                engineProcess.Close();
            }
        }
    }
}
