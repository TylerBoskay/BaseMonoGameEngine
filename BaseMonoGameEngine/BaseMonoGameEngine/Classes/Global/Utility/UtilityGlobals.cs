using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// Class for global utility functions
    /// </summary>
    public static class UtilityGlobals
    {
        public static readonly double TwoPI = (Math.PI * 2d);
        public static readonly double HalfPI = (Math.PI / 2d);

        public static int Clamp(in int value, in int min, in int max) => (value < min) ? min : (value > max) ? max : value;
        public static float Clamp(in float value, in float min, in float max) => (value < min) ? min : (value > max) ? max : value;
        public static double Clamp(in double value, in double min, in double max) => (value < min) ? min : (value > max) ? max : value;
        public static uint Clamp(in uint value, in uint min, in uint max) => (value < min) ? min : (value > max) ? max : value;

        public static int Wrap(in int value, in int min, in int max) => (value < min) ? max : (value > max) ? min : value;
        public static float Wrap(in float value, in float min, in float max) => (value < min) ? max : (value > max) ? min : value;
        public static double Wrap(in double value, in double min, in double max) => (value < min) ? max : (value > max) ? min : value;

        public static T Min<T>(in T val1, in T val2) where T : IComparable => (val1.CompareTo(val2) < 0) ? val1 : (val2.CompareTo(val1) < 0) ? val2 : val1;
        public static T Max<T>(in T val1, in T val2) where T : IComparable => (val1.CompareTo(val2) > 0) ? val1 : (val2.CompareTo(val1) > 0) ? val2 : val1;

        public static float ToDegrees(in float radians) => Microsoft.Xna.Framework.MathHelper.ToDegrees(radians);
        public static float ToRadians(in float degrees) => Microsoft.Xna.Framework.MathHelper.ToRadians(degrees);

        public static double ToDegrees(in double radians) => (radians * (180d / Math.PI));
        public static double ToRadians(in double degrees) => (degrees * (Math.PI / 180d));

        public static int Lerp(in int value1, in int value2, in float amount) => value1 + (int)((value2 - value1) * amount);
        public static float Lerp(in float value1, in float value2, in float amount) => value1 + ((value2 - value1) * amount);
        public static double Lerp(in double value1, in double value2, in float amount) => value1 + ((value2 - value1) * amount);

        public static double LerpPrecise(in double value1, in double value2, in float amount) => ((1 - amount) * value1) + (value2 * amount);
        public static float LerpPrecise(in float value1, in float value2, in float amount) => ((1 - amount) * value1) + (value2 * amount);
        public static int LerpPrecise(in int value1, in int value2, in float amount) => (int)(((1 - amount) * value1) + (value2 * amount));

        /// <summary>
        /// Bounces a value between 0 and a max value.
        /// </summary>
        /// <param name="time">The time value.</param>
        /// <param name="maxVal">The max value.</param>
        /// <returns>A double with a value between 0 and <paramref name="maxVal"/>.</returns>
        public static double PingPong(in double time, in double maxVal)
        {
            double lengthTimesTwo = maxVal * 2d;
            double timeMod = time % lengthTimesTwo;

            if (timeMod >= 0 && timeMod < maxVal)
                return timeMod;
            else
                return lengthTimesTwo - timeMod;
        }

        /// <summary>
        /// Bounces a value between a min and a max value.
        /// </summary>
        /// <param name="time">The time value.</param>
        /// <param name="minVal">The min value.</param>
        /// <param name="maxVal">The max value.</param>
        /// <returns>A float with a value between <paramref name="minVal"/> and <paramref name="maxVal"/>.</returns>
        public static double PingPong(in double time, in double minVal, in double maxVal)
        {
            return PingPong(time, maxVal - minVal) + minVal;
        }

        /// <summary>
        /// Bounces a value between 0 and a max value.
        /// </summary>
        /// <param name="time">The time value.</param>
        /// <param name="maxVal">The max value.</param>
        /// <returns>A float with a value between 0 and <paramref name="maxVal"/>.</returns>
        public static float PingPong(in double time, in float maxVal) => (float)PingPong(time, (double)maxVal);

        /// <summary>
        /// Bounces a value between a min and a max value.
        /// </summary>
        /// <param name="time">The time value.</param>
        /// <param name="minVal">The min value.</param>
        /// <param name="maxVal">The max value.</param>
        /// <returns>A float with a value between <paramref name="minVal"/> and <paramref name="maxVal"/>.</returns>
        public static float PingPong(in double time, in float minVal, in float maxVal) => (float)PingPong(time, (double)minVal, (double)maxVal);

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
        public static double Hermite(in float value1, in float tangent1, in float value2, in float tangent2, in float amount)
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
        public static double WeightedAverageInterpolation(in double curVal, in double targetVal, in double slowdownFactor)
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
        public static double CosignAngle(Vector2 vec1, Vector2 vec2)
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
        public static double SineAngle(Vector2 vec1, Vector2 vec2)
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
        public static Vector2 GetPointAroundCircle(Circle circle, double angle, in bool angleInDegrees)
        {
            //If the angle is in degrees, convert it to radians
            if (angleInDegrees == true)
            {
                angle = ToRadians(angle);
            }

            return circle.GetPointAround(angle);
        }

        /// <summary>
        /// Rotates a Vector2 around another Vector2 representing a pivot.
        /// </summary>
        /// <param name="vector">The Vector2 to rotate.</param>
        /// <param name="pivot">A Vector2 representing the pivot to rotate around.</param>
        /// <param name="angle">The rotation angle, in radians.</param>
        /// <returns>A Vector2 rotated around <paramref name="pivot"/> by <paramref name="angle"/>.</returns>
        public static Vector2 RotateVectorAround(in Vector2 vector, in Vector2 pivot, in double angle)
        {
            //Get the X and Y difference
            float xDiff = vector.X - pivot.X;
            float yDiff = vector.Y - pivot.Y;

            //Rotate the vector
            float x = (float)((Math.Cos(angle) * xDiff) - (Math.Sin(angle) * yDiff) + pivot.X);
            float y = (float)((Math.Sin(angle) * xDiff) + (Math.Cos(angle) * yDiff) + pivot.Y);

            return new Vector2(x, y);
        }

        /// <summary>
        /// Indicates whether an <see cref="IList{T}"/> is null or empty.
        /// </summary>
        /// <typeparam name="T">The Type of the elements in the IList.</typeparam>
        /// <param name="iList">The IList.</param>
        /// <returns>true if <paramref name="iList"/> is null or empty, otherwise false.</returns>
        public static bool IListIsNullOrEmpty<T>(in IList<T> iList)
        {
            return (iList == null || iList.Count == 0);
        }

        /// <summary>
        /// Subtracts a float from another float and divides the result by a dividing factor.
        /// </summary>
        /// <param name="value1">The float value which has its value subtracted by <paramref name="value2"/>.</param>
        /// <param name="value2">The float value used in the subtraction.</param>
        /// <returns>The difference between <paramref name="value2"/> and <paramref name="value1"/> divided by the
        /// <paramref name="dividingFactor"/>.
        /// If <paramref name="dividingFactor"/> is 0, then 0.
        /// </returns>
        public static float DifferenceDivided(in float value1, in float value2, in float dividingFactor)
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
        /// <param name="position">A Vector2 representing the position of the Rectangle.</param>
        /// <param name="scale">A Vector2 representing the scale of the Rectangle.</param>
        /// <returns>A <see cref="Rectangle"/> with the position and scale set.</returns>
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

        /// <summary>
        /// Tells if a double is approximately equal to another one.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="comparison">The value to compare to.</param>
        /// <param name="error">The threshold of error to account for.</param>
        /// <returns>true if (<paramref name="value"/> - <paramref name="comparison"/>) is within <paramref name="error"/>.</returns>
        public static bool IsApproximate(in double value, in double comparison, in double error)
        {
            double check = Math.Abs(value - comparison);
            double absError = Math.Abs(error);

            return (check <= absError);
        }

        /// <summary>
        /// Tells if a float is approximately equal to another one.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="comparison">The value to compare to.</param>
        /// <param name="error">The threshold of error to account for.</param>
        /// <returns>true if (<paramref name="value"/> - <paramref name="comparison"/>) is within <paramref name="error"/>.</returns>
        public static bool IsApproximate(in float value, in float comparison, in float error)
        {
            float check = Math.Abs(value - comparison);
            float absError = Math.Abs(error);

            return (check <= absError);
        }

        /// <summary>
        /// Tells if a Vector2 is approximately equal to another.
        /// This compares the X and Y components of the Vector2.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="comparison">The value to compare to.</param>
        /// <param name="error">The threshold of error to account for.</param>
        /// <returns>true if (<paramref name="value"/> - <paramref name="comparison"/>) is within <paramref name="error"/>.</returns>
        public static bool IsApproximate(in Vector2 value, in Vector2 comparison, in float error)
        {
            return (IsApproximate(value.X, comparison.X, error) && IsApproximate(value.Y, comparison.Y, error));
        }

        /// <summary>
        /// Clamps a Rectangle to another, lining it up with the edge of the other in a certain direction and offsetting by a position.
        /// </summary>
        /// <param name="clampDir">The direction to clamp the Rectangle: 0 for Up, 1 for Down, 2 for Left, and 3 for Right.</param>
        /// <param name="posToClamp">The position associated with the clamped Rectangle.</param>
        /// <param name="rectToClamp">The Rectangle to clamp.</param>
        /// <param name="clampedRect">The Rectangle to clamp to.</param>
        /// <returns>A Vector2 containing the clamped position.</returns>
        public static Vector2 ClampRectToOther(in int clampDir, in Vector2 posToClamp, RectangleF rectToClamp, RectangleF clampedRect)
        {
            Vector2 diff = new Vector2((float)Math.Round(rectToClamp.Center.X - posToClamp.X, 2),
                (float)Math.Round(rectToClamp.Center.Y - posToClamp.Y, 2));

            if (clampDir == 0)
            {
                Vector2 val = new Vector2(posToClamp.X, clampedRect.Bottom);
                val.Y += (rectToClamp.Height / 2) - diff.Y;

                return val;
            }
            else if (clampDir == 1)
            {
                Vector2 val = new Vector2(posToClamp.X, clampedRect.Top);
                val.Y -= (rectToClamp.Height / 2) + diff.Y;

                return val;
            }
            else if (clampDir == 2)
            {
                Vector2 val = new Vector2(clampedRect.Right, posToClamp.Y);
                val.X += (rectToClamp.Width / 2) - diff.X;

                return val;
            }
            else
            {
                Vector2 val = new Vector2(clampedRect.Left, posToClamp.Y);
                val.X -= (rectToClamp.Width / 2) + diff.X;

                return val;
            }
        }

        /// <summary>
        /// Returns points for a regular polygon with a number of sides around a starting point.
        /// </summary>
        /// <param name="sides">The number of sides the polygon has.</param>
        /// <param name="startingPoint">The starting position.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="startingAngle">The starting angle. Change this to rotate the points.</param>
        /// <param name="pointArray">A supplied array to fill the points into.</param>
        /// <remarks>
        /// Calculation for points obtained from this answer:
        /// https://stackoverflow.com/questions/7198144/how-to-draw-a-n-sided-regular-polygon-in-cartesian-coordinates/7198171#7198171
        /// </remarks>
        public static void GetPointsForRegularPolygon(in int sides, in Vector2 startingPoint, in double radius, in double startingAngle, Vector2[] pointArray)
        {
            double radianIncrement = (Math.PI * 2d) / sides;

            for (int i = 0; i < pointArray.Length; i++)
            {
                double angleVal = startingAngle + (radianIncrement * i);

                pointArray[i].X = (float)(startingPoint.X + (radius * Math.Cos(angleVal)));
                pointArray[i].Y = (float)(startingPoint.Y + (radius * Math.Sin(angleVal)));
            }
        }

        /// <summary>
        /// Returns points for a regular polygon with a number of sides around a starting point.
        /// </summary>
        /// <param name="sides">The number of sides the polygon has.</param>
        /// <param name="startingPoint">The starting position.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="startingAngle">The starting angle, in radians. Change this to rotate the points.</param>
        /// <remarks>
        public static Vector2[] GetPointsForRegularPolygon(in int sides, in Vector2 startingPoint, in double radius, in double startingAngle)
        {
            Vector2[] pointArray = new Vector2[sides];
            GetPointsForRegularPolygon(sides, startingPoint, radius, startingAngle, pointArray);
            return pointArray;
        }

        /// <summary>
        /// Opens a link in the default web browser.
        /// </summary>
        /// <param name="URL">The URL to visit.</param>
        public static void OpenURL(string URL)
        {
            try
            {
                //Simply start a process with the URL
                System.Diagnostics.Process.Start(URL);
            }
            catch (Exception e) when (e is System.ComponentModel.Win32Exception || e is ObjectDisposedException || e is System.IO.FileNotFoundException)
            {
                Debug.LogError($"Ran into an issue starting the process to open URL \"{URL}\": {e.Message}");
            }
        }

        /// <summary>
        /// Opens a folder at the given path in the operating system's file manager.
        /// </summary>
        /// <param name="folderPath">The folder path to open.</param>
        public static void OpenFolderInFileManager(string folderPath)
        {
            try
            {
                System.Diagnostics.Process.Start(folderPath);
            }
            catch (Exception e) when (e is System.ComponentModel.Win32Exception || e is ObjectDisposedException || e is System.IO.FileNotFoundException)
            {
                Debug.LogError($"Ran into an issue starting the process to open file path \"{folderPath}\": {e.Message}");
            }
        }
    }
}
