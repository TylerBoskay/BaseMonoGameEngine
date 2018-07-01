using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TDMonoGameEngine
{
    /// <summary>
    /// Describes a 2D-rectangle consisting of floats for its components.
    /// </summary>
    public struct RectangleF
    {
        private static RectangleF EmptyRectF = new RectangleF();

        /// <summary>
        /// The X component of the rectangle.
        /// </summary>
        public float X;

        /// <summary>
        /// The Y component of the rectangle.
        /// </summary>
        public float Y;

        /// <summary>
        /// The width of the rectangle.
        /// </summary>
        public float Width;
        
        /// <summary>
        /// The height of the rectangle.
        /// </summary>
        public float Height;

        /// <summary>
        /// Returns a <see cref="RectangleF"/> with X=0, Y=0, Width=0, Height=0.
        /// </summary>
        public static ref readonly RectangleF Empty => ref EmptyRectF;

        /// <summary>
        /// A float representing the left of the RectangleF.
        /// </summary>
        public float Left => X;

        /// <summary>
        /// A float representing the right of the RectangleF.
        /// </summary>
        public float Right => (X + Width);

        /// <summary>
        /// A float representing the top of the RectangleF.
        /// </summary>
        public float Top => Y;

        /// <summary>
        /// A float representing the bottom of the RectangleF.
        /// </summary>
        public float Bottom => (Y + Height);

        /// <summary>
        /// The point representing the top-left of the RectangleF.
        /// </summary>
        public Vector2 TopLeft => new Vector2(X, Y);

        /// <summary>
        /// The point representing the top-right of the RectangleF.
        /// </summary>
        public Vector2 TopRight => new Vector2(Right, Y);

        /// <summary>
        /// The point representing the bottom-left of the RectangleF.
        /// </summary>
        public Vector2 BottomLeft => new Vector2(X, Bottom);

        /// <summary>
        /// The point representing the bottom-right of the RectangleF.
        /// </summary>
        public Vector2 BottomRight => new Vector2(Right, Bottom);

        /// <summary>
        /// Whether or not the RectangleF has a width and height of 0 and an X and Y of 0.
        /// </summary>
        public bool IsEmpty => (X == 0f && Y == 0f && Width == 0f && Height == 0f);

        /// <summary>
        /// The top-left coordinates of the RectangleF.
        /// </summary>
        public Vector2 Location
        {
            get => new Vector2(X, Y);
            set { X = value.X; Y = value.Y; }
        }

        /// <summary>
        /// The width-height coordinates of the RectangleF.
        /// </summary>
        public Vector2 Size
        {
            get => new Vector2(Width, Height);
            set { Width = value.X; Height = value.Y; }
        }

        /// <summary>
        /// A Vector2 located in the center of the RectangleF.
        /// </summary>
        public Vector2 Center => new Vector2(X + (Width / 2f), Y + (Height / 2f));

        public RectangleF(Vector2 location, Vector2 size) : this(location.X, location.Y, size.X, size.Y)
        {
            
        }

        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Tells whether this RectangleF intersects with another RectangleF.
        /// </summary>
        /// <param name="value">The RectangleF to test intersection with.</param>
        /// <returns>true if this RectangleF intersects with the other, otherwise false.</returns>
        public bool Intersects(in RectangleF value)
        {
            return (value.Left < Right && Left < value.Right && value.Top < Bottom && Top < value.Bottom);
        }

        /// <summary>
        /// Gets whether or not the other <see cref="RectangleF"/> intersects with this rectangle.
        /// </summary>
        /// <param name="value">The other rectangle for testing.</param>
        /// <param name="result"><c>true</c> if the other <see cref="RectangleF"/> intersects with this rectangle; <c>false</c> otherwise. As an output parameter.</param>
        public void Intersects(in RectangleF value, out bool result)
        {
            result = Intersects(value);
        }

        /// <summary>
        /// Creates a new <see cref="RectangleF"/> that contains the overlapping region of two other rectangles.
        /// </summary>
        /// <param name="value1">The first <see cref="RectangleF"/>.</param>
        /// <param name="value2">The second <see cref="RectangleF"/>.</param>
        /// <returns>Overlapping region of the two rectangles.</returns>
        public static RectangleF Intersect(in RectangleF value1, in RectangleF value2)
        {
            Intersect(value1, value2, out RectangleF rectangle);
            return rectangle;
        }

        /// <summary>
        /// Creates a new <see cref="RectangleF"/> that contains the overlapping region of two other rectangles.
        /// </summary>
        /// <param name="value1">The first <see cref="RectangleF"/>.</param>
        /// <param name="value2">The second <see cref="RectangleF"/>.</param>
        /// <param name="result">The overlapping region of the two rectangles as an output parameter.</param>
        public static void Intersect(in RectangleF value1, in RectangleF value2, out RectangleF result)
        {
            //If they intersect, see where
            if (value1.Intersects(value2) == true)
            {
                float right_side = Math.Min(value1.Right, value2.Right);
                float left_side = Math.Max(value1.X, value2.X);
                float top_side = Math.Max(value1.Y, value2.Y);
                float bottom_side = Math.Min(value1.Bottom, value2.Bottom);

                result = new RectangleF(left_side, top_side, right_side - left_side, bottom_side - top_side);
            }
            else
            {
                result = Empty;
            }
        }

        /// <summary>
        /// Tells if the RectangleF intersects a Circle.
        /// </summary>
        /// <param name="circle">The Circle to test for intersection.</param>
        /// <returns>true if they intersect, otherwise false.</returns>
        public bool Intersects(in Circle circle)
        {
            return circle.Intersects(this);
        }

        #region Operators

        public static bool operator ==(RectangleF rectA, RectangleF rectB)
        {
            return ((rectA.X == rectB.X) && (rectA.Y == rectB.Y) && (rectA.Width == rectB.Width) && (rectA.Height == rectB.Height));
        }

        public static bool operator !=(RectangleF rectA, RectangleF rectB)
        {
            return !(rectA == rectB);
        }

        /// <summary>
        /// Explicitly converts a RectangleF to a Rectangle.
        /// </summary>
        /// <param name="rectF">The RectangleF to convert.</param>
        public static explicit operator Rectangle(RectangleF rectF)
        {
            return new Rectangle((int)rectF.X, (int)rectF.Y, (int)rectF.Width, (int)rectF.Height);
        }

        /// <summary>
        /// Implicitly converts a Rectangle to a RectangleF.
        /// </summary>
        /// <param name="rect">The Rectangle to convert.</param>
        public static implicit operator RectangleF(Rectangle rect)
        {
            return new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }

        #endregion

        #region Comparison and Object Overrides

        /// <summary>
        /// Compares whether the current instance is equal to the specified <see cref="Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public override bool Equals(object obj)
        {
            return (obj is RectangleF) && this == ((RectangleF)obj);
        }

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="RectangleF"/>.
        /// </summary>
        /// <param name="other">The <see cref="RectangleF"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public bool Equals(in RectangleF other) => (this == other);

        /// <summary>
        /// Gets the hash code of this <see cref="RectangleF"/>.
        /// </summary>
        /// <returns>The hash code of this <see cref="RectangleF"/>.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = (hash * 23) + X.GetHashCode();
                hash = (hash * 23) + Y.GetHashCode();
                hash = (hash * 23) + Width.GetHashCode();
                hash = (hash * 23) + Height.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        /// Returns a <see cref="String"/> representation of this <see cref="RectangleF"/> in the format:
        /// {X:[<see cref="X"/>] Y:[<see cref="Y"/>] Width:[<see cref="Width"/>] Height:[<see cref="Height"/>]}
        /// </summary>
        /// <returns><see cref="String"/> representation of this <see cref="RectangleF"/>.</returns>
        public override string ToString()
        {
            return "{X:" + X + " Y:" + Y + " Width:" + Width + " Height:" + Height + "}";
        }

        #endregion
    }
}
