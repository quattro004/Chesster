using ChessterUci;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public async void receive_uciok_after_initialization()
        {
            using (var uci = new UniversalChessInterface(ConfigurationManager.AppSettings["ChessEnginePath"]))
            {
                await uci.InitializeEngine();

                Assert.True(uci.InitializationComplete, "The engine failed to fully initialize.");
            }
        }

        [Fact]
        public async void contain_options_after_initialization()
        {
            using (var uci = new UniversalChessInterface(ConfigurationManager.AppSettings["ChessEnginePath"]))
            {
                await uci.InitializeEngine();

                Assert.True(uci.ChessEngineOptions != null, "The ChessEngineOptions are null!");
                Assert.NotEmpty(uci.ChessEngineOptions);
            }
        }

        [Fact]
        public async void receive_readyok_after_sending_isready()
        {
            using (var uci = new UniversalChessInterface(ConfigurationManager.AppSettings["ChessEnginePath"]))
            {
                await uci.InitializeEngine();
                using (var isReadyCommand = new IsReadyCommand(uci.ChessEngineController))
                {
                    await isReadyCommand.SendCommand();

                    Assert.True(isReadyCommand.ReceivedReadyOk);
                }
            }
        }
    }
}
