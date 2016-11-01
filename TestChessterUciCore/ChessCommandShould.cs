using ChessterUciCore.Commands;
using Xunit;
using ChessterUciCore;
using System;
using System.Threading;
using System.Diagnostics;

namespace TestChessterUciCore
{
    public class ChessCommandShould
    {
        private TestUtility _testUtility;

        public ChessCommandShould()
        {
            _testUtility = new TestUtility();
        }

        [Fact]
        public void receive_readyok_after_sending_isready()
        {
            using (var uci = new UniversalChessInterface(_testUtility.Config["ChessEnginePath"]))
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
            using (var uci = new UniversalChessInterface(_testUtility.Config["ChessEnginePath"]))
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
            using (var uci = new UniversalChessInterface(_testUtility.Config["ChessEnginePath"]))
            {
                uci.SetUciMode();
                using(var optionCommand = new OptionCommand())
                {
                    var optionWriteDebugLog = uci.ChessEngineOptions["optionWriteDebugLog"];
                    optionWriteDebugLog.Default = "true";
                    var optionContemptFactor = uci.ChessEngineOptions["optionContemptFactor"];
                    optionContemptFactor.Default = "10";
                    optionCommand.OptionValues.Add(optionWriteDebugLog);
                    optionCommand.OptionValues.Add(optionContemptFactor);
                    uci.SendCommand(optionCommand);
                    UniversalChessInterface.WaitForResponse(optionCommand);

                    Assert.True(optionCommand.ErrorText == default(string));
                }
            }
        }

        [Fact]
        public void timeout_when_specified()
        {
            using (var uci = new UniversalChessInterface(_testUtility.Config["ChessEnginePath"]))
            {
                var uciCommand = new UciCommand();
                uciCommand.CommandResponsePeriod = new TimeSpan(0, 0, 0, 0, 10); // 10 milliseconds
                uci.SendCommand(uciCommand);
                Thread.Sleep(100);

                Assert.False(uciCommand.CommandResponseReceived);
                Assert.True(uciCommand.CommandTimeoutElapsed);
            }
        }

        [Fact]
        public void quit_to_end_program()
        {
            using (var uci = new UniversalChessInterface(_testUtility.Config["ChessEnginePath"]))
            {
                uci.SetUciMode();
                using (var quitCommand = new QuitCommand())
                {
                    uci.SendCommand(quitCommand);

                    Assert.True(quitCommand.ErrorText == default(string));
                }
            }
        }

        [Fact]
        public void allow_the_engine_to_go()
        {
            using (var uci = new UniversalChessInterface(_testUtility.Config["ChessEnginePath"]))
            {
                uci.SetUciMode();
                using (var isReadyCommand = new IsReadyCommand())
                {
                    uci.SendCommand(isReadyCommand);
                    UniversalChessInterface.WaitForResponse(isReadyCommand);

                    Assert.True(isReadyCommand.CommandResponseReceived);
                    using(var newGameCommand = new UciNewGame())
                    {
                        uci.SendCommand(newGameCommand);
                        uci.SendCommand(isReadyCommand);
                        UniversalChessInterface.WaitForResponse(isReadyCommand);

                        using (var positionCommand = new PositionCommand())
                        {
                            positionCommand.IsStartPosition = true;
                            positionCommand.FenString = "e2e4 e7e5";
                            uci.SendCommand(positionCommand);

                            using (var goCommand = new GoCommand())
                            {
                                goCommand.Infinite = true;
                                uci.SendCommand(goCommand);
                                Thread.Sleep(500); // Allow the engine time to "think"
                                uci.SendCommand(new StopCommand());
                                UniversalChessInterface.WaitForResponse(goCommand);

                                Assert.True(!string.IsNullOrWhiteSpace(goCommand.InfoResponse.ToString()));
                                Debug.WriteLine(goCommand.InfoResponse.ToString());
                                Assert.True(!string.IsNullOrWhiteSpace(goCommand.BestMove));
                                Debug.WriteLine(goCommand.BestMove);

                                uci.SendCommand(new QuitCommand());
                            }
                        }
                    }
                }
            }
        }
    }
}
