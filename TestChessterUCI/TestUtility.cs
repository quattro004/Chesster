using ChessterUci;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestChessterUCI
{
    public static class TestUtility
    {
        public static UniversalChessInterface UciObject { get; private set; }

        public static async Task PrepareUniversalChessInterface()
        {
            UciObject = new UniversalChessInterface(ConfigurationManager.AppSettings["ChessEnginePath"]);
            await UciObject.SetUciMode();
        }
    }
}
