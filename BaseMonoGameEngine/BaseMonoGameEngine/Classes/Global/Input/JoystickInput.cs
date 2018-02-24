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
        /// <summary>
        /// The max number of supported joysticks.
        /// </summary>
        //private static int MaxJoysticks = 4;
        
        /// <summary>
        /// The cached set of JoystickStates.
        /// </summary>
        //private static JoystickState[] JoystickStates = null;

        static JoystickInput()
        {
            //JoystickStates = new JoystickState[MaxJoysticks];
        }

        #region Axes and Button Capabilities

        /// <summary>
        /// Tells whether a Joystick has a particular axis index.
        /// </summary>
        /// <param name="index">The index of the Joystick.</param>
        /// <param name="axis">The axis index.</param>
        /// <returns>true if the Joystick's AxisCount is greater than the axis index, otherwise false.</returns>
        public static bool HasAxis(int index, int axis)
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
        public static bool HasButton(int index, int button)
        {
            JoystickCapabilities capabilities = Joystick.GetCapabilities(index);
            return (capabilities.IsConnected == true && button >= 0 && capabilities.ButtonCount > button);
        }

        /// <summary>
        /// Tells if the Joystick has a left thumbstick.
        /// </summary>
        /// <param name="index">The index of the Joystick.</param>
        /// <returns>true if so, otherwise false.</returns>
        public static bool HasLeftStick(int index)
        {
            return HasAxis(index, 1);
        }

        /// <summary>
        /// Tells if the Joystick has a right thumbstick.
        /// </summary>
        /// <param name="index">The index of the Joystick.</param>
        /// <returns>true if so, otherwise false.</returns>
        public static bool HasRightStick(int index)
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
        public static int GetRawJoystickAxisValue(int index, int axis)
        {
            //Check if the joystick has the axis
            if (HasAxis(index, axis) == false)
            {
                //It doesn't, so return zero
                return 0;
            }

            JoystickState state = GetJoystickState(index);
            return state.Axes[axis];
        }

        /// <summary>
        /// Gets raw joystick axes values ranging from -32768 to 32767.
        /// </summary>
        /// <param name="index">The index of the Joystick.</param>
        /// <param name="xAxis">The axis value for the X axis.</param>
        /// <param name="yAxis">The axis value for the Y axis.</param>
        /// <returns>A Vector2 containing the raw X and Y axes values from -32768 to 32767.
        /// If one or more of the axes doesn't exist, the X and Y values will be 0.</returns>
        public static Vector2 GetRawJoystickAxesValues(int index, int xAxis, int yAxis)
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
        public static float GetNormalizedJoystickAxisValue(int index, int axis)
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
        public static Vector2 GetNormalizedJoystickAxesValues(int index, int xAxis, int yAxis)
        {
            return new Vector2(GetNormalizedJoystickAxisValue(index, xAxis), GetNormalizedJoystickAxisValue(index, yAxis));
        }

        /// <summary>
        /// Gets the normalized axes values of the left stick.
        /// </summary>
        /// <param name="index">The index of the Joystick.</param>
        /// <returns>A Vector2 containing the normalized X and Y axes values from -1 to 1.</returns>
        public static Vector2 GetLeftStickAxisValue(int index)
        {
            return GetNormalizedJoystickAxesValues(index, 0, 1);
        }

        /// <summary>
        /// Gets the normalized axes values of the left stick.
        /// </summary>
        /// <param name="index">The index of the Joystick.</param>
        /// <returns>A Vector2 containing the normalized X and Y axes values from -1 to 1.</returns>
        public static Vector2 GetRightStickAxisValue(int index)
        {
            JoystickCapabilities capabilities = Joystick.GetCapabilities(index);
            
            //2 and 3 are default right-stick axes
            int x = 2;
            int y = 3;

            //The GCN controller has 3 and 4 as the right stick's X and Y axes, respectively
            //The L and R triggers are their own axes, at 5 and 2, respectively
            //Other controllers might be the same, so for safety try these
            if (capabilities.AxisCount > 4)
            {
                x = 3;
                y = 4;
            }

            return GetNormalizedJoystickAxesValues(index, x, y);
        }

        #endregion

        #region Buttons

        /// <summary>
        /// Tells whether a button on the Joystick is pressed.
        /// </summary>
        /// <param name="index">The index of the Joystick.</param>
        /// <param name="button">The button index.</param>
        /// <returns>true if the button is pressed, otherwise false.</returns>
        public static bool GetButton(int index, int button)
        {
            //Check if this joystick has this button available
            if (HasButton(index, button) == false)
            {
                return false;
            }

            //Check if the button is down
            JoystickState curState = GetJoystickState(index);
            return (curState.Buttons[button] == ButtonState.Pressed);
        }

        ///// <summary>
        ///// Tells whether a button on the Joystick was just pressed.
        ///// </summary>
        ///// <param name="index">The index of the Joystick.</param>
        ///// <param name="button">The button index.</param>
        ///// <returns>true if the button was just pressed, otherwise false.</returns>
        //public static bool GetButtonDown(int index, int button)
        //{
        //    //Check if this joystick has this button available
        //    if (HasButton(index, button) == false)
        //    {
        //        return false;
        //    }
        //
        //    //Check if the button is down
        //    JoystickState curState = GetJoystickState(index);
        //    return (JoystickStates[index].Buttons[button] == ButtonState.Released && curState.Buttons[button] == ButtonState.Pressed);
        //}

        /// <summary>
        /// Tells whether a button on the Joystick was just pressed.
        /// </summary>
        /// <param name="index">The index of the Joystick.</param>
        /// <param name="button">The button index.</param>
        /// <param name="joystickState">The JoystickState to check.</param>
        /// <returns>true if the button was just pressed, otherwise false.</returns>
        public static bool GetButtonDown(int index, int button, in JoystickState joystickState)
        {
            //Check if this joystick has this button available
            if (HasButton(index, button) == false || joystickState.IsConnected == false)
            {
                return false;
            }

            //Check if the button is down
            JoystickState curState = GetJoystickState(index);
            return (joystickState.Buttons[button] == ButtonState.Released && curState.Buttons[button] == ButtonState.Pressed);
        }

        ///// <summary>
        ///// Tells whether a button on the Joystick was just released.
        ///// </summary>
        ///// <param name="index">The index of the Joystick.</param>
        ///// <param name="button">The button index.</param>
        ///// <returns>true if the button was just released, otherwise false.</returns>
        //public static bool GetButtonUp(int index, int button)
        //{
        //    //Check if this joystick has this button available
        //    if (HasButton(index, button) == false)
        //    {
        //        return false;
        //    }
        //
        //    //Check if the button is down
        //    JoystickState curState = GetJoystickState(index);
        //    return (JoystickStates[index].Buttons[button] == ButtonState.Pressed && curState.Buttons[button] == ButtonState.Released);
        //}

        /// <summary>
        /// Tells whether a button on the Joystick was just released.
        /// </summary>
        /// <param name="index">The index of the Joystick.</param>
        /// <param name="button">The button index.</param>
        /// <param name="joystickState">The JoystickState to check.</param>
        /// <returns>true if the button was just released, otherwise false.</returns>
        public static bool GetButtonUp(int index, int button, in JoystickState joystickState)
        {
            //Check if this joystick has this button available
            if (HasButton(index, button) == false || joystickState.IsConnected == false)
            {
                return false;
            }

            //Check if the button is down
            JoystickState curState = GetJoystickState(index);
            return (joystickState.Buttons[button] == ButtonState.Pressed && curState.Buttons[button] == ButtonState.Released);
        }

        #endregion

        /// <summary>
        /// Updates a JoystickState.
        /// </summary>
        /// <param name="index">The index of the Joystick to use.</param>
        /// <param name="joystickState">The JoystickState to update.</param>
        public static void UpdateJoystickState(int index, ref JoystickState joystickState)
        {
            joystickState = GetJoystickState(index);
        }

        ///// <summary>
        ///// Updates all JoystickStates from 0 to <see cref="MaxJoysticks"/>.
        ///// </summary>
        //public static void UpdateAllJoystickStates()
        //{
        //    for (int i = 0; i < MaxJoysticks; i++)
        //    {
        //        UpdateJoystickState(i, ref JoystickStates[i]);
        //    }
        //}

        /// <summary>
        /// Gets the current JoystickState of a Joystick at a particular index.
        /// </summary>
        /// <param name="index">The index of the Joystick.</param>
        /// <returns>A JoystickState.</returns>
        public static JoystickState GetJoystickState(int index)
        {
            return Joystick.GetState(index);
        }

        ///// <summary>
        ///// Clears the JoystickState at the specified index.
        ///// </summary>
        ///// <param name="index">The index of the Joystick.</param>
        //public static void ClearJoystickState(int index)
        //{
        //    JoystickStates[index] = new JoystickState();
        //}

        /// <summary>
        /// Clears a JoystickState.
        /// </summary>
        /// <param name="joystickState">The JoystickState to clear.</param>
        public static void ClearJoystickState(ref JoystickState joystickState)
        {
            joystickState = new JoystickState();
        }

        /// <summary>
        /// Gets all button states for the Joystick at the specified index.
        /// </summary>
        /// <param name="index">The index of the Joystick.</param>
        /// <returns>An array of ButtonStates.</returns>
        public static ButtonState[] GetJoystickButtons(int index)
        {
            JoystickState state = GetJoystickState(index);

            return state.Buttons;
        }
    }
}
