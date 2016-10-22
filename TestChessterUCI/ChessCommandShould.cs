using ChessterUci;
using ChessterUci.Commands;
using System.Configuration;
using Xunit;

namespace TestChessterUCI
{
    public class ChessCommandShould
    {
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

        [Fact]
        public async void allow_debug_mode_to_be_toggled()
        {
            using (var uci = new UniversalChessInterface(ConfigurationManager.AppSettings["ChessEnginePath"]))
            {
                await uci.InitializeEngine();
                using (var debugCommand = new DebugCommand(uci.ChessEngineController))
                {
                    Assert.DoesNotThrow(async () =>
                    {
                        debugCommand.DebugModeOn = true;
                        await debugCommand.SendCommand();
                        debugCommand.DebugModeOn = false;
                        await debugCommand.SendCommand();
                    });
                }
            }
        }
    }
}
