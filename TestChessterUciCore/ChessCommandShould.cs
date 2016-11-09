using ChessterUciCore.Commands;
using Xunit;
using ChessterUciCore;
using System;
using System.Threading;
using System.Diagnostics;
using TestChessterUciCore.Fakes;

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
            using (var uci = new UniversalChessInterface(_testUtility.EngineController))
            {
                uci.SetUciMode();
                using (var isReadyCommand = uci.CommandFactory.CreateCommand<IsReadyCommand>())
                {
                    uci.SendCommand(isReadyCommand);
                    uci.WaitForResponse(isReadyCommand);

                    Assert.True(isReadyCommand.CommandResponseReceived);
                }
            }
        }

        [Fact]
        public void error_when_engine_doesnt_recognize_command()
        {
            using (var uci = new UniversalChessInterface(_testUtility.EngineController))
            {
                uci.SetUciMode();
                using (var bogusCommand = uci.CommandFactory.CreateCommand<BogusCommand>())
                {
                    uci.SendCommand(bogusCommand);
                    uci.WaitForResponse(bogusCommand);

                    Assert.True(bogusCommand.ErrorText.StartsWith("Unknown command"));
                }
            }
        }

        [Fact]
        public void allow_options_to_be_set()
        {
            using (var uci = new UniversalChessInterface(_testUtility.EngineController))
            {
                uci.SetUciMode();
                using(var optionCommand = uci.CommandFactory.CreateCommand<OptionCommand>())
                {
                    var optionWriteDebugLog = uci.ChessEngineOptions["optionDebugLogFile"];
                    optionWriteDebugLog.Default = "true";
                    var optionContemptFactor = uci.ChessEngineOptions["optionContempt"];
                    optionContemptFactor.Default = "10";
                    optionCommand.OptionValues.Add(optionWriteDebugLog);
                    optionCommand.OptionValues.Add(optionContemptFactor);
                    uci.SendCommand(optionCommand);
                    uci.WaitForResponse(optionCommand);

                    Assert.True(optionCommand.ErrorText == default(string));
                }
            }
        }

        [Fact]
        public void timeout_when_specified()
        {
            Debug.WriteLine("timeout_when_specified()");
            using (var uci = new UniversalChessInterface(_testUtility.EngineController))
            {
                using (var uciCommand = uci.CommandFactory.CreateCommand<UciCommand>())
                {
                    uciCommand.CommandResponsePeriod = new TimeSpan(0, 0, 0, 0, 25); // 25 milliseconds
                    uci.SendCommand(uciCommand);
                    Thread.Sleep(50);

                    Assert.False(uciCommand.CommandResponseReceived);
                    Assert.True(uciCommand.CommandTimeoutElapsed);
                }
            }
        }

        [Fact]
        public void quit_to_end_program()
        {
            using (var uci = new UniversalChessInterface(_testUtility.EngineController))
            {
                uci.SetUciMode();
                using (var quitCommand = uci.CommandFactory.CreateCommand<QuitCommand>())
                {
                    uci.SendCommand(quitCommand);

                    Assert.True(quitCommand.ErrorText == default(string));
                }
            }
        }

        [Fact]
        public void allow_the_engine_to_go()
        {
            using (var uci = new UniversalChessInterface(_testUtility.EngineController))
            {
                uci.SetUciMode();
                using (var isReadyCommand = uci.CommandFactory.CreateCommand<IsReadyCommand>())
                {
                    uci.SendCommand(isReadyCommand);
                    uci.WaitForResponse(isReadyCommand);

                    Assert.True(isReadyCommand.CommandResponseReceived);
                    using(var newGameCommand = uci.CommandFactory.CreateCommand<UciNewGame>())
                    {
                        uci.SendCommand(newGameCommand);
                        uci.SendCommand(isReadyCommand);
                        uci.WaitForResponse(isReadyCommand);

                        using (var positionCommand = uci.CommandFactory.CreateCommand<PositionCommand>())
                        {
                            positionCommand.IsStartPosition = true;
                            positionCommand.FenString = "e2e4 e7e5";
                            uci.SendCommand(positionCommand);

                            using (var goCommand = uci.CommandFactory.CreateCommand<GoCommand>())
                            {
                                goCommand.Infinite = true;
                                uci.SendCommand(goCommand);
                                Thread.Sleep(500); // Allow the engine time to "think"
                                uci.SendCommand(uci.CommandFactory.CreateCommand<StopCommand>());
                                uci.WaitForResponse(goCommand);

                                Assert.True(!string.IsNullOrWhiteSpace(goCommand.InfoResponse.ToString()));
                                Debug.WriteLine(goCommand.InfoResponse.ToString());
                                Assert.True(!string.IsNullOrWhiteSpace(goCommand.BestMove));
                                Debug.WriteLine(goCommand.BestMove);

                                uci.SendCommand(uci.CommandFactory.CreateCommand<QuitCommand>());
                            }
                        }
                    }
                }
            }
        }

        [Fact]
        public void support_registration()
        {
            using (var uci = new UniversalChessInterface(new FakeEngineController()))
            {
                uci.SetUciMode();
                using (var registerCommand = uci.CommandFactory.CreateCommand<RegisterCommand>())
                {
                    registerCommand.SetRegistration(false, "Reese", "23098HHHJ");
                    uci.SendCommand(registerCommand);
                    uci.WaitForResponse(registerCommand);

                    if (!string.IsNullOrWhiteSpace(registerCommand.ErrorText))
                    {
                        Assert.False(registerCommand.ErrorText.StartsWith("Unknown command"));
                    }
                    Assert.True(registerCommand.CommandText == "register name Reese code 23098HHHJ");
                    Assert.True(registerCommand.CommandResponseReceived);
                    Assert.True(registerCommand.Status == RegistrationStatus.Ok);
                }
            }
        }

        [Fact]
        public void allow_registration_later()
        {
            using (var uci = new UniversalChessInterface(new FakeEngineController()))
            {
                uci.SetUciMode();
                using (var registerCommand = uci.CommandFactory.CreateCommand<RegisterCommand>())
                {
                    registerCommand.SetRegistration(true);
                    uci.SendCommand(registerCommand);
                    uci.WaitForResponse(registerCommand);

                    if (!string.IsNullOrWhiteSpace(registerCommand.ErrorText))
                    {
                        Assert.False(registerCommand.ErrorText.StartsWith("Unknown command"));
                    }
                    Assert.True(registerCommand.CommandResponseReceived);
                }
            }
        }

        [Fact]
        public void throw_when_registration_code_invalid()
        {
            using (var uci = new UniversalChessInterface(new FakeEngineController()))
            {
                uci.SetUciMode();
                using (var registerCommand = uci.CommandFactory.CreateCommand<RegisterCommand>())
                {
                    Assert.Throws<ChessterEngineException>(() =>
                    {
                        registerCommand.SetRegistration(false, "Reese");
                    });
                }
            }
        }

        [Fact]
        public void throw_when_registration_name_invalid()
        {
            using (var uci = new UniversalChessInterface(new FakeEngineController()))
            {
                uci.SetUciMode();
                using (var registerCommand = uci.CommandFactory.CreateCommand<RegisterCommand>())
                {
                    Assert.Throws<ChessterEngineException>(() =>
                    {
                        registerCommand.SetRegistration(false, string.Empty, "8987JJ");
                    });
                }
            }
        }

        [Fact]
        public void support_copy_protected_engines()
        {
            var engineController = new FakeEngineController();
            engineController.IsCopyProtected = true;

            using (var uci = new UniversalChessInterface(engineController))
            {
                using(var uciCommand = uci.CommandFactory.CreateCommand<UciCommand>())
                {
                    uci.SendCommand(uciCommand);
                    Assert.Equal("copyprotection checking", uciCommand.CopyProtectionInfo);
                }
            }
        }

        [Fact]
        public void throw_when_command_response_period_invalid()
        {
            using (var uci = new UniversalChessInterface(new FakeEngineController()))
            {
                using (var registerCommand = uci.CommandFactory.CreateCommand<RegisterCommand>())
                {
                    Assert.Throws<ChessterEngineException>(() =>
                    {
                        registerCommand.CommandResponsePeriod = new TimeSpan(0, 0, 0, 0, 0);
                    });
                }
            }
        }
    }
}
