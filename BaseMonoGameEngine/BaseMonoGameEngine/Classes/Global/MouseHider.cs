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
    /// Indicates that the mouse cursor should be hidden if it hasn't been moved after a certain amount of time.
    /// </summary>
    public static class MouseHider
    {
        private const double MouseHideTime = 1500d;
        private static double ElapsedMouseTime = MouseHideTime;

        private static Point PrevMousePos = Point.Zero;

        /// <summary>
        /// Whether the mouse cursor should be visible or not.
        /// </summary>
        public static bool MouseVisible => (ElapsedMouseTime < MouseHideTime);

        public static void Update()
        {
            MouseState mState = Mouse.GetState();

            //If the position changed, reset the time and update the previous position
            if (mState.Position != PrevMousePos)
            {
                ElapsedMouseTime = 0d;
                PrevMousePos = mState.Position;
            }
            else
            {
                ElapsedMouseTime = UtilityGlobals.Clamp(ElapsedMouseTime + Time.ElapsedTime.TotalMilliseconds, 0d, MouseHideTime);
            }
        }
    }
}
