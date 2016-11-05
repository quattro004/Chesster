using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessterUciCore.Commands
{
    /// <summary>
    /// Factory implementation used to create chess commands.
    /// </summary>
    public class ChessCommandFactory : IChessCommandFactory
    {
        private IEngineController _chessEngineController;

        /// <summary>
        /// Performs initialization for the factory.
        /// </summary>
        /// <param name="engineController">Instance of <see cref="IEngineController"/> which will be
        /// associated with a command during creation so the command can receive messages from the 
        /// chess engine.</param>
        public ChessCommandFactory(IEngineController engineController)
        {
            _chessEngineController = engineController;
        }

        /// <summary>
        /// Creates a <see cref="ChessCommand"/> of the type specified.
        /// </summary>
        /// <typeparam name="T">Type of chess command to create.</typeparam>
        /// <returns>Instance of <see cref="ChessCommand"/> using the specified type.</returns>
        public T CreateCommand<T>() where T : ChessCommand, new()
        {
            var command = new T();
            command.ChessEngineController = _chessEngineController;

            return command;
        }
    }
}
