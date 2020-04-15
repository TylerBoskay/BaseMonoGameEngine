using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TDMonoGameEngine
{
    /// <summary>
    /// A wrapper for obtaining GamePad input.
    /// </summary>
    public static class GamePadInput
    {
        /// <summary>
        /// A global gamepad state updated to use as a reference for checking if buttons were just pressed or released.
        /// </summary>
        private static GamePadState GlobalGPState = GamePadState.Default;

        /// <summary>
        /// Returns if a GamePad is connected.
        /// </summary>
        /// <param name="index">The index of the GamePad to use.</param>
        /// <returns>true if the GamePad is currently connected, otherwise false.</returns>
        public static bool IsGamePadConnected(in int index)
        {
            GetGamePadState(index, ref GlobalGPState);

            return GlobalGPState.IsConnected;
        }

        /// <summary>
        /// Returns true if a GamePad was connected this frame.
        /// </summary>
        /// <param name="index">The index of the GamePad to use.</param>
        /// <param name="curGPState">The GamePadState to compare with.</param>
        /// <returns>true if the GamePad at <paramref name="index"/> was connected this frame, otherwise false.</returns>
        public static bool HasGamePadJustConnected(in int index, GamePadState curGPState)
        {
            GetGamePadState(index, ref GlobalGPState);

            return (curGPState.IsConnected == false && GlobalGPState.IsConnected == true);
        }

        /// <summary>
        /// Returns true if a GamePad was disconnected this frame.
        /// </summary>
        /// <param name="index">The index of the GamePad to use.</param>
        /// <param name="curGPState">The GamePadState to compare with.</param>
        /// <returns>true if the GamePad at <paramref name="index"/> was disconnected this frame, otherwise false.</returns>
        public static bool HasGamePadJustDisconnected(in int index, GamePadState curGPState)
        {
            GetGamePadState(index, ref GlobalGPState);

            return (curGPState.IsConnected == true && GlobalGPState.IsConnected == false);
        }

        /// <summary>
        /// Gets the current GamePadState of a GamePad at a particular index.
        /// </summary>
        /// <param name="index">The index of the GamePad.</param>
        /// <returns>A GamePadState.</returns>
        public static GamePadState GetGamePadState(in int index)
        {
            return GamePad.GetState(index);
        }

        /// <summary>
        /// Updates a GamePadState.
        /// </summary>
        /// <param name="index">The index of the GamePad to use.</param>
        /// <param name="gpState">The GamePadState to update.</param>
        public static void GetGamePadState(in int index, ref GamePadState gpState)
        {
            gpState = GamePad.GetState(index);
        }

        /// <summary>
        /// Gets the left stick axis value for the GamePad.
        /// </summary>
        /// <param name="index">The index of the GamePad.</param>
        /// <returns>A Vector2 containing the left stick value from -1 to 1.</returns>
        public static Vector2 GetLeftStickValue(in int index)
        {
            GetGamePadState(index, ref GlobalGPState);

            return GlobalGPState.ThumbSticks.Left;
        }

        /// <summary>
        /// Gets the right stick axis value for the GamePad.
        /// </summary>
        /// <param name="index">The index of the GamePad.</param>
        /// <returns>A Vector2 containing the right stick value from -1 to 1.</returns>
        public static Vector2 GetRightStickValue(in int index)
        {
            GetGamePadState(index, ref GlobalGPState);

            return GlobalGPState.ThumbSticks.Right;
        }

        /// <summary>
        /// Tells whether a button on a GamePad at an index is pressed.
        /// </summary>
        /// <param name="index">The index of the GamePad.</param>
        /// <param name="button">The button.</param>
        /// <returns>true if the button is pressed, otherwise false.</returns>
        public static bool GetButton(in int index, in Buttons button)
        {
            GetGamePadState(index, ref GlobalGPState);

            return (GlobalGPState.IsButtonDown(button) == true);
        }

        /// <summary>
        /// Tells whether a button on the GamePad is pressed.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="gpState">The GamePadState to check.</param>
        /// <returns>true if the button is pressed, otherwise false.</returns>
        public static bool GetButton(in Buttons button, GamePadState gpState)
        {
            return (gpState.IsButtonDown(button) == true);
        }

        /// <summary>
        /// Tells whether a button on the GamePad was just pressed.
        /// </summary>
        /// <param name="index">The index of the GamePad.</param>
        /// <param name="button">The button.</param>
        /// <param name="gpState">The GamePadState to check.</param>
        /// <returns>true if the button was just pressed, otherwise false.</returns>
        public static bool GetButtonDown(in int index, in Buttons button, GamePadState gpState)
        {
            GetGamePadState(index, ref GlobalGPState);

            return (gpState.IsButtonDown(button) == false && GlobalGPState.IsButtonDown(button) == true);
        }

        /// <summary>
        /// Tells whether a button on the GamePad was just pressed.
        /// </summary>
        /// <param name="index">The index of the GamePad.</param>
        /// <param name="button">The button.</param>
        /// <param name="gpState">The GamePadState to check.</param>
        /// <param name="checkGPState">The GamePadState to compare with. This is often the most up-to-date one.</param>
        /// <returns>true if the button was just pressed, otherwise false.</returns>
        public static bool GetButtonDown(in int index, in Buttons button, GamePadState gpState, GamePadState checkGPState)
        {
            return (gpState.IsButtonDown(button) == false && checkGPState.IsButtonDown(button) == true);
        }

        /// <summary>
        /// Tells whether a button on the GamePad was just released.
        /// </summary>
        /// <param name="index">The index of the GamePad.</param>
        /// <param name="button">The button.</param>
        /// <param name="gpState">The GamePadState to check.</param>
        /// <returns>true if the button was just released, otherwise false.</returns>
        public static bool GetButtonUp(in int index, in Buttons button, GamePadState gpState)
        {
            GetGamePadState(index, ref GlobalGPState);

            return (gpState.IsButtonDown(button) == true && GlobalGPState.IsButtonDown(button) == false);
        }

        /// <summary>
        /// Tells whether a button on the GamePad was just released.
        /// </summary>
        /// <param name="index">The index of the GamePad.</param>
        /// <param name="button">The button.</param>
        /// <param name="gpState">The GamePadState to check.</param>
        /// <param name="checkGPState">The GamePadState to compare with. This is often the most up-to-date one.</param>
        /// <returns>true if the button was just released, otherwise false.</returns>
        public static bool GetButtonUp(in int index, in Buttons button, GamePadState gpState, GamePadState checkGPState)
        {
            return (gpState.IsButtonDown(button) == true && checkGPState.IsButtonDown(button) == false);
        }
    }
}
