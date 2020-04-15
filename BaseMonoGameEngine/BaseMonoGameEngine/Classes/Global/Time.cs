using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TDMonoGameEngine
{
    /// <summary>
    /// Handles all time.
    /// </summary>
    public static class Time
    {
        /// <summary>
        /// The number of milliseconds in a second.
        /// </summary>
        public const double MsPerS = 1000d;

        /// <summary>
        /// A TimeSpan representing the total amount of time since the game booted up.
        /// </summary>
        public static TimeSpan TotalTime { get; private set; } = default(TimeSpan);

        /// <summary>
        /// A TimeSpan representing the total amount of elapsed time since the previous frame.
        /// </summary>
        public static TimeSpan ElapsedTime { get; private set; } = default(TimeSpan);

        /// <summary>
        /// The frames per second the game runs at.
        /// </summary>
        public static double FPS = 60d;

        /// <summary>
        /// The total number of game updates since the game booted up.
        /// </summary>
        public static long TotalUpdates { get; private set; } = 0L;

        /// <summary>
        /// The total number of frames drawn since the game booted up.
        /// </summary>
        public static long TotalFrames { get; private set; } = 0L;

        /// <summary>
        /// Determines if the game is running slowly or not.
        /// </summary>
        public static bool RunningSlowly { get; private set; } = false;

        /// <summary>
        /// The current timestep setting.
        /// </summary>
        public static TimestepSettings TimeStep = TimestepSettings.Fixed;

        /// <summary>
        /// The current VSync setting.
        /// </summary>
        public static VSyncSettings VSyncSetting = VSyncSettings.Enabled;

        /// <summary>
        /// The max amount of time to frameskip and perform only updates with no draws.
        /// The elapsed time in a frame cannot exceed this.
        /// </summary>
        public static TimeSpan MaxElapsedTime = new TimeSpan(0, 0, 0, 0, 500);

        /// <summary>
        /// Updates the game time.
        /// </summary>
        /// <param name="gameTime">Provides a snapshop of timing values.</param>
        public static void UpdateTime(in GameTime gameTime)
        {
            TotalTime = gameTime.TotalGameTime;
            ElapsedTime = gameTime.ElapsedGameTime;
            RunningSlowly = gameTime.IsRunningSlowly;

            TotalUpdates++;
        }

        /// <summary>
        /// Updates the number of drawn frames.
        /// </summary>
        public static void UpdateFrames()
        {
            TotalFrames++;
        }

        /// <summary>
        /// Obtains a TimeSpan equivalent to a framerate value.
        /// </summary>
        /// <param name="fpsVal">The framerate value in frames-per-second.</param>
        /// <returns>A TimeSpan equivalent to the framerate value.
        /// If <paramref name="fpsVal"/> is less or equal to 0, <see cref="TimeSpan.Zero"/> is returned.</returns>
        public static TimeSpan GetTimeSpanFromFPS(in double fpsVal)
        {
            //Return TimeSpan.Zero for values less than or equal to 0
            if (fpsVal <= 0d) return TimeSpan.Zero;

            //Round to 7 digits for accuracy
            double val = Math.Round(1d / fpsVal, 7);

            //TimeSpan normally rounds, so to be precise we'll create them from ticks
            return TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond * val));
        }
    }
}
