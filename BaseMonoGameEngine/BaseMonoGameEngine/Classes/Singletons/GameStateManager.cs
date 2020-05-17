using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// Manages game states.
    /// <para>This is a Singleton.</para>
    /// </summary>
    public sealed class GameStateManager : ICleanup, IUpdateable
    {
        #region Singleton Fields

        public static GameStateManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new GameStateManager();

                return instance;
            }
        }

        public static bool HasInstance => (instance != null);

        private static GameStateManager instance = null;

        #endregion

        public IGameState CurrentState { get; private set; } = null;

        private GameStateManager()
        {

        }

        /// <summary>
        /// Changes the game state.
        /// </summary>
        /// <param name="newState">The new game state.</param>
        public void ChangeGameState(IGameState newState)
        {
            //Exit the old state and enter the new one
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }

        public void CleanUp()
        {
            CurrentState?.Exit();
            CurrentState = null;

            instance = null;
        }

        public void Update()
        {
            CurrentState.Update();
        }

        public void Render()
        {
            CurrentState.Render();
        }
    }
}
