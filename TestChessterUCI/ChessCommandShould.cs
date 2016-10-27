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
        public void receive_readyok_after_sending_isready()
        {
            try
            {
                TestUtility.PrepareUniversalChessInterface();

                using (var isReadyCommand = new IsReadyCommand())
                {
                    TestUtility.UciObject.SendCommand(isReadyCommand);
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
        public void error_when_engine_doesnt_recognize_command()
        {
            try
            {
                TestUtility.PrepareUniversalChessInterface();
                using (var bogusCommand = new BogusCommand())
                {
                    TestUtility.UciObject.SendCommand(bogusCommand);
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
