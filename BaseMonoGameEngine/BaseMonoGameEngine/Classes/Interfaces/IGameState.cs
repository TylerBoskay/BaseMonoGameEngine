using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// An interface for a game state.
    /// </summary>
    public interface IGameState : IFiniteStateMachine
    {
        /// <summary>
        /// Renders the game state.
        /// </summary>
        void Render();
    }
}
