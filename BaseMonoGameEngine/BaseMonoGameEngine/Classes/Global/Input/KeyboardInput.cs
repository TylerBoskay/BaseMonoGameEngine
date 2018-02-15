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
    /// A wrapper for obtaining keyboard input.
    /// </summary>
    public static class KeyboardInput
    {
        private static KeyboardState InputKeyboard = default(KeyboardState);

        private static KeyboardState KBState => Keyboard.GetState();

        /// <summary>
        /// Tells if a key on the keyboard is pressed.
        /// </summary>
        /// <param name="key">The key to test.</param>
        /// <returns>true if the key is pressed, otherwise false.</returns>
        public static bool GetKey(Keys key)
        {
            KeyboardState kbState = KBState;
            return GetKey(key, kbState);
        }

        /// <summary>
        /// Tells if a key on a KeyboardState is pressed.
        /// </summary>
        /// <param name="key">The key to test.</param>
        /// <param name="keyboardState">The KeyboardState to check.</param>
        /// <returns>true if the key is pressed on the KeyboardState, otherwise false.</returns>
        public static bool GetKey(Keys key, in KeyboardState keyboardState)
        {
            return keyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Tells if a key on the keyboard was just released.
        /// </summary>
        /// <param name="key">The key to test.</param>
        /// <returns>true if the key was just released, otherwise false.</returns>
        public static bool GetKeyUp(Keys key)
        {
            return GetKeyUp(key, InputKeyboard);
        }

        /// <summary>
        /// Tells if a key on a KeyboardState was just released.
        /// </summary>
        /// <param name="key">The key to test.</param>
        /// <param name="keyboardState">The KeyboardState to check.</param>
        /// <returns>true if the key on the KeyboardState was just released, otherwise false.</returns>
        public static bool GetKeyUp(Keys key, in KeyboardState keyboardState)
        {
            return (keyboardState.IsKeyDown(key) == true && KBState.IsKeyUp(key) == true);
        }

        /// <summary>
        /// Gets if a key on the keyboard was just pressed.
        /// </summary>
        /// <param name="key">The key to test.</param>
        /// <returns>true if the key on the keyboard was just pressed, otherwise false.</returns>
        public static bool GetKeyDown(Keys key)
        {
            return GetKeyDown(key, InputKeyboard);
        }

        /// <summary>
        /// Gets if a key on a KeyboardState was just pressed.
        /// </summary>
        /// <param name="key">The key to test.</param>
        /// <param name="keyboardState">The KeyboardState to check.</param>
        /// <returns>true if the key on the KeyboardState was just pressed, otherwise false.</returns>
        public static bool GetKeyDown(Keys key, in KeyboardState keyboardState)
        {
            return (keyboardState.IsKeyUp(key) == true && KBState.IsKeyDown(key) == true);
        }

        /// <summary>
        /// Clears the keyboard input state.
        /// </summary>
        /// <param name="keys">An optional array of Keys to be marked as pressed initially.</param>
        public static void ClearKeyboardState(params Keys[] keys)
        {
            InputKeyboard = new KeyboardState(keys);
        }

        /// <summary>
        /// Clears the keyboard input state.
        /// </summary>
        /// <param name="keyboardState">The KeyboardState to clear.</param>
        /// <param name="keys">An optional array of Keys to be marked as pressed initially.</param>
        public static void ClearKeyboardState(ref KeyboardState keyboardState, params Keys[] keys)
        {
            keyboardState = new KeyboardState(keys);
        }

        /// <summary>
        /// Updates the keyboard.
        /// </summary>
        public static void UpdateKeyboardState()
        {
            UpdateKeyboardState(ref InputKeyboard);
        }

        /// <summary>
        /// Updates a KeyboardState.
        /// </summary>
        /// <param name="keyboardState">The KeyboardState to update.</param>
        public static void UpdateKeyboardState(ref KeyboardState keyboardState)
        {
            keyboardState = KBState;
        }
    }
}
