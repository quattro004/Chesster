using ChessterUci.Commands;
using Xunit;
using ChessterUci;
using System.Configuration;
using System.Collections.Generic;
using System;
using System.Threading;

namespace TestChessterUCI
{
    public class ChessCommandShould
    {
        [Fact]
        public void receive_readyok_after_sending_isready()
        {
            using (var uci = new UniversalChessInterface(ConfigurationManager.AppSettings["ChessEnginePath"]))
            {
                uci.SetUciMode();
                using (var isReadyCommand = new IsReadyCommand())
                {
                    uci.SendCommand(isReadyCommand);
                    UniversalChessInterface.WaitForResponse(isReadyCommand);

                    Assert.True(isReadyCommand.CommandResponseReceived);
                }
            }
        }

        [Fact]
        public void error_when_engine_doesnt_recognize_command()
        {
            using (var uci = new UniversalChessInterface(ConfigurationManager.AppSettings["ChessEnginePath"]))
            {
                uci.SetUciMode();
                using (var bogusCommand = new BogusCommand())
                {
                    uci.SendCommand(bogusCommand);
                    UniversalChessInterface.WaitForResponse(bogusCommand);

                    Assert.True(bogusCommand.ErrorText.StartsWith("Unknown command"));
                }
            }
        }

        [Fact]
        public void allow_options_to_be_set()
        {
            using (var uci = new UniversalChessInterface(ConfigurationManager.AppSettings["ChessEnginePath"]))
            {
                uci.SetUciMode();
                using(var optionCommand = new OptionCommand())
                {
                    var optionWriteDebugLog = uci.ChessEngineOptions["optionWriteDebugLog"];
                    optionWriteDebugLog.Default = "true";
                    var optionContemptFactor = uci.ChessEngineOptions["optionContemptFactor"];
                    optionContemptFactor.Default = "10";
                    optionCommand.OptionValues.AddRange(new List<OptionData> {
                        optionWriteDebugLog, optionContemptFactor
                    });
                    uci.SendCommand(optionCommand);
                    UniversalChessInterface.WaitForResponse(optionCommand);

                    Assert.True(optionCommand.ErrorText == default(string));
                }
            }
        }

        [Fact]
        public void timeout_when_specified()
        {
            using (var uci = new UniversalChessInterface(ConfigurationManager.AppSettings["ChessEnginePath"]))
            {
                var uciCommand = new UciCommand();
                uciCommand.CommandResponsePeriod = new TimeSpan(0, 0, 0, 0, 10); // 10 milliseconds
                uci.SendCommand(uciCommand);
                Thread.Sleep(50);

                Assert.False(uciCommand.CommandResponseReceived);
                Assert.True(uciCommand.CommandTimeoutElapsed);
            }
        }
    }
}
