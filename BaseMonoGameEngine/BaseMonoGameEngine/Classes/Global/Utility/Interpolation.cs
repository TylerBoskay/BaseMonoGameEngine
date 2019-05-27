using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TDMonoGameEngine
{
    /// <summary>
    /// A class containing useful interpolation methods.
    /// </summary>
    public static class Interpolation
    {
        /// <summary>
        /// The types of interpolation.
        /// </summary>
        public enum InterpolationTypes
        {
            Linear,
            QuadIn,
            QuadOut,
            QuadInOut,
            CubicIn,
            CubicOut,
            CubicInOut,
            QuartIn,
            QuartOut,
            QuartInOut,
            QuintIn,
            QuintOut,
            QuintInOut,
            ExponentialIn,
            ExponentialOut,
            ExponentialInOut,
            SineIn,
            SineOut,
            SineInOut,
            ElasticIn,
            ElasticOut,
            ElasticInOut
        }

        /// <summary>
        /// A delegate for an interpolation method.
        /// This should adjust the time value based on the interpolation, which will be used in the interpolation method.
        /// </summary>
        /// <param name="time">The time, from 0 to 1.</param>
        /// <returns>A double with the adjusted time value.</returns>
        public delegate double InterpolationMethod(in double time);

        #region Internal Time Calculations

        //These private methods will calculate the time, while the public methods will return values.
        //They are based off Robert Penner's easing equations in Actionscript found here: http://gizma.com/easing/
        //Some methods sourced from: https://github.com/acron0/Easings/blob/master/Easings.cs and https://joshondesign.com/2013/03/01/improvedEasingEquations with modifications

        private static InterpolationMethod Linear = LinearTime;
        private static double LinearTime(in double time)
        {
            return time;
        }

        private static InterpolationMethod EaseInQuad = EaseInQuadTime;
        private static double EaseInQuadTime(in double time)
        {
            return Math.Pow(time, 2);
        }

        private static InterpolationMethod EaseOutQuad = EaseOutQuadTime;
        private static double EaseOutQuadTime(in double time)
        {
            return (1 - EaseInQuadTime(1 - time));
        }

        private static InterpolationMethod EaseInOutQuad = EaseInOutQuadTime;
        private static double EaseInOutQuadTime(in double time)
        {
            if (time < .5d) return (EaseInQuadTime(time * 2d) / 2d);
            return (1 - (EaseInQuadTime((1 - time) * 2d) / 2d));
        }

        private static InterpolationMethod EaseInCubic = EaseInCubicTime;
        private static double EaseInCubicTime(in double time)
        {
            return Math.Pow(time, 3);
        }

        private static InterpolationMethod EaseOutCubic = EaseOutCubicTime;
        private static double EaseOutCubicTime(in double time)
        {
            return (1 - EaseInCubicTime(1 - time));
        }

        private static InterpolationMethod EaseInOutCubic = EaseInOutCubicTime;
        private static double EaseInOutCubicTime(in double time)
        {
            if (time < .5d) return (EaseInCubicTime(time * 2d) / 2d);
            return (1 - (EaseInCubicTime((1 - time) * 2d) / 2d));
        }

        private static InterpolationMethod EaseInQuart = EaseInQuartTime;
        private static double EaseInQuartTime(in double time)
        {
            return Math.Pow(time, 4);
        }

        private static InterpolationMethod EaseOutQuart = EaseOutQuartTime;
        private static double EaseOutQuartTime(in double time)
        {
            return (1 - EaseInQuartTime(1 - time));
        }

        private static InterpolationMethod EaseInOutQuart = EaseInOutQuartTime;
        private static double EaseInOutQuartTime(in double time)
        {
            if (time < .5d) return (EaseInQuartTime(time * 2d) / 2d);
            return (1 - (EaseInQuartTime((1 - time) * 2d) / 2d));
        }

        private static InterpolationMethod EaseInQuint = EaseInQuintTime;
        private static double EaseInQuintTime(in double time)
        {
            return Math.Pow(time, 5);
        }

        private static InterpolationMethod EaseOutQuint = EaseOutQuintTime;
        private static double EaseOutQuintTime(in double time)
        {
            return (1 - EaseInQuintTime(1 - time));
        }

        private static InterpolationMethod EaseInOutQuint = EaseInOutQuintTime;
        private static double EaseInOutQuintTime(in double time)
        {
            if (time < .5d) return (EaseInQuintTime(time * 2d) / 2d);
            return (1 - (EaseInQuintTime((1 - time) * 2d) / 2d));
        }

        private static InterpolationMethod EaseInExponential = EaseInExponentialTime;
        private static double EaseInExponentialTime(in double time)
        {
            //Exponential gets close to the starting value, but not to it
            if (time == 0d) return time;
            return Math.Pow(2, 10 * (time - 1));
        }

        private static InterpolationMethod EaseOutExponential = EaseOutExponentialTime;
        private static double EaseOutExponentialTime(in double time)
        {
            //Exponential gets close to the final value, but not to it
            if (time == 1d) return time;
            return -Math.Pow(2, -10 * time) + 1;
        }

        private static InterpolationMethod EaseInOutExponential = EaseInOutExponentialTime;
        private static double EaseInOutExponentialTime(in double time)
        {
            //Exponential gets close to the starting and final values, but not to them
            if (time == 0d || time == 1d) return time;

            if (time < .5d) return (.5d * Math.Pow(2, (20 * time) - 10));
            return (.5d * -Math.Pow(2, (-20 * time) + 10)) + 1;
        }

        private static InterpolationMethod EaseInSine = EaseInSineTime;
        private static double EaseInSineTime(in double time)
        {
            return Math.Sin((time - 1) * UtilityGlobals.HalfPI) + 1;
        }

        private static InterpolationMethod EaseOutSine = EaseOutSineTime;
        private static double EaseOutSineTime(in double time)
        {
            return Math.Sin(time * UtilityGlobals.HalfPI);
        }

        private static InterpolationMethod EaseInOutSine = EaseInOutSineTime;
        private static double EaseInOutSineTime(in double time)
        {
            return (.5d * (1 - Math.Cos(time * Math.PI)));
        }

        private static InterpolationMethod EaseInElastic = EaseInElasticTime;
        private static double EaseInElasticTime(in double time)
        {
            return (Math.Sin(13 * (UtilityGlobals.HalfPI * time)) * Math.Pow(2, 10 * (time - 1)));
        }

        private static InterpolationMethod EaseOutElastic = EaseOutElasticTime;
        private static double EaseOutElasticTime(in double time)
        {
            return (Math.Sin(-13 * (UtilityGlobals.HalfPI * (time + 1))) * Math.Pow(2, -10 * time)) + 1;
        }

        private static InterpolationMethod EaseInOutElastic = EaseInOutElasticTime;
        private static double EaseInOutElasticTime(in double time)
        {
            if (time < 0.5d)
            {
                return 0.5d * (Math.Sin(13 * (UtilityGlobals.HalfPI * (2 * time))) * Math.Pow(2, 10 * ((2 * time) - 1)));
            }
            else
            {
                return 0.5f * (Math.Sin(-13 * UtilityGlobals.HalfPI * ((2 * time - 1) + 1)) * Math.Pow(2, -10 * (2 * time - 1)) + 2);
            }
        }

        #endregion

        /// <summary>
        /// Gets the appropriate interpolation method based on the InterpolationType passed in.
        /// </summary>
        /// <param name="interpolationType">The InterpolationTypes to get.</param>
        /// <returns>The interpolation method associated with the InterpolationType, otherwise null.</returns>
        private static InterpolationMethod GetInterpolationFromType(in InterpolationTypes interpolationType)
        {
            switch (interpolationType)
            {
                case InterpolationTypes.Linear: return Linear;
                case InterpolationTypes.QuadIn: return EaseInQuad;
                case InterpolationTypes.QuadOut: return EaseOutQuad;
                case InterpolationTypes.QuadInOut: return EaseInOutQuad;
                case InterpolationTypes.CubicIn: return EaseInCubic;
                case InterpolationTypes.CubicOut: return EaseOutCubic;
                case InterpolationTypes.CubicInOut: return EaseInOutCubic;
                case InterpolationTypes.QuartIn: return EaseInQuart;
                case InterpolationTypes.QuartOut: return EaseOutQuart;
                case InterpolationTypes.QuartInOut: return EaseInOutQuart;
                case InterpolationTypes.QuintIn: return EaseInQuint;
                case InterpolationTypes.QuintOut: return EaseOutQuint;
                case InterpolationTypes.QuintInOut: return EaseInOutQuint;
                case InterpolationTypes.ExponentialIn: return EaseInExponential;
                case InterpolationTypes.ExponentialOut: return EaseOutExponential;
                case InterpolationTypes.ExponentialInOut: return EaseInOutExponential;
                case InterpolationTypes.SineIn: return EaseInSine;
                case InterpolationTypes.SineOut: return EaseOutSine;
                case InterpolationTypes.SineInOut: return EaseInOutSine;
                case InterpolationTypes.ElasticIn: return EaseInElastic;
                case InterpolationTypes.ElasticOut: return EaseOutElastic;
                case InterpolationTypes.ElasticInOut: return EaseInOutElastic;
                default: return null;
            }
        }

        /// <summary>
        /// Performs interpolation based on the InterpolationType.
        /// </summary>
        /// <param name="startVal">The starting value.</param>
        /// <param name="endVal">The ending value.</param>
        /// <param name="time">The time, between 0 and 1.</param>
        /// <param name="interpolationType">The type of interpolation.</param>
        /// <returns>A double in the range of <paramref name="startVal"/> and <paramref name="endVal"/> based on the interpolation type.</returns>
        public static double Interpolate(in double startVal, in double endVal, in double time, in InterpolationTypes interpolationType)
        {
            return CustomInterpolate(startVal, endVal, time, GetInterpolationFromType(interpolationType));
        }

        /// <summary>
        /// Performs interpolation with an interpolation method.
        /// </summary>
        /// <param name="startVal">The starting value.</param>
        /// <param name="endVal">The ending value.</param>
        /// <param name="time">The time, between 0 and 1.</param>
        /// <param name="interpolateMethod">The interpolation method.</param>
        /// <returns>A double in the range of <paramref name="startVal"/> and <paramref name="endVal"/> based on the interpolation method.</returns>
        public static double CustomInterpolate(in double startVal, in double endVal, in double time, in InterpolationMethod interpolateMethod)
        {
            //Return the starting value if the interpolation method is null
            if (interpolateMethod == null) return startVal;

            double diff = endVal - startVal;
            double interpolateVal = interpolateMethod(time);
            return startVal + (interpolateVal * diff);
        }

        #region Overloads

        /// <summary>
        /// Performs interpolation based on the InterpolationType.
        /// </summary>
        /// <param name="startVal">The starting value.</param>
        /// <param name="endVal">The ending value.</param>
        /// <param name="time">The time, between 0 and 1.</param>
        /// <param name="interpolationType">The type of interpolation.</param>
        /// <returns>An int in the range of <paramref name="startVal"/> and <paramref name="endVal"/> based on the interpolation type.</returns>
        public static int Interpolate(in int startVal, in int endVal, in double time, in InterpolationTypes interpolationType)
        {
            return (int)CustomInterpolate(startVal, endVal, time, GetInterpolationFromType(interpolationType));
        }

        /// <summary>
        /// Performs interpolation based on the InterpolationType.
        /// </summary>
        /// <param name="startVal">The starting value.</param>
        /// <param name="endVal">The ending value.</param>
        /// <param name="time">The time, between 0 and 1.</param>
        /// <param name="interpolationType">The type of interpolation.</param>
        /// <returns>A float in the range of <paramref name="startVal"/> and <paramref name="endVal"/> based on the interpolation type.</returns>
        public static float Interpolate(in float startVal, in float endVal, in double time, in InterpolationTypes interpolationType)
        {
            return (float)CustomInterpolate(startVal, endVal, time, GetInterpolationFromType(interpolationType));
        }

        /// <summary>
        /// Performs interpolation based on the InterpolationType.
        /// </summary>
        /// <param name="startVal">The starting value.</param>
        /// <param name="endVal">The ending value.</param>
        /// <param name="time">The time, between 0 and 1.</param>
        /// <param name="interpolationType">The type of interpolation.</param>
        /// <returns>A byte in the range of <paramref name="startVal"/> and <paramref name="endVal"/> based on the interpolation type.</returns>
        public static byte Interpolate(in byte startVal, in byte endVal, in double time, in InterpolationTypes interpolationType)
        {
            return (byte)CustomInterpolate(startVal, endVal, time, GetInterpolationFromType(interpolationType));
        }

        /// <summary>
        /// Performs interpolation between Vector2s based on the InterpolationType.
        /// </summary>
        /// <param name="startVal">The starting Vector2 value.</param>
        /// <param name="endVal">The ending Vector2 value.</param>
        /// <param name="time">The time, between 0 and 1.</param>
        /// <param name="interpolationType">The type of interpolation.</param>
        /// <returns>A Vector2 in the range of <paramref name="startVal"/> and <paramref name="endVal"/> based on the interpolation type.</returns>
        public static Vector2 Interpolate(in Vector2 startVal, in Vector2 endVal, in double time, in InterpolationTypes interpolationType)
        {
            float x = Interpolate(startVal.X, endVal.X, time, interpolationType);
            float y = Interpolate(startVal.Y, endVal.Y, time, interpolationType);

            return new Vector2(x, y);
        }

        /// <summary>
        /// Performs interpolation between Vector2s based on the InterpolationTypes.
        /// </summary>
        /// <param name="startVal">The starting Vector2 value.</param>
        /// <param name="endVal">The ending Vector2 value.</param>
        /// <param name="time">The time, between 0 and 1.</param>
        /// <param name="xInterpolationType">The type of interpolation for the X value.</param>
        /// <param name="yInterpolationType">The type of interpolation for the Y value.</param>
        /// <returns>A Vector2 in the range of <paramref name="startVal"/> and <paramref name="endVal"/> based on the X and Y interpolation types.</returns>
        public static Vector2 Interpolate(in Vector2 startVal, in Vector2 endVal, in double time, in InterpolationTypes xInterpolationType, in InterpolationTypes yInterpolationType)
        {
            float x = Interpolate(startVal.X, endVal.X, time, xInterpolationType);
            float y = Interpolate(startVal.Y, endVal.Y, time, yInterpolationType);

            return new Vector2(x, y);
        }

        /// <summary>
        /// Performs interpolation between Colors based on the InterpolationType.
        /// </summary>
        /// <param name="startVal">The starting Color value.</param>
        /// <param name="endVal">The ending Color value.</param>
        /// <param name="time">The time, between 0 and 1.</param>
        /// <param name="interpolationType">The type of interpolation.</param>
        /// <returns>A Color in the range of <paramref name="startVal"/> and <paramref name="endVal"/> based on the interpolation type.</returns>
        public static Color Interpolate(Color startVal, Color endVal, in double time, in InterpolationTypes interpolationType)
        {
            byte r = Interpolate(startVal.R, endVal.R, time, interpolationType);
            byte g = Interpolate(startVal.G, endVal.G, time, interpolationType);
            byte b = Interpolate(startVal.B, endVal.B, time, interpolationType);
            byte a = Interpolate(startVal.A, endVal.A, time, interpolationType);

            return new Color(r, g, b, a);
        }

        #endregion

        #region Inverse

        /// <summary>
        /// Calculates the time of a linear interpolation.
        /// </summary>
        /// <param name="startVal">The starting value.</param>
        /// <param name="endVal">The ending value.</param>
        /// <param name="value">The current value between <paramref name="startVal"/> and <paramref name="endVal"/>.</param>
        /// <returns>A double between 0 and 1.</returns>
        public static double InverseLerp(in double startVal, in double endVal, in double value)
        {
            double diff = endVal - startVal;

            if (Math.Abs(diff) < double.Epsilon) return startVal;

            return (value - startVal) / diff;
        }

        #endregion
    }
}
