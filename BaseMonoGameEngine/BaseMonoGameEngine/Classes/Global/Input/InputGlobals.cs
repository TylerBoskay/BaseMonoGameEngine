using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDMonoGameEngine
{
    /// <summary>
    /// Class for global values dealing with input.
    /// </summary>
    public static class InputGlobals
    {
        public const int MinJoystickAxisValue = short.MinValue;
        public const int MaxJoystickAxisValue = short.MaxValue;
    }

    /// <summary>
    /// The types of actions you can perform in the game.
    /// </summary>
    public static class InputActions
    {
        public const string Horizontal = "Horizontal";
        public const string Vertical = "Vertical";

        public const string A = "A";
        public const string B = "B";
        public const string X = "X";
        public const string Y = "Y";
    }
}
