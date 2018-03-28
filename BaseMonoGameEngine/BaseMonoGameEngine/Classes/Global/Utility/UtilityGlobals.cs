using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TDMonoGameEngine
{
    /// <summary>
    /// Class for global utility functions
    /// </summary>
    public static class UtilityGlobals
    {
        public static readonly double TwoPI = (Math.PI * 2d);
        public static readonly double HalfPI = (Math.PI / 2d);

        public static int Clamp(int value, int min, int max) => (value < min) ? min : (value > max) ? max : value;
        public static float Clamp(float value, float min, float max) => (value < min) ? min : (value > max) ? max : value;
        public static double Clamp(double value, double min, double max) => (value < min) ? min : (value > max) ? max : value;
        public static uint Clamp(uint value, uint min, uint max) => (value < min) ? min : (value > max) ? max : value;

        public static int Wrap(int value, int min, int max) => (value < min) ? max : (value > max) ? min : value;
        public static float Wrap(float value, float min, float max) => (value < min) ? max : (value > max) ? min : value;
        public static double Wrap(double value, double min, double max) => (value < min) ? max : (value > max) ? min : value;

        public static T Min<T>(T val1, T val2) where T : IComparable => (val1.CompareTo(val2) < 0) ? val1 : (val2.CompareTo(val1) < 0) ? val2 : val1;
        public static T Max<T>(T val1, T val2) where T : IComparable => (val1.CompareTo(val2) > 0) ? val1 : (val2.CompareTo(val1) > 0) ? val2 : val1;

        public static float ToDegrees(in float radians) => Microsoft.Xna.Framework.MathHelper.ToDegrees(radians);
        public static float ToRadians(in float degrees) => Microsoft.Xna.Framework.MathHelper.ToRadians(degrees);

        public static double ToDegrees(in double radians) => (radians * (180d / Math.PI));
        public static double ToRadians(in double degrees) => (degrees * (Math.PI / 180d));

        public static int Lerp(int value1, int value2, float amount) => value1 + (int)((value2 - value1) * amount);
        public static float Lerp(float value1, float value2, float amount) => value1 + ((value2 - value1) * amount);
        public static double Lerp(double value1, double value2, float amount) => value1 + ((value2 - value1) * amount);

        public static double LerpPrecise(double value1, double value2, float amount) => ((1 - amount) * value1) + (value2 * amount);
        public static float LerpPrecise(float value1, float value2, float amount) => ((1 - amount) * value1) + (value2 * amount);
        public static int LerpPrecise(int value1, int value2, float amount) => (int)(((1 - amount) * value1) + (value2 * amount));

        /// <summary>
        /// Bounces a value between 0 and a max value.
        /// </summary>
        /// <param name="time">The time value.</param>
        /// <param name="maxVal">The max value.</param>
        /// <returns>A double with a value between 0 and <paramref name="maxVal"/>.</returns>
        public static double PingPong(double time, double maxVal)
        {
            double lengthTimesTwo = maxVal * 2d;
            double timeMod = time % lengthTimesTwo;

            if (timeMod >= 0 && timeMod < maxVal)
                return timeMod;
            else
                return lengthTimesTwo - timeMod;
        }

        /// <summary>
        /// Bounces a value between 0 and a max value.
        /// </summary>
        /// <param name="time">The time value.</param>
        /// <param name="maxVal">The max value.</param>
        /// <returns>A float with a value between 0 and <paramref name="maxVal"/>.</returns>
        public static float PingPong(double time, float maxVal) => (float)PingPong(time, (double)maxVal);

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <remarks>This is a more verbose but readable version of MonoGame's Hermite.
        /// It returns a double rather than a float.</remarks>
        /// <param name="value1">The startpoint of the curve.</param>
        /// <param name="tangent1">Initial tangent; the direction and speed to how the curve leaves the startpoint.</param>
        /// <param name="value2">The endpoint of the curve.</param>
        /// <param name="tangent2">End tangent; the direction and speed to how the curve leaves the endpoint.</param>
        /// <param name="amount">Weighting factor; between 0 and 1.</param>
        /// <returns>A double representing the result of the Hermite spline interpolation.</returns>
        public static double Hermite(float value1, float tangent1, float value2, float tangent2, float amount)
        {
            /*Hermite basis functions:
             * s = amount (or time)
             * h1 = 2s^3 - 3s^2 + 1
             * h2 = -2s^3 + 3s^2
             * h3 = s^3 - 2s^2 + s
             * h4 = s^3 - s^2
             * 
             * The values are multiplied by the basis functions and added together like so:
             * result = (h1 * val1) + (h2 * val2) + (h3 * tan1) + (h4 * tan2);
            */

            double val1 = value1;
            double val2 = value2;
            double tan1 = tangent1;
            double tan2 = tangent2;
            double amt = amount;
            double result = 0d;

            //Define cube and squares
            double amtCubed = amt * amt * amt;
            double amtSquared = amt * amt;

            //If 0, return the initial value
            if (amount == 0f)
            {
                result = value1;
            }
            //If 1, return the 
            else if (amount == 1f)
            {
                result = value2;
            }
            else
            {
                //Define hermite functions
                //double h1 = (2 * amtCubed) - (3 * amtSquared) + 1;
                //double h2 = (-2 * amtCubed) + (3 * amtSquared);
                //double h3 = amtCubed - (2 * amtSquared) + amt;
                //double h4 = amtCubed - amtSquared;

                //Multiply the results
                //result = (h1 * val1) + (h2 * val2) + (h3 * tan1) + (h4 * tan2);

                //Condensed
                result =
                    (((2 * val1) - (2 * val2) + tan2 + tan1) * amtCubed) +
                    (((3 * val2) - (3 * val1) - (2 * tan1) - tan2) * amtSquared) +
                    (tan1 * amt) + val1;
            }

            return result;
        }

        /// <summary>
        /// Interpolates a value by weighted average.
        /// The closer the value gets to the target, the slower it moves.
        /// </summary>
        /// <param name="curVal">The current value.</param>
        /// <param name="targetVal">The target value.</param>
        /// <param name="slowdownFactor">The slowdown factor. The higher this is, the slower <paramref name="curVal"/> will approach <paramref name="targetVal"/>.</param>
        /// <returns>A double representing the weighted average interpolation.</returns>
        public static double WeightedAverageInterpolation(double curVal, double targetVal, double slowdownFactor)
        {
            //Avoid division by 0
            if (slowdownFactor == 0)
            {
                return targetVal;
            }

            return ((curVal * (slowdownFactor - 1)) + targetVal) / slowdownFactor;
        }

        /// <summary>
        /// Swaps two references of the same Type.
        /// </summary>
        /// <typeparam name="T">The Type of the objects to swap.</typeparam>
        /// <param name="obj1">The first object to swap.</param>
        /// <param name="obj2">The second object to swap.</param>
        public static void Swap<T>(ref T obj1, ref T obj2)
        {
            T temp = obj1;
            obj1 = obj2;
            obj2 = temp;
        }

        /// <summary>
        /// Gets the tangent angle between two Vector2s in radians. This value is between -π and π. 
        /// </summary>
        /// <param name="vec1">The first vector2.</param>
        /// <param name="vec2">The second vector.</param>
        /// <returns>A double representing the tangent angle between the two vectors, in radians.</returns>
        public static double TangentAngle(in Vector2 vec1, in Vector2 vec2) => Math.Atan2(vec2.Y - vec1.Y, vec2.X - vec1.X);

        /// <summary>
        /// Gets the cosign angle between two Vector2s in radians.
        /// </summary>
        /// <param name="vec1">The first vector.</param>
        /// <param name="vec2">The second vector.</param>
        /// <returns>A double representing the cosign angle between the two vectors, in radians.</returns>
        public static double CosignAngle(in Vector2 vec1, in Vector2 vec2)
        {
            //a · b = (a.X * b.X) + (a.Y * b.Y) = ||a|| * ||b|| * cos(θ)
            double dotProduct = Vector2.Dot(vec1, vec2);
            double mag1 = vec1.Length();
            double mag2 = vec2.Length();

            double magMult = mag1 * mag2;

            double div = dotProduct / magMult;

            double angleRadians = Math.Acos(div);
            
            return angleRadians;
        }

        /// <summary>
        /// Gets the cosign angle between two Vector2s in degrees.
        /// </summary>
        /// <param name="vec1">The first vector.</param>
        /// <param name="vec2">The second vector.</param>
        /// <returns>A double representing the cosign angle between the two vectors, in degrees.</returns>
        public static double CosignAngleDegrees(in Vector2 vec1, in Vector2 vec2) => ToDegrees(CosignAngle(vec1, vec2));

        /// <summary>
        /// Obtains the 2D cross product result of two Vector2s.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>A float representing the 2D cross product result between the two Vectors.</returns>
        public static double Cross(in Vector2 vector1, in Vector2 vector2)
        {
            //a x b = ((a.y * b.z) - (a.z * b.y), (a.z * b.x) - (a.x * b.z), (a.x * b.y) - (a.y * b.x))
            //The Z component is the only one that remains since we're dealing with Vector2s
            return (vector1.X * vector2.Y) - (vector1.Y * vector2.X);
        }

        /// <summary>
        /// Gets the sine angle between two Vector2s in radians.
        /// </summary>
        /// <param name="vec1">The first vector.</param>
        /// <param name="vec2">The second vector.</param>
        /// <returns>A double representing the sine angle between the two vectors, in radians.</returns>
        public static double SineAngle(in Vector2 vec1, in Vector2 vec2)
        {
            //||a x b|| = ||a|| * ||b|| * sin(θ)
            double crossMag = Cross(vec1, vec2);

            double mag1 = vec1.Length();
            double mag2 = vec2.Length();

            double magMult = mag1 * mag2;

            double div = crossMag / magMult;

            double angleRadians = Math.Asin(div);

            return angleRadians;
        }

        /// <summary>
        /// Finds a point around a circle at a particular angle.
        /// </summary>
        /// <param name="circle">The Circle.</param>
        /// <param name="angle">The angle of the point.</param>
        /// <param name="angleInDegrees">Whether the angle passed in is in degrees or not.</param>
        /// <returns>A Vector2 with the X and Y components at the location around the circle.</returns>
        public static Vector2 GetPointAroundCircle(in Circle circle, double angle, bool angleInDegrees)
        {
            //If the angle is in degrees, convert it to radians
            if (angleInDegrees == true)
            {
                angle = ToRadians(angle);
            }

            return circle.GetPointAround(angle);
        }

        public static T[] GetEnumValues<T>()
        {
            return EnumUtility.GetValues<T>.EnumValues;
        }

        public static string[] GetEnumNames<T>()
        {
            return EnumUtility.GetNames<T>.EnumNames;
        }

        /// <summary>
        /// Indicates whether an <see cref="IList{T}"/> is null or empty.
        /// </summary>
        /// <typeparam name="T">The Type of the elements in the IList.</typeparam>
        /// <param name="iList">The IList.</param>
        /// <returns>true if <paramref name="iList"/> is null or empty, otherwise false.</returns>
        public static bool IListIsNullOrEmpty<T>(IList<T> iList)
        {
            return (iList == null || iList.Count == 0);
        }

        #region Flag Check Utilities

        /* Adding flags: flag1 |= flag2            ; 10 | 01 = 11
         * Checking flags: (flag1 & flag2) != 0    ; 11 & 10 = 10
         * Removing flags: (flag1 & (~flag2))      ; 1111 & (~0010) = 1111 & 1101 = 1101
         * */

        #endregion

        /// <summary>
        /// Subtracts a float from another float and divides the result by a dividing factor.
        /// </summary>
        /// <param name="value1">The float value which has its value subtracted by <paramref name="value2"/>.</param>
        /// <param name="value2">The float value used in the subtraction.</param>
        /// <returns>The difference between <paramref name="value2"/> and <paramref name="value1"/> divided by the
        /// <paramref name="dividingFactor"/>.
        /// If <paramref name="dividingFactor"/> is 0, then 0.
        /// </returns>
        public static float DifferenceDivided(float value1, float value2, float dividingFactor)
        {
            //Return 0 if we're trying to divide by 0
            if (dividingFactor == 0f)
            {
                return 0f;
            }

            //Return the difference over the division
            float diff = (value1 - value2);
            return diff / dividingFactor;
        }

        /// <summary>
        /// Creates a Rectangle from two Vector2s representing the position and scale of the Rectangle.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Rectangle CreateRect(in Vector2 position, in Vector2 scale)
        {
            return new Rectangle((int)position.X, (int)position.Y, (int)scale.X, (int)scale.Y);
        }

        /// <summary>
        /// Returns the squared distance to a point.
        /// </summary>
        /// <param name="rectTopLeft">The top-left point of the Rectangle.</param>
        /// <param name="rectBottomRight">The bottom-right point of the Rectangle.</param>
        /// <param name="point">The point to get the distance to.</param>
        /// <returns>A double containing the squared distance to the point.</returns>
        public static double SquaredDistanceToPointFromRectangle(in Vector2 rectTopLeft, in Vector2 rectBottomRight, in Vector2 point)
        {
            double squaredDistance = 0d;

            //X-axis - See whether we get the distance from the left or right point of the rectangle
            if (point.X < rectTopLeft.X)
            {
                float distance = rectTopLeft.X - point.X;

                squaredDistance += (distance * distance);
            }
            else if (point.X > rectBottomRight.X)
            {
                float distance = rectBottomRight.X - point.X;

                squaredDistance += (distance * distance);
            }

            //Y-axis - See whether we get the distance from the top or bottom point of the rectangle
            if (point.Y < rectTopLeft.Y)
            {
                float distance = rectTopLeft.Y - point.Y;

                squaredDistance += (distance * distance);
            }
            else if (point.Y > rectBottomRight.Y)
            {
                float distance = rectBottomRight.Y - point.Y;

                squaredDistance += (distance * distance);
            }

            return squaredDistance;
        }
    }
}
