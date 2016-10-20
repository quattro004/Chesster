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
                Assert.True(uci.IsEngineRunning, "The engine failed to start.");
            }
        }

        [Fact]
        public void receive_uciok_after_initialization()
        {
            using (var uci = new UniversalChessInterface(ConfigurationManager.AppSettings["ChessEnginePath"]))
            {
                uci.InitializationPeriod = 1;
                uci.InitializeEngine();

                Thread.Sleep(1700);
                Assert.True(uci.InitializationComplete, "The engine failed to fully initialize.");
            }
        }

        [Fact]
        public void contain_options_after_initialization()
        {
            using (var uci = new UniversalChessInterface(ConfigurationManager.AppSettings["ChessEnginePath"]))
            {
                uci.InitializationPeriod = 1;
                uci.InitializeEngine();

                Thread.Sleep(1700);
                Assert.True(uci.ChessEngineOptions != null, "The ChessEngineOptions are null!");
                Assert.NotEmpty(uci.ChessEngineOptions);
            }
        }
    }
}
