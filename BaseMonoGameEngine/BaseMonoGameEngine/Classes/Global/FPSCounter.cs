using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// FPS Counter that displays the FPS
    /// </summary>
    public static class FPSCounter
    {
        /// <summary>
        /// The amount of time, in milliseconds, to wait before updating the FPS display
        /// </summary>
        private static double UpdateInterval = 1000d;

        /// <summary>
        /// The current frame rate.
        /// </summary>
        private static double FPSValue = 0d;
        private static double PrevUpdateVal = 0d;

        /// <summary>
        /// The number of updates that have been performed.
        /// </summary>
        private static int Updates = 0;

        /// <summary>
        /// The number of frames that have been drawn.
        /// </summary>
        private static int Frames = 0;

        private static string FPSText = string.Empty;

        public static void Update()
        {
            PrevUpdateVal += Time.ElapsedTime.TotalMilliseconds;

            //Check if we should update the FPS value displayed
            if (PrevUpdateVal >= UpdateInterval)
            {
                if (UpdateInterval <= 0d)
                {
                    FPSValue = 0d;
                }
                else
                {
                    //Our FPS is how many frames passed in the update interval
                    FPSValue = (Frames / UpdateInterval) * Time.MsPerS;
                }

                FPSText = $"{FPSValue.ToString()} FPS  Updates: {Updates}  Draws: {Frames}";

                Updates = 0;
                Frames = 0;
                PrevUpdateVal = 0d;
            }

            //Count each update
            Updates++;
        }

        public static void Draw()
        {
            SpriteFont font = AssetManager.Instance.LoadFont("Font");

            if (FPSText != string.Empty)
            {
                Debug.DebugUIBatch?.DrawString(font, FPSText, Vector2.Zero, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, .5f);
            }
            if (Time.RunningSlowly == true)
            {
                Debug.DebugUIBatch?.DrawString(font, "RUNNING SLOW!", new Vector2(0f, 20f), Color.Red, 0f, Vector2.Zero, 1f, SpriteEffects.None, .5f);
            }

            //Count each frame
            Frames++;
        }
    }
}
