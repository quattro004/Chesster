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
            var uciClient = new ChessterUciClient();
            var engineInitialized = uciClient.InitializeEngine();
            Assert.True(engineInitialized, "The chess engine was not initialized properly.");
        }
    }
}
