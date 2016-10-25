using ChessterUci.Commands;
using Xunit;
using System;
using System.Threading.Tasks;
using ChessterUci;

namespace TestChessterUCI
{
    public class ChessCommandShould
    {
        [Fact]
        public async Task receive_readyok_after_sending_isready()
        {
            try
            {
                await TestUtility.PrepareUniversalChessInterface();

                using (var isReadyCommand = new IsReadyCommand())
                {
                    await TestUtility.UciObject.SendCommand(isReadyCommand);
                    UniversalChessInterface.WaitForResponse(isReadyCommand);

                    Assert.True(isReadyCommand.CommandResponseReceived);
                }
            }
            finally
            {
                TestUtility.UciObject.Dispose();
            } 
        }

        [Fact]
        public async Task error_when_engine_doesnt_recognize_command()
        {
            try
            {
                await TestUtility.PrepareUniversalChessInterface();
                using (var bogusCommand = new BogusCommand())
                {
                    await TestUtility.UciObject.SendCommand(bogusCommand);
                    UniversalChessInterface.WaitForResponse(bogusCommand);

                    Assert.True(bogusCommand.ErrorText.StartsWith("Unknown command"));
                }
            }
            finally
            {
                TestUtility.UciObject.Dispose();
            }
        }
    }
}
