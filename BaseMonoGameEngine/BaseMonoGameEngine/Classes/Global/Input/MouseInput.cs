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
    /// The buttons on a mouse.
    /// </summary>
    public enum MouseButtons
    {
        Left, Right, Middle, X1, X2
    }

    /// <summary>
    /// A wrapper for obtaining mouse input.
    /// </summary>
    public static class MouseInput
    {
        private static MouseState InputMouse = default(MouseState);

        private static MouseState MState => Mouse.GetState();

        /// <summary>
        /// Gets a ButtonState associated with a MouseButton.
        /// </summary>
        /// <param name="button">The MouseButton to get the ButtonState for.</param>
        /// <param name="mouseState">The MouseState to get the ButtonState for.</param>
        /// <returns>A ButtonState on the mouse for the MouseButton.</returns>
        private static ButtonState GetButtonState(in MouseButtons button, in MouseState mouseState)
        {
            switch (button)
            {
                case MouseButtons.Right: return mouseState.RightButton;
                case MouseButtons.Middle: return mouseState.MiddleButton;
                case MouseButtons.X1: return mouseState.XButton1;
                case MouseButtons.X2: return mouseState.XButton2;
                case MouseButtons.Left:
                default: return mouseState.LeftButton;
            }
        }

        /// <summary>
        /// Tells if a button on the mouse is pressed.
        /// </summary>
        /// <param name="button">The button to test.</param>
        /// <returns>true if the button is pressed, otherwise false.</returns>
        public static bool GetButton(in MouseButtons button)
        {
            return GetButton(button, MState);
        }

        /// <summary>
        /// Tells if a button on a MouseState is pressed.
        /// </summary>
        /// <param name="button">The button to test.</param>
        /// <param name="mouseState">The MouseState to check.</param>
        /// <returns>true if the button on the MouseState is pressed, otherwise false.</returns>
        public static bool GetButton(in MouseButtons button, in MouseState mouseState)
        {
            return (GetButtonState(button, mouseState) == ButtonState.Pressed);
        }

        /// <summary>
        /// Gets if a button on the mouse was just released.
        /// </summary>
        /// <param name="button">The button to test.</param>
        /// <returns>true if the button on the mouse was just released, otherwise false.</returns>
        public static bool GetButtonReleased(in MouseButtons button)
        {
            return GetButtonReleased(button, MState);
        }

        /// <summary>
        /// Gets if a button on a MouseState was just released.
        /// </summary>
        /// <param name="button">The button to test.</param>
        /// <param name="mouseState">The MouseState to check.</param>
        /// <returns>true if the button on the MouseState was just released, otherwise false.</returns>
        public static bool GetButtonReleased(in MouseButtons button, in MouseState mouseState)
        {
            return (GetButtonState(button, mouseState) == ButtonState.Pressed && GetButtonState(button, MState) == ButtonState.Released);
        }

        /// <summary>
        /// Gets if a button on the mouse was just pressed.
        /// </summary>
        /// <param name="button">The button to test.</param>
        /// <returns>true if the button on the mouse was just pressed, otherwise false.</returns>
        public static bool GetButtonPressed(in MouseButtons button)
        {
            return GetButtonPressed(button, MState);
        }

        /// <summary>
        /// Gets if a button on a MouseState was just pressed.
        /// </summary>
        /// <param name="button">The button to test.</param>
        /// <param name="mouseState">The MouseState to check.</param>
        /// <returns>true if the button on the MouseState was just pressed, otherwise false.</returns>
        public static bool GetButtonPressed(in MouseButtons button, in MouseState mouseState)
        {
            return (GetButtonState(button, mouseState) == ButtonState.Released && GetButtonState(button, MState) == ButtonState.Pressed);
        }

        /*#region Left Button

        public static bool GetLeftButton()
        {
            return (MState.LeftButton == ButtonState.Pressed);
        }

        public static bool GetLeftButtonPressed()
        {
            return GetLeftButtonPressed(InputMouse);
        }

        public static bool GetLeftButtonPressed(in MouseState mouseState)
        {
            return (mouseState.LeftButton == ButtonState.Released && MState.LeftButton == ButtonState.Pressed);
        }

        public static bool GetLeftButtonReleased()
        {
            return GetLeftButtonReleased(InputMouse);
        }

        public static bool GetLeftButtonReleased(in MouseState mouseState)
        {
            return (mouseState.LeftButton == ButtonState.Pressed && MState.LeftButton == ButtonState.Released);
        }

        #endregion

        #region Right Button

        public static bool GetRightButton()
        {
            return (MState.RightButton == ButtonState.Pressed);
        }

        public static bool GetRightButtonPressed()
        {
            return GetRightButtonPressed(InputMouse);
        }

        public static bool GetRightButtonPressed(in MouseState mouseState)
        {
            return (mouseState.RightButton == ButtonState.Released && MState.RightButton == ButtonState.Pressed);
        }

        public static bool GetRightButtonReleased()
        {
            return GetRightButtonReleased(InputMouse);
        }

        public static bool GetRightButtonReleased(in MouseState mouseState)
        {
            return (mouseState.RightButton == ButtonState.Pressed && MState.RightButton == ButtonState.Released);
        }

        #endregion

        #region Middle Button

        public static bool GetMiddleButton()
        {
            return (MState.MiddleButton == ButtonState.Pressed);
        }

        public static bool GetMiddleButtonPressed()
        {
            return GetMiddleButtonPressed(InputMouse);
        }

        public static bool GetMiddleButtonPressed(in MouseState mouseState)
        {
            return (mouseState.MiddleButton == ButtonState.Released && MState.MiddleButton == ButtonState.Pressed);
        }

        public static bool GetMiddleButtonReleased()
        {
            return GetMiddleButtonReleased(InputMouse);
        }

        public static bool GetMiddleButtonReleased(in MouseState mouseState)
        {
            return (mouseState.MiddleButton == ButtonState.Pressed && MState.MiddleButton == ButtonState.Released);
        }

        #endregion

        #region XButton1

        public static bool GetXButton1()
        {
            return (MState.XButton1 == ButtonState.Pressed);
        }

        public static bool GetXButton1Pressed()
        {
            return GetXButton1Pressed(InputMouse);
        }

        public static bool GetXButton1Pressed(in MouseState mouseState)
        {
            return (mouseState.XButton1 == ButtonState.Released && MState.XButton1 == ButtonState.Pressed);
        }

        public static bool GetXButton1Released()
        {
            return GetXButton1Released(InputMouse);
        }

        public static bool GetXButton1Released(in MouseState mouseState)
        {
            return (mouseState.XButton1 == ButtonState.Pressed && MState.XButton1 == ButtonState.Released);
        }

        #endregion

        #region XButton2

        public static bool GetXButton2()
        {
            return (MState.XButton2 == ButtonState.Pressed);
        }

        public static bool GetXButton2Pressed()
        {
            return GetXButton2Pressed(InputMouse);
        }

        public static bool GetXButton2Pressed(in MouseState mouseState)
        {
            return (mouseState.XButton2 == ButtonState.Released && MState.XButton2 == ButtonState.Pressed);
        }

        public static bool GetXButton2Released()
        {
            return GetXButton2Released(InputMouse);
        }

        public static bool GetXButton2Released(in MouseState mouseState)
        {
            return (mouseState.XButton2 == ButtonState.Pressed && MState.XButton2 == ButtonState.Released);
        }

        #endregion*/

        #region Position and Scroll Wheels

        /// <summary>
        /// Gets the current position of the mouse.
        /// </summary>
        /// <returns>A Point representing the mouse position.</returns>
        public static Point GetMousePos()
        {
            return MState.Position;
        }

        /// <summary>
        /// Gets the amount the mouse moved.
        /// </summary>
        /// <returns>A Point representing the difference of the mouse position.</returns>
        public static Point GetMousePosDiff()
        {
            return GetMousePosDiff(InputMouse);
        }

        /// <summary>
        /// Gets the amount the mouse moved with a MouseState.
        /// </summary>
        /// <returns>A Point representing the difference of the mouse position.</returns>
        public static Point GetMousePosDiff(in MouseState mouseState)
        {
            return (MState.Position - mouseState.Position);
        }

        /// <summary>
        /// Gets the value of the vertical mouse scroll wheel.
        /// </summary>
        /// <returns>An integer representing the vertical scroll wheel value.</returns>
        public static int GetScrollWheel()
        {
            return MState.ScrollWheelValue;
        }

        /// <summary>
        /// Gets the value difference of the vertical mouse scroll wheel.
        /// </summary>
        /// <returns>An integer representing the difference of the vertical scroll wheel value.</returns>
        public static int GetScrollWheelDiff()
        {
            return GetScrollWheelDiff(InputMouse);
        }

        /// <summary>
        /// Gets the value difference of the vertical mouse scroll wheel with a MouseState.
        /// </summary>
        /// <returns>An integer representing the difference of the vertical scroll wheel value.</returns>
        public static int GetScrollWheelDiff(in MouseState mouseState)
        {
            return (MState.ScrollWheelValue - mouseState.ScrollWheelValue);
        }

        /// <summary>
        /// Gets the value of the horizontal mouse scroll wheel.
        /// </summary>
        /// <returns>An integer representing the horizontal scroll wheel value.</returns>
        public static int GetHorizontalWheel()
        {
            return MState.HorizontalScrollWheelValue;
        }

        /// <summary>
        /// Gets the value difference of the horizontal mouse scroll wheel.
        /// </summary>
        /// <returns>An integer representing the difference of the horizontal scroll wheel value.</returns>
        public static int GetHorizontalWheelDiff()
        {
            return GetHorizontalWheelDiff(InputMouse);
        }

        /// <summary>
        /// Gets the value difference of the horizontal mouse scroll wheel with a MouseState.
        /// </summary>
        /// <returns>An integer representing the difference of the horizontal scroll wheel value.</returns>
        public static int GetHorizontalWheelDiff(in MouseState mouseState)
        {
            return (MState.HorizontalScrollWheelValue - mouseState.HorizontalScrollWheelValue);
        }

        #endregion

        /// <summary>
        /// Updates the mouse.
        /// </summary>
        public static void UpdateMouseState()
        {
            UpdateMouseState(ref InputMouse);
        }

        /// <summary>
        /// Updates a MouseState.
        /// </summary>
        /// <param name="mouseState">The MouseState to update.</param>
        public static void UpdateMouseState(ref MouseState mouseState)
        {
            mouseState = MState;
        }
    }
}
