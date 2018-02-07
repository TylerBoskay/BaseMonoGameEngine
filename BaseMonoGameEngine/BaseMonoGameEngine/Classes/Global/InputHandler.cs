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
    /// Instance-based input handler for either a keyboard or joystick.
    /// </summary>
    public sealed class InputHandler
    {
        /// <summary>
        /// The types of input.
        /// </summary>
        public enum InputTypes
        {
            Keyboard,
            Joystick
        }

        /// <summary>
        /// The input type for this InputHandler.
        /// </summary>
        public InputTypes InputType = InputTypes.Keyboard;

        /// <summary>
        /// The input mappings for this InputHandler.
        /// </summary>
        private readonly Dictionary<string, InputMapValue> InputMappings = new Dictionary<string, InputMapValue>();

        /// <summary>
        /// The KeyboardState to help determine differences in input.
        /// </summary>
        private KeyboardState KBState = default(KeyboardState);

        /// <summary>
        /// The JoystickState to help determine differences in input.
        /// </summary>
        private JoystickState JSState = default(JoystickState);

        public InputHandler(InputTypes inputType)
        {
            InputType = inputType;
        }

        /// <summary>
        /// Creates a new InputHandler with a default mapping based on the input type.
        /// </summary>
        /// <param name="inputType">The InputTypes.</param>
        /// <returns>An InputHandler with default mappings for the specified input type.</returns>
        public static InputHandler CreateWithDefaultMappings(InputTypes inputType)
        {
            InputHandler inputHandler = new InputHandler(inputType);
            inputHandler.SetDefaultMappings(inputType);

            return inputHandler;
        }

        /// <summary>
        /// Tells whether the InputHandler has an input action.
        /// </summary>
        /// <param name="action">The input action.</param>
        /// <returns>true if the InputHandler has the input action, otherwise false.</returns>
        private bool HasInputAction(string action)
        {
            if (action == null) return false;

            return InputMappings.ContainsKey(action);
        }

        /// <summary>
        /// Gets the axis value for an action.
        /// </summary>
        /// <param name="playerIndex">The player index.</param>
        /// <param name="action">The action to test.</param>
        /// <returns>A float from -1 to 1 representing the axis value.</returns>
        public float GetAxis(int playerIndex, string action)
        {
            if (HasInputAction(action) == false)
            {
                return 0f;
            }

            InputMapValue val = InputMappings[action];

            return GetAxisForInputType(playerIndex, ref val);
        }

        /// <summary>
        /// Tells whether a button is pressed.
        /// </summary>
        /// <param name="playerIndex">The player index.</param>
        /// <param name="action">The action to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public bool GetButton(int playerIndex, string action)
        {
            if (HasInputAction(action) == false)
            {
                return false;
            }

            InputMapValue val = InputMappings[action];

            return GetButtonForInputType(playerIndex, ref val);
        }

        /// <summary>
        /// Tells whether a button was just pressed.
        /// </summary>
        /// <param name="playerIndex">The player index.</param>
        /// <param name="action">The action to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public bool GetButtonDown(int playerIndex, string action)
        {
            if (HasInputAction(action) == false)
            {
                return false;
            }

            InputMapValue val = InputMappings[action];

            return GetButtonDownForInputType(playerIndex, ref val);
        }

        /// <summary>
        /// Tells whether a button was just released.
        /// </summary>
        /// <param name="playerIndex">The player index.</param>
        /// <param name="action">The action to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public bool GetButtonUp(int playerIndex, string action)
        {
            if (HasInputAction(action) == false)
            {
                return false;
            }

            InputMapValue val = InputMappings[action];

            return GetButtonUpForInputType(playerIndex, ref val);
        }

        /// <summary>
        /// Gets the axis value for a particular input type.
        /// </summary>
        /// <param name="playerIndex">The player index.</param>
        /// <param name="inputMapValue">The input mapping value.</param>
        /// <returns>A float from -1 to 1 representing the axis value.</returns>
        private float GetAxisForInputType(int playerIndex, ref InputMapValue buttonMapValue)
        {
            if (InputType == InputTypes.Keyboard)
            {
                bool pressed = KeyboardInput.GetKey((Keys)buttonMapValue.InputVal, ref KBState);
                float axisVal = 0f;

                //Check if there's a negative value if this isn't pressed
                if (pressed == false)
                {
                    //If the negative value is pressed, set the axis value to -1
                    if (buttonMapValue.NegativeInputVal.HasValue == true && KeyboardInput.GetKey((Keys)buttonMapValue.NegativeInputVal, ref KBState) == true)
                    {
                        axisVal = -1f;
                    }
                }
                else
                {
                    axisVal = 1f;
                }

                return axisVal;
            }
            else
            {
                return JoystickInput.GetNormalizedJoystickAxisValue(playerIndex, buttonMapValue.InputVal);
            }
        }

        /// <summary>
        /// Tells whether a button is pressed for a particular input type.
        /// </summary>
        /// <param name="playerIndex">The player index.</param>
        /// <param name="inputMapValue">The input mapping value.</param>
        /// <returns>true if so, otherwise false.</returns>
        private bool GetButtonForInputType(int playerIndex, ref InputMapValue buttonMapValue)
        {
            if (InputType == InputTypes.Keyboard)
            {
                return KeyboardInput.GetKey((Keys)buttonMapValue.InputVal, ref KBState);
            }
            else
            {
                return JoystickInput.GetButton(playerIndex, buttonMapValue.InputVal);
            }
        }

        /// <summary>
        /// Tells whether a button was just pressed for a particular input type.
        /// </summary>
        /// <param name="playerIndex">The player index.</param>
        /// <param name="inputMapValue">The input mapping value.</param>
        /// <returns>true if so, otherwise false.</returns>
        private bool GetButtonDownForInputType(int playerIndex, ref InputMapValue buttonMapValue)
        {
            if (InputType == InputTypes.Keyboard)
            {
                return KeyboardInput.GetKeyDown((Keys)buttonMapValue.InputVal, ref KBState);
            }
            else
            {
                return JoystickInput.GetButtonDown(playerIndex, buttonMapValue.InputVal, ref JSState);
            }
        }

        /// <summary>
        /// Tells whether a button was just released for a particular input type.
        /// </summary>
        /// <param name="playerIndex">The player index.</param>
        /// <param name="inputMapValue">The input mapping value.</param>
        /// <returns>true if so, otherwise false.</returns>
        private bool GetButtonUpForInputType(int playerIndex, ref InputMapValue inputMapValue)
        {
            if (InputType == InputTypes.Keyboard)
            {
                return KeyboardInput.GetKeyUp((Keys)inputMapValue.InputVal, ref KBState);
            }
            else
            {
                return JoystickInput.GetButtonUp(playerIndex, inputMapValue.InputVal, ref JSState);
            }
        }

        /// <summary>
        /// Adds a new mapped input. If one already exists, it will be replaced.
        /// </summary>
        /// <param name="action">The action to map.</param>
        /// <param name="value">The value mapped to the action.</param>
        public void AddMapping(string action, InputMapValue value)
        {
            //Null keys cannot be added
            if (action == null)
            {
                return;
            }

            //Remove a mapping if it exists
            if (HasInputAction(action) == true)
            {
                InputMappings.Remove(action);
            }

            //Add the new mapping
            InputMappings.Add(action, value);
        }

        /// <summary>
        /// Clears a mapped input.
        /// </summary>
        /// <param name="action">The action to clear the mapping for.</param>
        public void ClearMapping(string action)
        {
            //Null keys cannot be added or removed
            if (action == null)
            {
                return;
            }

            InputMappings.Remove(action);
        }

        /// <summary>
        /// Clears all mappings.
        /// </summary>
        public void ClearMappings()
        {
            InputMappings.Clear();
        }

        /// <summary>
        /// Clears all mappings and sets them to the default mapping.
        /// </summary>
        /// <param name="inputType">The type of input to set the default mapping for.</param>
        public void SetDefaultMappings(InputTypes inputType)
        {
            ClearMappings();

            InputType = inputType;

            if (InputType == InputTypes.Keyboard)
            {
                AddMapping(InputActions.Horizontal, new InputMapValue((int)Keys.Right, (int)Keys.Left));
                AddMapping(InputActions.Vertical, new InputMapValue((int)Keys.Down, (int)Keys.Up));

                AddMapping(InputActions.A, new InputMapValue((int)Keys.Z));
                AddMapping(InputActions.B, new InputMapValue((int)Keys.X));
                AddMapping(InputActions.X, new InputMapValue((int)Keys.A));
                AddMapping(InputActions.Y, new InputMapValue((int)Keys.S));
            }
            else if (InputType == InputTypes.Joystick)
            {
                AddMapping(InputActions.Horizontal, new InputMapValue(0));
                AddMapping(InputActions.Vertical, new InputMapValue(1));

                AddMapping(InputActions.A, new InputMapValue(0));
                AddMapping(InputActions.B, new InputMapValue(1));
                AddMapping(InputActions.X, new InputMapValue(7));
                AddMapping(InputActions.Y, new InputMapValue(8));
            }
        }

        /// <summary>
        /// Clears the input state.
        /// </summary>
        public void ClearInputState()
        {
            KeyboardInput.ClearKeyboardState(ref KBState);
            JoystickInput.ClearJoystickState(ref JSState);
        }

        /// <summary>
        /// Updates the input state.
        /// </summary>
        /// <param name="playerIndex">The player index for updating the joystick input state.</param>
        public void UpdateInputState(int playerIndex)
        {
            KeyboardInput.UpdateKeyboardState(ref KBState);
            JoystickInput.UpdateJoystickState(playerIndex, ref JSState);
        }

        /// <summary>
        /// Holds values regarding an input mapping.
        /// </summary>
        public struct InputMapValue
        {
            /// <summary>
            /// The input value.
            /// </summary>
            public int InputVal { get; private set; }

            /// <summary>
            /// The negative input value. This is used only for non-joystick inputs when getting the axis value.
            /// <para>For example, on a keyboard you can specify the positive as Down and the negative as Up.</para>
            /// </summary>
            public int? NegativeInputVal { get; private set; }

            public InputMapValue(int buttonVal) : this (buttonVal, null)
            {
                
            }

            public InputMapValue(int buttonVal, int? negativeInputVal)
            {
                InputVal = buttonVal;
                NegativeInputVal = negativeInputVal;
            }
        }
    }
}
