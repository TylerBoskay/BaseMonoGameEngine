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
    /// Class for global values dealing with input.
    /// </summary>
    public static class InputGlobals
    {
        public const int MinJoystickAxisValue = short.MinValue;
        public const int MaxJoystickAxisValue = short.MaxValue;

        /// <summary>
        /// The starting input index used for gamepads.
        /// </summary>
        public const int GamePadInputIndex = 1;

        /// <summary>
        /// The max number of gamepads the game allows.
        /// </summary>
        public static readonly int MaxGamePadCount = GamePad.MaximumGamePadCount;

        #region Default Input Mappings

        public static readonly Dictionary<InputHandler.InputTypes, Dictionary<string, int>> DefaultMappings = new Dictionary<InputHandler.InputTypes, Dictionary<string, int>>()
        {
            {
                InputHandler.InputTypes.Keyboard, new Dictionary<string, int>(8)
                {
                    { InputActions.Left, (int)Keys.Left },
                    { InputActions.Right, (int)Keys.Right },
                    { InputActions.Up, (int)Keys.Up },
                    { InputActions.Down, (int)Keys.Down },
                    { InputActions.A, (int)Keys.Z},
                    { InputActions.B, (int)Keys.X },
                    { InputActions.X, (int)Keys.A },
                    { InputActions.Y, (int)Keys.S},
                }
            },
            {
                InputHandler.InputTypes.GamePad, new Dictionary<string, int>(8)
                {
                    { InputActions.Right, (int)Buttons.DPadRight },
                    { InputActions.Left, (int)Buttons.DPadLeft },
                    { InputActions.Down, (int)Buttons.DPadDown },
                    { InputActions.Up, (int)Buttons.DPadUp },
                    { InputActions.A, (int)Buttons.A },
                    { InputActions.B, (int)Buttons.B },
                    { InputActions.X, (int)Buttons.X },
                    { InputActions.Y, (int)Buttons.Y }
                }
            },
            {
                InputHandler.InputTypes.Joystick, new Dictionary<string, int>(8)
                {
                    { InputActions.Left, 0 },
                    { InputActions.Right, 1 },
                    { InputActions.Up, 2 },
                    { InputActions.Down, 3 },
                    { InputActions.A, 4 },
                    { InputActions.B, 5 },
                    { InputActions.X, 6 },
                    { InputActions.Y, 7 }
                }
            }
        };

        #endregion
    }

    /// <summary>
    /// The types of actions you can perform in the game.
    /// </summary>
    public static class InputActions
    {
        public const string Left = "Left";
        public const string Right = "Right";
        public const string Up = "Up";
        public const string Down = "Down";

        public const string A = "A";
        public const string B = "B";
        public const string X = "X";
        public const string Y = "Y";
    }
}
