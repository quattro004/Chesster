using ChessterUci.Commands;
using Xunit;
using ChessterUci;
using System.Configuration;

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
    }
}
