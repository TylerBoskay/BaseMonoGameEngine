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
    /// Instance-based input handler for either a keyboard, gamepad, or joystick.
    /// </summary>
    public sealed class InputHandler
    {
        /// <summary>
        /// The types of input.
        /// </summary>
        public enum InputTypes
        {
            Keyboard,
            GamePad,
            Joystick,
        }

        /// <summary>
        /// The player index of this InputHandler.
        /// </summary>
        public int PlayerIndex = 0;

        /// <summary>
        /// The controller index of this InputHandler for gamepads and joysticks.
        /// </summary>
        public int ControllerIndex = 0;

        /// <summary>
        /// The input type for this InputHandler.
        /// </summary>
        public InputTypes InputType = InputTypes.Keyboard;

        /// <summary>
        /// The input mappings for this InputHandler.
        /// </summary>
        private readonly Dictionary<string, InputMapValue> InputMappings = new Dictionary<string, InputMapValue>(11);

        /// <summary>
        /// The KeyboardState to help determine differences in input.
        /// </summary>
        private KeyboardState KBState = default(KeyboardState);

        /// <summary>
        /// The GamePadState to help determine differences in input.
        /// </summary>
        private GamePadState GPState = GamePadState.Default;

        /// <summary>
        /// The JoystickState to help determine differences in input.
        /// </summary>
        private JoystickState JSState = Joystick.GetState(-1);

        public InputHandler(in InputTypes inputType, in int playerIndex, in int controllerIndex)
        {
            InputType = inputType;
            PlayerIndex = playerIndex;
            ControllerIndex = controllerIndex;
        }

        /// <summary>
        /// Creates a new InputHandler with a default mapping based on the input type.
        /// </summary>
        /// <param name="inputType">The InputTypes.</param>
        /// <returns>An InputHandler with default mappings for the specified input type.</returns>
        public static InputHandler CreateWithDefaultMappings(in InputTypes inputType, in int playerIndex, in int controllerIndex)
        {
            InputHandler inputHandler = new InputHandler(inputType, playerIndex, controllerIndex);
            inputHandler.SetDefaultMappings(inputType);

            return inputHandler;
        }

        /// <summary>
        /// Tells whether the InputHandler has an input action.
        /// </summary>
        /// <param name="action">The input action.</param>
        /// <returns>true if the InputHandler has the input action, otherwise false.</returns>
        public bool HasInputAction(in string action)
        {
            if (action == null) return false;

            return InputMappings.ContainsKey(action);
        }

        /// <summary>
        /// Gets the axis value for an action.
        /// </summary>
        /// <param name="action">The action to test.</param>
        /// <returns>A float from -1 to 1 representing the axis value.</returns>
        public float GetAxis(in string action)
        {
            if (InputMappings.TryGetValue(action, out InputMapValue val) == false)
            {
                return 0f;
            }

            return GetAxisForInputType(val);
        }

        /// <summary>
        /// Tells whether a button is pressed.
        /// </summary>
        /// <param name="action">The action to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public bool GetButton(in string action)
        {
            if (InputMappings.TryGetValue(action, out InputMapValue val) == false)
            {
                return false;
            }

            return GetButtonForInputType(val);
        }

        /// <summary>
        /// Tells whether a button was just pressed.
        /// </summary>
        /// <param name="action">The action to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public bool GetButtonDown(in string action)
        {
            if (InputMappings.TryGetValue(action, out InputMapValue val) == false)
            {
                return false;
            }

            return GetButtonDownForInputType(val);
        }

        /// <summary>
        /// Tells whether a button was just released.
        /// </summary>
        /// <param name="action">The action to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public bool GetButtonUp(in string action)
        {
            if (InputMappings.TryGetValue(action, out InputMapValue val) == false)
            {
                return false;
            }

            return GetButtonUpForInputType(val);
        }

        /// <summary>
        /// Gets the axis value for a particular input type.
        /// </summary>
        /// <param name="inputMapValue">The input mapping value.</param>
        /// <returns>A float from -1 to 1 representing the axis value.</returns>
        private float GetAxisForInputType(in InputMapValue buttonMapValue)
        {
            if (InputType == InputTypes.Keyboard)
            {
                bool pressed = KeyboardInput.GetKey((Keys)buttonMapValue.InputVal, KBState);
                float axisVal = 0f;

                //Check if there's a negative value if this isn't pressed
                if (pressed == false)
                {
                    //If the negative value is pressed, set the axis value to -1
                    if (buttonMapValue.NegativeInputVal.HasValue == true && KeyboardInput.GetKey((Keys)buttonMapValue.NegativeInputVal, KBState) == true)
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
            //NOTE: The requirement for this method is hazy when it comes to GamePad; this likely doesn't work as intended
            else if (InputType == InputTypes.GamePad)
            {
                int val = buttonMapValue.InputVal;

                switch (val)
                {
                    default:
                    case 0: return GamePadInput.GetLeftStickValue(ControllerIndex).X;
                    case 1: return GamePadInput.GetLeftStickValue(ControllerIndex).Y;
                    case 2: return GamePadInput.GetRightStickValue(ControllerIndex).X;
                    case 3: return GamePadInput.GetRightStickValue(ControllerIndex).Y;
                }
            }
            else
            {
                return JoystickInput.GetNormalizedJoystickAxisValue(ControllerIndex, buttonMapValue.InputVal);
            }
        }

        /// <summary>
        /// Tells whether a button is pressed for a particular input type.
        /// </summary>
        /// <param name="inputMapValue">The input mapping value.</param>
        /// <returns>true if so, otherwise false.</returns>
        private bool GetButtonForInputType(in InputMapValue buttonMapValue)
        {
            if (InputType == InputTypes.Keyboard)
            {
                return KeyboardInput.GetKey((Keys)buttonMapValue.InputVal);
            }
            else if (InputType == InputTypes.GamePad)
            {
                return GamePadInput.GetButton(ControllerIndex, (Buttons)buttonMapValue.InputVal);
            }
            else
            {
                return JoystickInput.GetButton(ControllerIndex, buttonMapValue.InputVal);
            }
        }

        /// <summary>
        /// Tells whether a button was just pressed for a particular input type.
        /// </summary>
        /// <param name="inputMapValue">The input mapping value.</param>
        /// <returns>true if so, otherwise false.</returns>
        private bool GetButtonDownForInputType(in InputMapValue buttonMapValue)
        {
            if (InputType == InputTypes.Keyboard)
            {
                return KeyboardInput.GetKeyDown((Keys)buttonMapValue.InputVal, KBState);
            }
            else if (InputType == InputTypes.GamePad)
            {
                return GamePadInput.GetButtonDown(ControllerIndex, (Buttons)buttonMapValue.InputVal, GPState);
            }
            else
            {
                return JoystickInput.GetButtonDown(ControllerIndex, buttonMapValue.InputVal, JSState);
            }
        }

        /// <summary>
        /// Tells whether a button was just released for a particular input type.
        /// </summary>
        /// <param name="inputMapValue">The input mapping value.</param>
        /// <returns>true if so, otherwise false.</returns>
        private bool GetButtonUpForInputType(in InputMapValue inputMapValue)
        {
            if (InputType == InputTypes.Keyboard)
            {
                return KeyboardInput.GetKeyUp((Keys)inputMapValue.InputVal, KBState);
            }
            else if (InputType == InputTypes.GamePad)
            {
                return GamePadInput.GetButtonUp(ControllerIndex, (Buttons)inputMapValue.InputVal, GPState);
            }
            else
            {
                return JoystickInput.GetButtonUp(ControllerIndex, inputMapValue.InputVal, JSState);
            }
        }

        /// <summary>
        /// Adds a new mapped input. If one already exists, it will be replaced.
        /// </summary>
        /// <param name="action">The action to map.</param>
        /// <param name="value">The value mapped to the action.</param>
        public void AddMapping(in string action, in InputMapValue value)
        {
            //Null keys cannot be added
            if (action == null)
            {
                return;
            }

            //Remove a mapping if it exists
            InputMappings.Remove(action);

            //Add the new mapping
            InputMappings.Add(action, value);
        }

        /// <summary>
        /// Clears a mapped input.
        /// </summary>
        /// <param name="action">The action to clear the mapping for.</param>
        public void ClearMapping(in string action)
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
        public void SetDefaultMappings(in InputTypes inputType)
        {
            ClearMappings();

            InputType = inputType;

            CopyMappingsFromDict(InputGlobals.DefaultMappings[InputType]);
        }

        /// <summary>
        /// Clears the input state.
        /// </summary>
        public void ClearInputState()
        {
            if (InputType == InputTypes.Keyboard)
            {
                KeyboardInput.ClearKeyboardState(ref KBState);
            }
            else if (InputType == InputTypes.GamePad)
            {
                GPState = GamePadState.Default;
            }
            else
            {
                JoystickInput.ClearJoystickState(ref JSState);
            }
        }

        /// <summary>
        /// Updates the input state.
        /// </summary>
        public void UpdateInputState()
        {
            if (InputType == InputTypes.Keyboard)
            {
                KeyboardInput.UpdateKeyboardState(ref KBState);
            }
            else if (InputType == InputTypes.GamePad)
            {
                GamePadInput.GetGamePadState(ControllerIndex, ref GPState);
            }
            else
            {
                JoystickInput.UpdateJoystickState(ControllerIndex, ref JSState);
            }
        }

        /// <summary>
        /// Gets the <see cref="InputMapValue"/> associated with an input for this input handler.
        /// </summary>
        /// <param name="input">The input to get the value for.</param>
        /// <returns>An <see cref="InputMapValue"/> associated with the input if found, otherwise a default value.</returns>
        public InputMapValue GetMappingVal(in string input)
        {
            if (InputMappings.TryGetValue(input, out InputMapValue val) == false)
            {
                val = default(InputMapValue);
            }

            return val;
        }

        /// <summary>
        /// Copies the input handler's input mappings into a supplied dictionary.
        /// </summary>
        /// <param name="dictCopiedTo">The dictionary to copy input mappings to.</param>
        public void CopyMappingsToDict(Dictionary<string, int> dictCopiedTo)
        {
            foreach (KeyValuePair<string, InputMapValue> keyValPair in InputMappings)
            {
                dictCopiedTo[keyValPair.Key] = keyValPair.Value.InputVal;
            }
        }

        /// <summary>
        /// Copies input mappings from a given dictionary into the input handler's input mappings.
        /// </summary>
        /// <param name="dictCopiedFrom">The dictionary to copy input mappings from.</param>
        public void CopyMappingsFromDict(Dictionary<string, int> dictCopiedFrom)
        {
            foreach (KeyValuePair<string, int> keyValPair in dictCopiedFrom)
            {
                AddMapping(keyValPair.Key, new InputMapValue(keyValPair.Value));
            }
        }

        /// <summary>
        /// Holds values regarding an input mapping.
        /// </summary>
        public readonly struct InputMapValue
        {
            /// <summary>
            /// The input value.
            /// </summary>
            public readonly int InputVal;

            /// <summary>
            /// The negative input value. This is used only for non-joystick inputs when getting the axis value.
            /// <para>For example, on a keyboard you can specify the positive as Down and the negative as Up.</para>
            /// </summary>
            public readonly int? NegativeInputVal;

            public InputMapValue(in int buttonVal) : this(buttonVal, null)
            {

            }

            public InputMapValue(in int buttonVal, int? negativeInputVal)
            {
                InputVal = buttonVal;
                NegativeInputVal = negativeInputVal;
            }

            public override bool Equals(object obj)
            {
                if (obj is InputMapValue inputMapValue)
                {
                    return (this == inputMapValue);
                }

                return false;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 37;
                    hash = (hash * 23) + InputVal.GetHashCode();
                    hash = (hash * 23) + NegativeInputVal.GetHashCode();
                    return hash;
                }
            }

            public static bool operator ==(InputMapValue a, InputMapValue b)
            {
                return (a.InputVal == b.InputVal && a.NegativeInputVal == b.NegativeInputVal);
            }

            public static bool operator !=(InputMapValue a, InputMapValue b)
            {
                return !(a == b);
            }
        }
    }
}
