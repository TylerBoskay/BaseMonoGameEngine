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
    /// A wrapper for obtaining Joystick input.
    /// </summary>
    public static class JoystickInput
    {
        // <summary>
        // The max number of supported joysticks.
        // </summary>
        public const int MaxSupportedJoysticks = 16;

        /// <summary>
        /// The global JoystickState cached to avoid allocations.
        /// </summary>
        private static JoystickState GlobalJSState = Joystick.GetState(-1);

        #region Axes and Button Capabilities

        /// <summary>
        /// Tells whether a Joystick has a particular axis index.
        /// </summary>
        /// <param name="index">The index of the Joystick.</param>
        /// <param name="axis">The axis index.</param>
        /// <returns>true if the Joystick's AxisCount is greater than the axis index, otherwise false.</returns>
        public static bool HasAxis(in int index, in int axis)
        {
            JoystickCapabilities capabilities = Joystick.GetCapabilities(index);
            return (capabilities.IsConnected == true && axis >= 0 && capabilities.AxisCount > axis);
        }

        /// <summary>
        /// Tells if the Joystick has a particular button index.
        /// </summary>
        /// <param name="index">The index of the Joystick.</param>
        /// <param name="button">The button index.</param>
        /// <returns>true if the Joystick's ButtonCount is greater than the button index, otherwise false.</returns>
        public static bool HasButton(in int index, in int button)
        {
            JoystickCapabilities capabilities = Joystick.GetCapabilities(index);
            return (capabilities.IsConnected == true && button >= 0 && capabilities.ButtonCount > button);
        }

        /// <summary>
        /// Tells if the Joystick has a left thumbstick.
        /// </summary>
        /// <param name="index">The index of the Joystick.</param>
        /// <returns>true if so, otherwise false.</returns>
        public static bool HasLeftStick(in int index)
        {
            return HasAxis(index, 1);
        }

        /// <summary>
        /// Tells if the Joystick has a right thumbstick.
        /// </summary>
        /// <param name="index">The index of the Joystick.</param>
        /// <returns>true if so, otherwise false.</returns>
        public static bool HasRightStick(in int index)
        {
            return HasAxis(index, 3);
        }

        #endregion

        #region Raw and Normalized Axes Values

        /// <summary>
        /// Gets a raw joystick axis value ranging from -32768 to 32767.
        /// </summary>
        /// <param name="index">The index of the Joystick.</param>
        /// <param name="axis">The axis index.</param>
        /// <returns>An int containing the raw axis value from -32768 to 32767.
        /// If the joystick doesn't have the axis, then 0.</returns>
        public static int GetRawJoystickAxisValue(in int index, in int axis)
        {
            //Check if the joystick has the axis
            if (HasAxis(index, axis) == false)
            {
                //It doesn't, so return zero
                return 0;
            }

            GetJoystickState(index, ref GlobalJSState);
            return GlobalJSState.Axes[axis];
        }

        /// <summary>
        /// Gets raw joystick axes values ranging from -32768 to 32767.
        /// </summary>
        /// <param name="index">The index of the Joystick.</param>
        /// <param name="xAxis">The axis value for the X axis.</param>
        /// <param name="yAxis">The axis value for the Y axis.</param>
        /// <returns>A Vector2 containing the raw X and Y axes values from -32768 to 32767.
        /// If one or more of the axes doesn't exist, the X and Y values will be 0.</returns>
        public static Vector2 GetRawJoystickAxesValues(in int index, in int xAxis, in int yAxis)
        {
            return new Vector2(GetRawJoystickAxisValue(index, xAxis), GetRawJoystickAxisValue(index, yAxis));
        }

        /// <summary>
        /// Gets a normalized joystick axis value ranging from -1 to 1.
        /// </summary>
        /// <param name="index">The index of the Joystick.</param>
        /// <param name="axis">The axis index.</param>
        /// <returns>A float containing the raw axis value from -1 to 1.
        /// If the joystick doesn't have the axis, then 0.</returns>
        public static float GetNormalizedJoystickAxisValue(in int index, in int axis)
        {
            float value = GetRawJoystickAxisValue(index, axis);

            if (value < 0)
            {
                value = value / (-InputGlobals.MinJoystickAxisValue);
            }
            else if (value > 0)
            {
                value = value / InputGlobals.MaxJoystickAxisValue;
            }

            return value;
        }

        /// <summary>
        /// Gets raw joystick axes values normalized to the -1 to 1 range.
        /// </summary>
        /// <param name="index">The index of the Joystick.</param>
        /// <param name="xAxis">The axis value for the X axis.</param>
        /// <param name="yAxis">The axis value for the Y axis.</param>
        /// <returns>A Vector2 containing the normalized X and Y axes values from -1 to 1.</returns>
        public static Vector2 GetNormalizedJoystickAxesValues(in int index, in int xAxis, in int yAxis)
        {
            return new Vector2(GetNormalizedJoystickAxisValue(index, xAxis), GetNormalizedJoystickAxisValue(index, yAxis));
        }

        #endregion

        #region Buttons

        /// <summary>
        /// Tells whether a button on the Joystick is pressed.
        /// </summary>
        /// <param name="index">The index of the Joystick.</param>
        /// <param name="button">The button index.</param>
        /// <returns>true if the button is pressed, otherwise false.</returns>
        public static bool GetButton(in int index, in int button)
        {
            //Check if this joystick has this button available
            if (HasButton(index, button) == false)
            {
                return false;
            }

            //Check if the button is down
            GetJoystickState(index, ref GlobalJSState);
            return (GlobalJSState.Buttons[button] == ButtonState.Pressed);
        }

        /// <summary>
        /// Tells whether a button on the Joystick was just pressed.
        /// </summary>
        /// <param name="index">The index of the Joystick.</param>
        /// <param name="button">The button index.</param>
        /// <param name="joystickState">The JoystickState to check.</param>
        /// <returns>true if the button was just pressed, otherwise false.</returns>
        public static bool GetButtonDown(in int index, in int button, JoystickState joystickState)
        {
            //Check if this joystick has this button available
            if (joystickState.IsConnected == false || HasButton(index, button) == false)
            {
                return false;
            }

            //Check if the button is down
            GetJoystickState(index, ref GlobalJSState);
            return (joystickState.Buttons[button] == ButtonState.Released && GlobalJSState.Buttons[button] == ButtonState.Pressed);
        }

        /// <summary>
        /// Tells whether a button on the Joystick was just released.
        /// </summary>
        /// <param name="index">The index of the Joystick.</param>
        /// <param name="button">The button index.</param>
        /// <param name="joystickState">The JoystickState to check.</param>
        /// <returns>true if the button was just released, otherwise false.</returns>
        public static bool GetButtonUp(in int index, in int button, JoystickState joystickState)
        {
            //Check if this joystick has this button available
            if (joystickState.IsConnected == false || HasButton(index, button) == false)
            {
                return false;
            }

            //Check if the button is down
            GetJoystickState(index, ref GlobalJSState);
            return (joystickState.Buttons[button] == ButtonState.Pressed && GlobalJSState.Buttons[button] == ButtonState.Released);
        }

        #endregion

        /// <summary>
        /// Updates a JoystickState.
        /// </summary>
        /// <param name="index">The index of the Joystick to use.</param>
        /// <param name="joystickState">The JoystickState to update.</param>
        public static void UpdateJoystickState(in int index, ref JoystickState joystickState)
        {
            GetJoystickState(index, ref joystickState);
        }

        /// <summary>
        /// Returns if a joystick is connected.
        /// </summary>
        /// <param name="index">The index of the Joystick to use.</param>
        /// <returns>true if the Joystick is currently connected, otherwise false.</returns>
        public static bool IsJoystickConnected(in int index)
        {
            JoystickCapabilities capabilities = Joystick.GetCapabilities(index);
            return capabilities.IsConnected;
        }

        /// <summary>
        /// Returns true if a Joystick was connected this frame.
        /// </summary>
        /// <param name="index">The index of the Joystick to use.</param>
        /// <param name="curJoystickState">The JoystickState to compare with.</param>
        /// <returns>true if the Joystick at <paramref name="index"/> was connected this frame, otherwise false.</returns>
        public static bool HasJoystickJustConnected(in int index, JoystickState curJoystickState)
        {
            GetJoystickState(index, ref GlobalJSState);

            return (curJoystickState.IsConnected == false && GlobalJSState.IsConnected == true);
        }

        /// <summary>
        /// Returns true if a Joystick was disconnected this frame.
        /// </summary>
        /// <param name="index">The index of the Joystick to use.</param>
        /// <param name="curJoystickState">The JoystickState to compare with.</param>
        /// <returns>true if the Joystick at <paramref name="index"/> was disconnected this frame, otherwise false.</returns>
        public static bool HasJoystickJustDisconnected(in int index, JoystickState curJoystickState)
        {
            GetJoystickState(index, ref GlobalJSState);

            return (curJoystickState.IsConnected == true && GlobalJSState.IsConnected == false);
        }

        /// <summary>
        /// Gets the current JoystickState of a Joystick at a particular index.
        /// </summary>
        /// <param name="index">The index of the Joystick.</param>
        /// <returns>A JoystickState.</returns>
        public static JoystickState GetJoystickState(in int index)
        {
            return Joystick.GetState(index);
        }

        /// <summary>
        /// Gets the current JoystickState of a Joystick at a particular index by updating an existing JoystickState.
        /// </summary>
        /// <param name="index">The index of the Joystick.</param>
        /// <param name="jsState">The JoystickState to update.</param>
        public static void GetJoystickState(in int index, ref JoystickState jsState)
        {
            Joystick.GetState(ref jsState, index);
        }

        /// <summary>
        /// Clears a JoystickState.
        /// </summary>
        /// <param name="joystickState">The JoystickState to clear.</param>
        public static void ClearJoystickState(ref JoystickState joystickState)
        {
            joystickState = new JoystickState();
        }
    }
}
