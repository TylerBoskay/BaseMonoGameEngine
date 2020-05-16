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
        public static bool GetKey(in Keys key)
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
        public static bool GetKey(in Keys key, KeyboardState keyboardState)
        {
            return keyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Tells if a key on the keyboard was just released.
        /// </summary>
        /// <param name="key">The key to test.</param>
        /// <returns>true if the key was just released, otherwise false.</returns>
        public static bool GetKeyUp(in Keys key)
        {
            return GetKeyUp(key, InputKeyboard);
        }

        /// <summary>
        /// Tells if a key on a KeyboardState was just released.
        /// </summary>
        /// <param name="key">The key to test.</param>
        /// <param name="keyboardState">The KeyboardState to check.</param>
        /// <returns>true if the key on the KeyboardState was just released, otherwise false.</returns>
        public static bool GetKeyUp(in Keys key, KeyboardState keyboardState)
        {
            return (keyboardState.IsKeyDown(key) == true && KBState.IsKeyUp(key) == true);
        }

        /// <summary>
        /// Tells if a key on a KeyboardState was just released.
        /// </summary>
        /// <param name="key">The key to test.</param>
        /// <param name="keyboardState">The KeyboardState to check.</param>
        /// <param name="checkKBState">The KeyboardState to compare with. This is often the most up-to-date one.</param>
        /// <returns>true if the key on the KeyboardState was just released, otherwise false.</returns>
        public static bool GetKeyUp(in Keys key, KeyboardState keyboardState, KeyboardState checkKBState)
        {
            return (keyboardState.IsKeyDown(key) == true && checkKBState.IsKeyUp(key) == true);
        }

        /// <summary>
        /// Gets if a key on the keyboard was just pressed.
        /// </summary>
        /// <param name="key">The key to test.</param>
        /// <returns>true if the key on the keyboard was just pressed, otherwise false.</returns>
        public static bool GetKeyDown(in Keys key)
        {
            return GetKeyDown(key, InputKeyboard);
        }

        /// <summary>
        /// Gets if a key on a KeyboardState was just pressed.
        /// </summary>
        /// <param name="key">The key to test.</param>
        /// <param name="keyboardState">The KeyboardState to check.</param>
        /// <returns>true if the key on the KeyboardState was just pressed, otherwise false.</returns>
        public static bool GetKeyDown(in Keys key, KeyboardState keyboardState)
        {
            return (keyboardState.IsKeyUp(key) == true && KBState.IsKeyDown(key) == true);
        }

        /// <summary>
        /// Tells if a key on a KeyboardState was just pressed.
        /// </summary>
        /// <param name="key">The key to test.</param>
        /// <param name="keyboardState">The KeyboardState to check.</param>
        /// <param name="checkKBState">The KeyboardState to compare with. This is often the most up-to-date one.</param>
        /// <returns>true if the key on the KeyboardState was just pressed, otherwise false.</returns>
        public static bool GetKeyDown(in Keys key, KeyboardState keyboardState, KeyboardState checkKBState)
        {
            return (keyboardState.IsKeyUp(key) == true && checkKBState.IsKeyDown(key) == true);
        }

        /// <summary>
        /// Finds and returns the first key on the keyboard that was just pressed in a set of keys.
        /// </summary>
        /// <param name="keys">The set of keys to test.</param>
        /// <returns>The key in the set that was pressed, otherwise <see cref="Keys.None"/>.</returns>
        public static Keys GetKeyDownInRange(Keys[] keys)
        {
            if (keys == null) return Keys.None;

            for (int i = 0; i < keys.Length; i++)
            {
                if (GetKeyDown(keys[i]) == true) return keys[i];
            }

            return Keys.None;
        }

        /// <summary>
        /// Clears the keyboard input state.
        /// </summary>
        public static void ClearKeyboardState()
        {
            InputKeyboard = new KeyboardState(null);
        }

        /// <summary>
        /// Clears the keyboard input state.
        /// </summary>
        /// <param name="keyboardState">The KeyboardState to clear.</param>
        public static void ClearKeyboardState(ref KeyboardState keyboardState)
        {
            keyboardState = new KeyboardState(null);
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
