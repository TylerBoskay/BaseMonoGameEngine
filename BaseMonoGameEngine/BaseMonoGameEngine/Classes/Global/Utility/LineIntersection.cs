using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TDMonoGameEngine
{
    /// <summary>
    /// A class for checking Line intersection.
    /// </summary>
    public static class LineIntersection
    {
        private const double CloseToZero = .00001d;

        /// <summary>
        /// Checks the overlap between two Lines. If no overlap exists, returns null.
        /// </summary>
        /// <param name="line1">The first Line to test overlap with.</param>
        /// <param name="line2">The second Line to test overlap with.</param>
        /// <remarks>Code obtained from here: http://stackoverflow.com/q/22456517 and modified to handle division by 0.</remarks>
        /// <returns>A Line containing the overlap points between the two lines. null if no overlap exists.</returns>
        public static Line? GetLineOverlap(in Line line1, in Line line2)
        {
            bool undefinedSlope = false;
            double xDiff = (line1.P2.X - line1.P1.X);
            if (xDiff == 0f) undefinedSlope = true;

            double slope = 0d;
            if (undefinedSlope == false) slope = (line1.P2.Y - line1.P1.Y) / (line1.P2.X - line1.P1.X);

            bool isHorizontal = (undefinedSlope == true) ? false : AlmostZero(slope);
            bool isDescending = slope < 0 && !isHorizontal;
            double invertY = isDescending || isHorizontal ? -1 : 1;

            Point min1 = new Point(Math.Min(line1.P1.X, line1.P2.X), (int)Math.Min(line1.P1.Y * invertY, line1.P2.Y * invertY));
            Point max1 = new Point(Math.Max(line1.P1.X, line1.P2.X), (int)Math.Max(line1.P1.Y * invertY, line1.P2.Y * invertY));

            Point min2 = new Point(Math.Min(line2.P1.X, line2.P2.X), (int)Math.Min(line2.P1.Y * invertY, line2.P2.Y * invertY));
            Point max2 = new Point(Math.Max(line2.P1.X, line2.P2.X), (int)Math.Max(line2.P1.Y * invertY, line2.P2.Y * invertY));

            Point minIntersection;
            if (isDescending)
                minIntersection = new Point(Math.Max(min1.X, min2.X), (int)Math.Min(min1.Y * invertY, min2.Y * invertY));
            else
                minIntersection = new Point(Math.Max(min1.X, min2.X), (int)Math.Max(min1.Y * invertY, min2.Y * invertY));

            Point maxIntersection;
            if (isDescending)
                maxIntersection = new Point(Math.Min(max1.X, max2.X), (int)Math.Max(max1.Y * invertY, max2.Y * invertY));
            else
                maxIntersection = new Point(Math.Min(max1.X, max2.X), (int)Math.Min(max1.Y * invertY, max2.Y * invertY));

            bool intersect = minIntersection.X <= maxIntersection.X &&
                             (!isDescending && minIntersection.Y <= maxIntersection.Y ||
                               isDescending && minIntersection.Y >= maxIntersection.Y);

            if (!intersect)
                return null;

            return new Line(minIntersection, maxIntersection);
        }

        /// <summary>
        /// Determines if two Lines intersect.
        /// </summary>
        /// <param name="line1">The first Line to test intersection with.</param>
        /// <param name="line2">The second Line to test intersection with.</param>
        /// <remarks>Code obtained from here: http://gamedev.stackexchange.com/a/26022 </remarks>
        /// <returns>true if the Lines intersect each other, otherwise false.</returns>
        public static bool Intersects(in Line line1, in Line line2)
        {
            //Find if a Line intersects:
            /*Formula:
             *Pa = P1+Ua(P2-P1)
             *Pb = P3+Ub(P4-P3)
             * 0 for U = start, 1 = end
             * Pa=Pb
             * P1+Ua(P2-P1)=P3+Ub(P4-P3)
             * X-Y Terms:
             * x1+Ua(x2-x1)=x3+Ub(x4-x3)
             * y1+Ua(y2-y1)=y3+Ub(y4-y3)
             * 
             * Solve for U:
             * Ua=((x4-x3)(y1-y3)-(y4-y3)(x1-x3))/((y4-y3)(x2-x1)-(x4-x3)(y2-y1))
             * Ub=((x2-x1)(y1-y3)-(y2-y1)(x1-x3))/((y4-y3)(x2-x1)-(x4-x3)(y2-y1))
             * 
             * Solve denominator first: if 0, then the lines are parallel and don't intersect
             * If both numerators are 0, then the two lines are coincident (lie on top of each other, but may or may not overlap)
             * 
             * Check:
             * 0<=Ua<= 1
             * 0<=Ub<=1
             * 
             * If so, the lines intersect. To find point of intersection:
             * x=x1+Ua(x2-x1)
             * y=y1+Ua(y2-y1)
            */

            Point a = line1.P1;
            Point b = line1.P2;
            Point c = line2.P1;
            Point d = line2.P2;

            //numerator1 = Ua, numerator2 = Ub
            float denominator = ((d.Y - c.Y) * (b.X - a.X)) - ((d.X - c.X) * (b.Y - a.Y));
            float numerator1 = ((d.X - c.X) * (a.Y - c.Y)) - ((d.Y - c.Y) * (a.X - c.X));
            float numerator2 = ((b.X - a.X) * (a.Y - c.Y)) - ((b.Y - a.Y) * (a.X - c.X));

            //Check for parallel - check for a close to 0 value since these are floats
            if (Math.Abs(denominator) <= CloseToZero)
            {
                //Parallel; check if they are coincident

                //Check if they're coincident - if so, make sure they don't overlap
                if (Math.Abs(numerator1) <= CloseToZero && Math.Abs(numerator2) <= CloseToZero)
                {
                    //Check for an overlap
                    Line? overlap = GetLineOverlap(line1, line2);

                    return (overlap != null);
                }
                //Not coincident, no intersection
                else
                {
                    return false;
                }
            }

            float r = numerator1 / denominator;
            float s = numerator2 / denominator;

            return ((r >= 0 && r <= 1) && (s >= 0 && s <= 1));
        }

        private static bool AlmostEqualTo(double value1, double value2)
        {
            return Math.Abs(value1 - value2) <= CloseToZero;
        }

        private static bool AlmostZero(double value)
        {
            return Math.Abs(value) <= CloseToZero;
        }
    }
}
