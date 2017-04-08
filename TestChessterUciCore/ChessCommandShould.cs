using ChessterUciCore.Commands;
using Xunit;
using ChessterUciCore;
using System;
using System.Threading;
using System.Diagnostics;
using TestChessterUciCore.Fakes;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace TestChessterUciCore
{
    public class ChessCommandShould 
    {
        private static ILogger Logger {get;} = TestUtility.LoggerFactory.CreateLogger<UniversalChessInterfaceShould>();

        public ChessCommandShould()
        {
            Logger.LogTrace($"ChessCommandShould(), ManagedThreadId is {Thread.CurrentThread.ManagedThreadId}");
        }

        [Fact]
        public async Task receive_readyok_after_sending_isreadyAsync()
        {
            Logger.LogInformation("ChessCommandShould.receive_readyok_after_sending_isready");

            using (var uci = new UniversalChessInterface(TestUtility.ChessEnginePath))
            {
                using (var isReadyCommand = uci.CreateCommand<IsReadyCommand>())
                {
                    await isReadyCommand.SendAsync();

                    Assert.True(isReadyCommand.CommandResponseReceived);
                }

            }
        }

        [Fact]
        public async Task error_when_engine_doesnt_recognize_command()
        {
            Logger.LogInformation("ChessCommandShould.error_when_engine_doesnt_recognize_command");
            
            using (var uci = new UniversalChessInterface(TestUtility.ChessEnginePath))
            {
                using (var bogusCommand = uci.CreateCommand<BogusCommand>())
                {
                    await bogusCommand.SendAsync();

                    Assert.True(bogusCommand.ErrorText.StartsWith("Unknown command"));
                }
            }
        }

        [Fact]
        public async Task allow_options_to_be_set()
        {
            Logger.LogInformation("ChessCommandShould.allow_options_to_be_set");

            using (var uci = new UniversalChessInterface(TestUtility.ChessEnginePath))
            {
                using(var optionCommand = uci.CreateCommand<OptionCommand>())
                {
                    var optionWriteDebugLog = uci.ChessEngineOptions["optionDebugLogFile"];
                    optionWriteDebugLog.Default = "true";
                    var optionContemptFactor = uci.ChessEngineOptions["optionContempt"];
                    optionContemptFactor.Default = "10";
                    optionCommand.OptionValues.Add(optionWriteDebugLog);
                    optionCommand.OptionValues.Add(optionContemptFactor);
                    await optionCommand.SendAsync();
                    using(var isReady = uci.CreateCommand<IsReadyCommand>())
                    {
                        await isReady.SendAsync();

                        Assert.True(isReady.CommandResponseReceived);
                    }

                    Assert.True(optionCommand.ErrorText == default(string));
                }
                
            }
        }

        [Fact]
        public void timeout_when_specified()
        {
            Logger.LogInformation("ChessCommandShould.timeout_when_specified");

            using (var uci = new UniversalChessInterface(TestUtility.ChessEnginePath))
            {
                using (var uciNewGame = uci.CreateCommand<UciNewGame>())
                {
                    uciNewGame.CommandResponsePeriod = new TimeSpan(0, 0, 0, 0, 25); // 25 milliseconds
                    Logger.LogInformation("timeout_when_specified: sending uciNewGame");
                    var task = uciNewGame.SendAsync();
                    Logger.LogInformation("timeout_when_specified: sleeping");
                    Thread.Sleep(50);

                    Assert.False(uciNewGame.CommandResponseReceived);
                    Assert.True(uciNewGame.CommandTimeoutElapsed);
                }
                
            }
        }

        [Fact]
        public async Task quit_to_end_program()
        {
            Logger.LogInformation("ChessCommandShould.quit_to_end_program");

            using (var uci = new UniversalChessInterface(TestUtility.ChessEnginePath))
            {
                using (var quitCommand = uci.CreateCommand<QuitCommand>())
                {
                    await quitCommand.SendAsync();

                    Assert.True(quitCommand.ErrorText == default(string));
                }
                
            }
        }

        [Fact]
        public async Task allow_the_engine_to_go()
        {
            Logger.LogInformation("ChessCommandShould.allow_the_engine_to_go");

            using (var uci = new UniversalChessInterface(TestUtility.ChessEnginePath))
            {
                using (var isReadyCommand = uci.CreateCommand<IsReadyCommand>())
                {
                    await isReadyCommand.SendAsync();

                    Assert.True(isReadyCommand.CommandResponseReceived);
                    using(var newGameCommand = uci.CreateCommand<UciNewGame>())
                    {
                        await newGameCommand.SendAsync();
                        await isReadyCommand.SendAsync();

                        using (var positionCommand = uci.CreateCommand<PositionCommand>())
                        {
                            positionCommand.IsStartPosition = true;
                            positionCommand.FenString = "e2e4 e7e5";
                            await positionCommand.SendAsync();

                            using (var goCommand = uci.CreateCommand<GoCommand>())
                            {
                                goCommand.Infinite = true;
                                await goCommand.SendAsync();
                                Thread.Sleep(500); // Allow the engine time to "think"
                                await uci.CreateCommand<StopCommand>().SendAsync();

                                Assert.True(!string.IsNullOrWhiteSpace(goCommand.InfoResponse.ToString()));
                                Debug.WriteLine(goCommand.InfoResponse.ToString());
                                Assert.True(!string.IsNullOrWhiteSpace(goCommand.BestMove));
                                Debug.WriteLine(goCommand.BestMove);

                                await uci.CreateCommand<QuitCommand>().SendAsync();
                            }
                        }
                    }
                }
                
            }
        }

        [Fact]
        public async Task support_registration()
        {
            Logger.LogInformation("ChessCommandShould.support_registration");
            
            using (var uci = new UniversalChessInterface(new FakeEngineController()))
            {
                using (var registerCommand = uci.CreateCommand<RegisterCommand>())
                {
                    registerCommand.SetRegistration(false, "Reese", "23098HHHJ");
                    await registerCommand.SendAsync();
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
        public async Task allow_registration_later()
        {
            Logger.LogInformation("ChessCommandShould.allow_registration_later");

            using (var uci = new UniversalChessInterface(new FakeEngineController()))
            {
                using (var registerCommand = uci.CreateCommand<RegisterCommand>())
                {
                    registerCommand.SetRegistration(true);
                    await registerCommand.SendAsync();
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
            Logger.LogInformation("ChessCommandShould.throw_when_registration_code_invalid");

            using (var uci = new UniversalChessInterface(new FakeEngineController()))
            {
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
            Logger.LogInformation("ChessCommandShould.throw_when_registration_name_invalid");

            using (var uci = new UniversalChessInterface(new FakeEngineController()))
            {
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
        public async Task support_copy_protected_engines()
        {
            Logger.LogInformation("ChessCommandShould.support_copy_protected_engines");

            var engineController = new FakeEngineController();
            engineController.IsCopyProtected = true;

            using (var uci = new UniversalChessInterface(engineController))
            {
                using(var uciCommand = uci.CreateCommand<UciCommand>())
                {
                    await uciCommand.SendAsync();
                    Assert.Equal("copyprotection checking", uciCommand.CopyProtectionInfo);
                }
            }
        }

        [Fact]
        public void throw_when_command_response_period_invalid()
        {
            Logger.LogInformation("ChessCommandShould.throw_when_command_response_period_invalid");

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
