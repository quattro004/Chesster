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
        [Fact]
        public void receive_readyok_after_sending_isready()
        {
            Console.WriteLine("\tChessCommandShould.receive_readyok_after_sending_isready");

            using (var uci = new UniversalChessInterface())
            {
                uci.SetUciMode();
                using (var isReadyCommand = uci.CreateCommand<IsReadyCommand>())
                {
                    isReadyCommand.Send();
                    uci.WaitForResponse(isReadyCommand);

                    Assert.True(isReadyCommand.CommandResponseReceived);
                }
                
            }
        }

        [Fact]
        public void error_when_engine_doesnt_recognize_command()
        {
            Console.WriteLine("\tChessCommandShould.error_when_engine_doesnt_recognize_command");
            
            using (var uci = new UniversalChessInterface())
            {
                uci.SetUciMode();
                using (var bogusCommand = uci.CreateCommand<BogusCommand>())
                {
                    bogusCommand.Send();
                    uci.WaitForResponse(bogusCommand);

                    Assert.True(bogusCommand.ErrorText.StartsWith("Unknown command"));
                }
            }
        }

        [Fact]
        public void allow_options_to_be_set()
        {
            Console.WriteLine("\tChessCommandShould.allow_options_to_be_set");

            using (var uci = new UniversalChessInterface())
            {
                uci.SetUciMode();
                using(var optionCommand = uci.CreateCommand<OptionCommand>())
                {
                    var optionWriteDebugLog = uci.ChessEngineOptions["optionDebugLogFile"];
                    optionWriteDebugLog.Default = "true";
                    var optionContemptFactor = uci.ChessEngineOptions["optionContempt"];
                    optionContemptFactor.Default = "10";
                    optionCommand.OptionValues.Add(optionWriteDebugLog);
                    optionCommand.OptionValues.Add(optionContemptFactor);
                    optionCommand.Send();
                    using(var isReady = uci.CreateCommand<IsReadyCommand>())
                    {
                        isReady.Send();
                        uci.WaitForResponse(isReady);

                        Assert.True(isReady.CommandResponseReceived);
                    }

                    Assert.True(optionCommand.ErrorText == default(string));
                }
                
            }
        }

        [Fact]
        public void timeout_when_specified()
        {
            Console.WriteLine("\tChessCommandShould.timeout_when_specified");

            using (var uci = new UniversalChessInterface())
            {
                using (var uciCommand = uci.CreateCommand<UciCommand>())
                {
                    uciCommand.CommandResponsePeriod = new TimeSpan(0, 0, 0, 0, 25); // 25 milliseconds
                    uciCommand.Send();
                    Thread.Sleep(50);

                    Assert.False(uciCommand.CommandResponseReceived);
                    Assert.True(uciCommand.CommandTimeoutElapsed);
                }
                
            }
        }

        [Fact]
        public void quit_to_end_program()
        {
            Console.WriteLine("\tChessCommandShould.quit_to_end_program");

            using (var uci = new UniversalChessInterface())
            {
                uci.SetUciMode();
                using (var quitCommand = uci.CreateCommand<QuitCommand>())
                {
                    quitCommand.Send();

                    Assert.True(quitCommand.ErrorText == default(string));
                }
                
            }
        }

        [Fact]
        public void allow_the_engine_to_go()
        {
            Console.WriteLine("\tChessCommandShould.allow_the_engine_to_go");

            using (var uci = new UniversalChessInterface())
            {
                uci.SetUciMode();
                using (var isReadyCommand = uci.CreateCommand<IsReadyCommand>())
                {
                    isReadyCommand.Send();
                    uci.WaitForResponse(isReadyCommand);

                    Assert.True(isReadyCommand.CommandResponseReceived);
                    using(var newGameCommand = uci.CreateCommand<UciNewGame>())
                    {
                        newGameCommand.Send();
                        isReadyCommand.Send();
                        uci.WaitForResponse(isReadyCommand);

                        using (var positionCommand = uci.CreateCommand<PositionCommand>())
                        {
                            positionCommand.IsStartPosition = true;
                            positionCommand.FenString = "e2e4 e7e5";
                            positionCommand.Send();

                            using (var goCommand = uci.CreateCommand<GoCommand>())
                            {
                                goCommand.Infinite = true;
                                goCommand.Send();
                                Thread.Sleep(500); // Allow the engine time to "think"
                                uci.CreateCommand<StopCommand>().Send();
                                uci.WaitForResponse(goCommand);

                                Assert.True(!string.IsNullOrWhiteSpace(goCommand.InfoResponse.ToString()));
                                Debug.WriteLine(goCommand.InfoResponse.ToString());
                                Assert.True(!string.IsNullOrWhiteSpace(goCommand.BestMove));
                                Debug.WriteLine(goCommand.BestMove);

                                uci.CreateCommand<QuitCommand>().Send();
                            }
                        }
                    }
                }
                
            }
        }

        [Fact]
        public void support_registration()
        {
            Console.WriteLine("\tChessCommandShould.support_registration");
            
            using (var uci = new UniversalChessInterface(new FakeEngineController()))
            {
                uci.SetUciMode();
                using (var registerCommand = uci.CreateCommand<RegisterCommand>())
                {
                    registerCommand.SetRegistration(false, "Reese", "23098HHHJ");
                    registerCommand.Send();
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
            Console.WriteLine("\tChessCommandShould.allow_registration_later");

            using (var uci = new UniversalChessInterface(new FakeEngineController()))
            {
                uci.SetUciMode();
                using (var registerCommand = uci.CreateCommand<RegisterCommand>())
                {
                    registerCommand.SetRegistration(true);
                    registerCommand.Send();
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
            Console.WriteLine("\tChessCommandShould.throw_when_registration_code_invalid");

            using (var uci = new UniversalChessInterface(new FakeEngineController()))
            {
                uci.SetUciMode();
                using (var registerCommand = uci.CreateCommand<RegisterCommand>())
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
            Console.WriteLine("\tChessCommandShould.throw_when_registration_name_invalid");

            using (var uci = new UniversalChessInterface(new FakeEngineController()))
            {
                uci.SetUciMode();
                using (var registerCommand = uci.CreateCommand<RegisterCommand>())
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
            Console.WriteLine("\tChessCommandShould.support_copy_protected_engines");

            var engineController = new FakeEngineController();
            engineController.IsCopyProtected = true;

            using (var uci = new UniversalChessInterface(engineController))
            {
                using(var uciCommand = uci.CreateCommand<UciCommand>())
                {
                    uciCommand.Send();
                    Assert.Equal("copyprotection checking", uciCommand.CopyProtectionInfo);
                }
            }
        }

        [Fact]
        public void throw_when_command_response_period_invalid()
        {
            Console.WriteLine("\tChessCommandShould.throw_when_command_response_period_invalid");

            using (var uci = new UniversalChessInterface(new FakeEngineController()))
            {
                using (var registerCommand = uci.CreateCommand<RegisterCommand>())
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
