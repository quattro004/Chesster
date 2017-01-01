using System;
using ChessterUciCore;
using Xunit;

namespace TestChessterUciCore
{
    public class UniversalChessInterfaceShould
    {
        [Fact]
        public void start_engine_process()
        {
            Console.WriteLine("\tUniversalChessInterfaceShould.start_engine_process");

            using (var uci = new UniversalChessInterface())
            {
                Assert.True(uci.IsEngineProcessRunning, "The engine failed to start.");
                uci.ChessEngineController.KillEngine();
            }
        }

        [Fact]
        public void receive_uciok_after_setting_uci_mode()
        {
            Console.WriteLine("\tUniversalChessInterfaceShould.receive_uciok_after_setting_uci_mode");

            using (var uci = new UniversalChessInterface())
            {
                uci.SetUciMode();
                Assert.True(uci.UciModeComplete, "The engine failed to fully initialize.");
            }
        }

        [Fact]
        public void contain_options_after_initialization()
        {
            Console.WriteLine("\tUniversalChessInterfaceShould.contain_options_after_initialization");

            using (var uci = new UniversalChessInterface())
            {
                uci.SetUciMode();
                Assert.NotNull(uci.ChessEngineOptions);
                Assert.NotEmpty(uci.ChessEngineOptions);
            }
        }
    }
}
