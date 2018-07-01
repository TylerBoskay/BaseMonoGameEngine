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
        /// Whether in-game time is enabled or not. If set to false, ActiveMilliseconds won't be updated.
        /// </summary>
        public static bool InGameTimeEnabled { get; private set; } = true;

        /// <summary>
        /// The total number of game updates since the game booted up.
        /// </summary>
        public static long TotalUpdates { get; private set; } = 0L;

        /// <summary>
        /// The total number of frames drawn since the game booted up.
        /// </summary>
        public static long TotalFrames { get; private set; } = 0L;

        /// <summary>
        /// The total amount of time, in milliseconds, since the game booted up.
        /// </summary>
        public static double TotalMilliseconds => TotalTime.TotalMilliseconds;

        /// <summary>
        /// The total amount of unpaused or unfrozen time, in milliseconds, since the game booted up.
        /// </summary>
        public static double ActiveTotalMS { get; private set; } = 0d;

        /// <summary>
        /// The amount of time, in milliseconds, since the previous frame.
        /// </summary>
        public static double ElapsedMilliseconds => ElapsedTime.TotalMilliseconds;

        /// <summary>
        /// The amount of unpaused or unfrozen time, in milliseconds, since the previous frame.
        /// </summary>
        public static double ActiveElapsedMS => (InGameTimeEnabled == true) ? ElapsedTime.TotalMilliseconds : 0d;

        /// <summary>
        /// Determines if the game is running slowly or not.
        /// </summary>
        public static bool RunningSlowly { get; private set; } = false;

        /// <summary>
        /// Whether the game is run at a fixed interval or not.
        /// </summary>
        public static bool FixedTimeStep = true;

        /// <summary>
        /// Whether VSync is enabled or not.
        /// </summary>
        public static bool VSyncEnabled = true;

        /// <summary>
        /// Enables or disables in-game time. If false, <see cref="ActiveTotalMS"/> will not be updated.
        /// </summary>
        /// <param name="ingameTimeEnabled">Whether to enable in-game time or not.</param>
        public static void ToggleInGameTime(in bool ingameTimeEnabled)
        {
            InGameTimeEnabled = ingameTimeEnabled;
        }

        /// <summary>
        /// Updates the game time.
        /// </summary>
        /// <param name="gameTime">Provides a snapshop of timing values.</param>
        public static void UpdateTime(in GameTime gameTime)
        {
            TotalTime = gameTime.TotalGameTime;
            ElapsedTime = gameTime.ElapsedGameTime;
            RunningSlowly = gameTime.IsRunningSlowly;

            if (InGameTimeEnabled == true)
            {
                ActiveTotalMS += ElapsedTime.TotalMilliseconds;
            }

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
