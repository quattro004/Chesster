using ChessterUciCore;
using Microsoft.Extensions.Logging;
using Xunit;

namespace TestChessterUciCore
{
    public class UniversalChessInterfaceShould
    {
        private static ILogger Logger {get;} = TestUtility.LoggerFactory.CreateLogger<UniversalChessInterfaceShould>();
        
        [Fact]
        public void start_engine_process()
        {
            Logger.LogInformation("\tUniversalChessInterfaceShould.start_engine_process");

            using (var uci = new UniversalChessInterface(TestUtility.ChessEnginePath))
            {
                Assert.True(uci.IsEngineProcessRunning, "The engine failed to start.");
                uci.ChessEngineController.KillEngine();
            }
        }

        [Fact]
        public void receive_uciok_after_setting_uci_mode()
        {
            Logger.LogInformation("\tUniversalChessInterfaceShould.receive_uciok_after_setting_uci_mode");
            //
            // Note: the UciCommand is sent to enable uci mode during construction of the UniversalChessInterface.
            using (var uci = new UniversalChessInterface(TestUtility.ChessEnginePath))
            {
                Assert.True(uci.UciModeComplete, "The engine failed to fully initialize.");
            }
        }

        [Fact]
        public void contain_options_after_initialization()
        {
            Logger.LogInformation("\tUniversalChessInterfaceShould.contain_options_after_initialization");

            using (var uci = new UniversalChessInterface(TestUtility.ChessEnginePath))
            {
                Assert.NotNull(uci.ChessEngineOptions);
                Assert.NotEmpty(uci.ChessEngineOptions);
            }
        }
    }
}
