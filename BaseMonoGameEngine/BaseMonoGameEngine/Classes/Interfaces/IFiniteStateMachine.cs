using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// An interface for finite state machines.
    /// </summary>
    public interface IFiniteStateMachine
    {
        /// <summary>
        /// What happens when entering the state.
        /// </summary>
        void Enter();

        /// <summary>
        /// What happens when exiting the state.
        /// </summary>
        void Exit();

        /// <summary>
        /// Updates the state.
        /// </summary>
        void Update();
    }
}
