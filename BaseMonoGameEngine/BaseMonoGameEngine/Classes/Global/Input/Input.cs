using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// A static class for managing inputs.
    /// This helps read input from either a keyboard or joystick for multiple players by utilitizing multiple <see cref="InputHandler"/>s.
    /// </summary>
    public static class Input
    {
        /// <summary>
        /// The inputs handlers. The key is the player index, and the value is an input handler.
        /// </summary>
        private static readonly Dictionary<int, InputHandler> InputHandlers = new Dictionary<int, InputHandler>();

        static Input()
        {
            InputHandlers.Add(0, InputHandler.CreateWithDefaultMappings(InputHandler.InputTypes.Keyboard));
        }

        /// <summary>
        /// Sets the default mapping of an input for a player.
        /// </summary>
        /// <param name="playerIndex">The index of the player.</param>
        /// <param name="inputType">The input type.</param>
        public static void SetDefaultMapping(int playerIndex, InputHandler.InputTypes inputType)
        {
            if (InputHandlers.ContainsKey(playerIndex) == false)
            {
                return;
            }

            InputHandlers[playerIndex].SetDefaultMappings(inputType);
        }

        /// <summary>
        /// Gets the axis value for a player.
        /// </summary>
        /// <param name="playerIndex">The index of the player.</param>
        /// <param name="action">The action to get the axis value for.</param>
        /// <returns>A float from -1 to 1 representing the axis value.</returns>
        public static float GetAxis(int playerIndex, string action)
        {
            if (InputHandlers.ContainsKey(playerIndex) == false)
                return 0f;

            return InputHandlers[playerIndex].GetAxis(playerIndex, action);
        }

        /// <summary>
        /// Tells whether a button is pressed for a player.
        /// </summary>
        /// <param name="playerIndex">The index of the player.</param>
        /// <param name="action">The action to get the axis value for.</param>
        /// <returns>true if the button is pressed, otherwise false.</returns>
        public static bool GetButton(int playerIndex, string action)
        {
            if (InputHandlers.ContainsKey(playerIndex) == false)
                return false;

            return InputHandlers[playerIndex].GetButton(playerIndex, action);
        }

        /// <summary>
        /// Tells whether a button was just pressed for a player.
        /// </summary>
        /// <param name="playerIndex">The index of the player.</param>
        /// <param name="action">The action to get the axis value for.</param>
        /// <returns>true if the button was just pressed, otherwise false.</returns>
        public static bool GetButtonDown(int playerIndex, string action)
        {
            if (InputHandlers.ContainsKey(playerIndex) == false)
                return false;

            return InputHandlers[playerIndex].GetButtonDown(playerIndex, action);
        }

        /// <summary>
        /// Tells whether a button was just released for a player.
        /// </summary>
        /// <param name="playerIndex">The index of the player.</param>
        /// <param name="action">The action to get the axis value for.</param>
        /// <returns>true if the button was just released, otherwise false.</returns>
        public static bool GetButtonUp(int playerIndex, string action)
        {
            if (InputHandlers.ContainsKey(playerIndex) == false)
                return false;

            return InputHandlers[playerIndex].GetButtonUp(playerIndex, action);
        }

        /// <summary>
        /// Resets the input state for a player.
        /// </summary>
        /// <param name="playerIndex">The index of the player to reset.</param>
        public static void ClearInputState(int playerIndex)
        {
            if (InputHandlers.ContainsKey(playerIndex) == false)
                return;

            InputHandlers[playerIndex].ClearInputState();
        }

        /// <summary>
        /// Updates all the input states for all players.
        /// </summary>
        public static void UpdateInput()
        {
            foreach (KeyValuePair<int, InputHandler> inputs in InputHandlers)
            {
                inputs.Value.UpdateInputState(inputs.Key);
            }
        }
    }
}
