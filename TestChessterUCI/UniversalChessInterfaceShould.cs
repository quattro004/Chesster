using ChessterUci;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace TestChessterUCI
{
    public class UniversalChessInterfaceShould
    {
        [Fact]
        public void start_engine_process()
        {
            using (var uci = new UniversalChessInterface(ConfigurationManager.AppSettings["ChessEnginePath"]))
            {
                Assert.True(uci.IsEngineProcessRunning, "The engine failed to start.");
            }
        }

        [Fact]
        public async Task receive_uciok_after_setting_uci_mode()
        {
            try
            {
                await TestUtility.PrepareUniversalChessInterface();
                Assert.True(TestUtility.UciObject.UciModeComplete, "The engine failed to fully initialize.");
            }
            finally
            {
                TestUtility.UciObject.Dispose();
            }
        }

        [Fact]
        public async Task contain_options_after_initialization()
        {
            try
            {
                await TestUtility.PrepareUniversalChessInterface();
                Assert.NotNull(TestUtility.UciObject.ChessEngineOptions);
                Assert.NotEmpty(TestUtility.UciObject.ChessEngineOptions);
            }
            finally
            {
                TestUtility.UciObject.Dispose();
            }
        }
    }
}
