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
        private static readonly Dictionary<int, InputHandler> InputHandlers = new Dictionary<int, InputHandler>(1 + InputGlobals.MaxGamePadCount);

        static Input()
        {
            InputHandler inputHandler = InputHandler.CreateWithDefaultMappings(InputHandler.InputTypes.Keyboard, 0, 0);
            InputHandlers.Add(0, inputHandler);

            //Add input handlers for all possible gamepads
            int maxIndex = InputGlobals.GamePadInputIndex + InputGlobals.MaxGamePadCount;
            for (int i = InputGlobals.GamePadInputIndex; i < maxIndex; i++)
            {
                InputHandler inputHandlerGamePad = InputHandler.CreateWithDefaultMappings(InputHandler.InputTypes.GamePad, i, i - InputGlobals.GamePadInputIndex);
                InputHandlers.Add(i, inputHandlerGamePad);
            }
        }

        /// <summary>
        /// Sets the default mapping of an input for a player.
        /// </summary>
        /// <param name="inputIndex">The index of the player.</param>
        /// <param name="inputType">The input type.</param>
        public static void SetDefaultMapping(in int inputIndex, in InputHandler.InputTypes inputType)
        {
            if (InputHandlers.TryGetValue(inputIndex, out InputHandler handler) == false)
            {
                return;
            }

            handler.SetDefaultMappings(inputType);
        }

        /// <summary>
        /// Returns the InputHandler at the given input index if one is found, otherwise null.
        /// </summary>
        /// <param name="inputIndex">The input index.</param>
        /// <returns>The InputHandler at <paramref name="inputIndex"/> if found, otherwise null.</returns>
        public static InputHandler GetInputHandler(in int inputIndex)
        {
            InputHandlers.TryGetValue(inputIndex, out InputHandler handler);

            return handler;
        }

        /// <summary>
        /// Gets the axis value for a player.
        /// </summary>
        /// <param name="inputIndex">The index of the player.</param>
        /// <param name="action">The action to get the axis value for.</param>
        /// <returns>A float from -1 to 1 representing the axis value.</returns>
        public static float GetAxis(in int inputIndex, in string action)
        {
            if (InputHandlers.TryGetValue(inputIndex, out InputHandler handler) == false)
                return 0f;

            return handler.GetAxis(action);
        }

        /// <summary>
        /// Tells whether a button is pressed for a player.
        /// </summary>
        /// <param name="inputIndex">The index of the player.</param>
        /// <param name="action">The action to get the axis value for.</param>
        /// <returns>true if the button is pressed, otherwise false.</returns>
        public static bool GetButton(in int inputIndex, in string action)
        {
            if (InputHandlers.TryGetValue(inputIndex, out InputHandler handler) == false)
                return false;

            return handler.GetButton(action);
        }

        /// <summary>
        /// Tells whether a button was just pressed for a player.
        /// </summary>
        /// <param name="inputIndex">The index of the player.</param>
        /// <param name="action">The action to get the axis value for.</param>
        /// <returns>true if the button was just pressed, otherwise false.</returns>
        public static bool GetButtonDown(in int inputIndex, in string action)
        {
            if (InputHandlers.TryGetValue(inputIndex, out InputHandler handler) == false)
                return false;

            return handler.GetButtonDown(action);
        }

        /// <summary>
        /// Tells whether a button was just released for a player.
        /// </summary>
        /// <param name="inputIndex">The index of the player.</param>
        /// <param name="action">The action to get the axis value for.</param>
        /// <returns>true if the button was just released, otherwise false.</returns>
        public static bool GetButtonUp(in int inputIndex, in string action)
        {
            if (InputHandlers.TryGetValue(inputIndex, out InputHandler handler) == false)
                return false;

            return handler.GetButtonUp(action);
        }

        /// <summary>
        /// Tells whether a button is pressed for any player.
        /// </summary>
        /// <param name="action">The action to get the axis value for.</param>
        /// <returns>true if the button is pressed, otherwise false.</returns>
        public static bool GetButton(in string action)
        {
            int count = InputHandlers.Keys.Count;

            for (int i = 0; i < count; i++)
            {
                if (GetButton(i, action) == true)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tells whether a button was just pressed for any player.
        /// </summary>
        /// <param name="action">The action to get the axis value for.</param>
        /// <returns>true if the button was just pressed, otherwise false.</returns>
        public static bool GetButtonDown(in string action)
        {
            int count = InputHandlers.Keys.Count;

            for (int i = 0; i < count; i++)
            {
                if (GetButtonDown(i, action) == true)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tells whether a button was just released for any player.
        /// </summary>
        /// <param name="action">The action to get the axis value for.</param>
        /// <returns>true if the button was just released, otherwise false.</returns>
        public static bool GetButtonUp(in string action)
        {
            int count = InputHandlers.Keys.Count;

            for (int i = 0; i < count; i++)
            {
                if (GetButtonUp(i, action) == true)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Resets the input state for a player.
        /// </summary>
        /// <param name="playerIndex">The index of the player to reset.</param>
        public static void ClearInputState(in int playerIndex)
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
                inputs.Value.UpdateInputState();
            }
        }
    }
}
