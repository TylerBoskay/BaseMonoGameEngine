using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDMonoGameEngine
{
    /// <summary>
    /// The base state machine for the player.
    /// </summary>
    public abstract class PlayerStateMachine : IFiniteStateMachine
    {
        public Player PlayerRef = null;

        public PlayerStateMachine(Player playerRef)
        {
            PlayerRef = playerRef;
        }

        /// <summary>
        /// What happens when entering the state.
        /// </summary>
        public abstract void Enter();

        /// <summary>
        /// What happens when exiting the state.
        /// </summary>
        public abstract void Exit();

        public abstract void HandleInput();

        /// <summary>
        /// Updates the state.
        /// </summary>
        public abstract void Update();
    }
}
