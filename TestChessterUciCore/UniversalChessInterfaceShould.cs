using ChessterUciCore;
using Xunit;

namespace TestChessterUciCore
{
    public class UniversalChessInterfaceShould
    {
        private TestUtility _testUtility;

        public UniversalChessInterfaceShould()
        {
            _testUtility = new TestUtility();
        }

        [Fact]
        public void start_engine_process()
        {
            using (var uci = new UniversalChessInterface(_testUtility.EngineController))
            {
                Assert.True(uci.IsEngineProcessRunning, "The engine failed to start.");
            }
        }

        [Fact]
        public void receive_uciok_after_setting_uci_mode()
        {
            using (var uci = new UniversalChessInterface(_testUtility.EngineController))
            {
                uci.SetUciMode();
                Assert.True(uci.UciModeComplete, "The engine failed to fully initialize.");
            }
        }

        [Fact]
        public void contain_options_after_initialization()
        {
            using (var uci = new UniversalChessInterface(_testUtility.EngineController))
            {
                uci.SetUciMode();
                Assert.NotNull(uci.ChessEngineOptions);
                Assert.NotEmpty(uci.ChessEngineOptions);
            }
        }
    }
}
