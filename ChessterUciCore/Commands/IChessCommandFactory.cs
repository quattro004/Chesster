using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessterUciCore.Commands
{
    /// <summary>
    /// Defines the factory for creating chess commands.
    /// </summary>
    public interface IChessCommandFactory
    {
        /// <summary>
        /// Creates a chess command.
        /// </summary>
        /// <returns>Chess command of the specified type.</returns>
        T CreateCommand<T>() where T : ChessCommand, new();
    }
}
