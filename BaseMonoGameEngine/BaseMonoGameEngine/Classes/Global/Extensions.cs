using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// A class for defining Extension Methods
    /// </summary>
    public static class Extensions
    {
        #region Texture2D Extensions
        
        /// <summary>
        /// Gets the origin of a Texture2D by ratio instead of specifying width and height.
        /// </summary>
        /// <param name="texture2D">The texture to get the origin for.</param>
        /// <param name="x">The X ratio of the origin, between 0 and 1.</param>
        /// <param name="y">The Y ratio of the origin, between 0 and 1.</param>
        /// <returns>A Vector2 with the origin.</returns>
        public static Vector2 GetOrigin(this Texture2D texture2D, in float x, in float y)
        {
            int xVal = (int)(texture2D.Width * UtilityGlobals.Clamp(x, 0f, 1f));
            int yVal = (int)(texture2D.Height * UtilityGlobals.Clamp(y, 0f, 1f));

            return new Vector2(xVal, yVal);
        }

        /// <summary>
        /// Gets the center origin of a Texture2D.
        /// </summary>
        /// <param name="texture2D">The texture to get the origin for.</param>
        /// <returns>A Vector2 with the center origin.</returns>
        public static Vector2 GetCenterOrigin(this Texture2D texture2D)
        {
            return texture2D.GetOrigin(.5f, .5f);
        }

        /// <summary>
        /// Gets the texture coordinates at specified X and Y values of a Texture2D in a Vector2. The returned X and Y values will be from 0 to 1.
        /// </summary>
        /// <param name="texture2D">The Texture2D to get the texture coordinates from.</param>
        /// <param name="sourceRect">The Rectangle to get the coordinates from.</param>
        /// <returns>A Vector2 with the Rectangle's X and Y values divided by the texture's width and height, respectively.</returns>
        public static Vector2 GetTexCoordsAt(this Texture2D texture2D, Rectangle? sourceRect)
        {
            Vector2 texCoords = Vector2.Zero;

            if (sourceRect != null)
            {
                return GetTexCoordsAt(texture2D, sourceRect.Value.X, sourceRect.Value.Y);
            }

            return texCoords;
        }

        /// <summary>
        /// Gets the texture coordinates at specified X and Y values of a Texture2D in a Vector2. The returned X and Y values will be from 0 to 1.
        /// </summary>
        /// <param name="texture2D">The Texture2D to get the texture coordinates from.</param>
        /// <param name="x">The X position on the texture.</param>
        /// <param name="y">The Y position on the texture.</param>
        /// <returns>A Vector2 with the X and Y values divided by the texture's width and height, respectively.</returns>
        public static Vector2 GetTexCoordsAt(this Texture2D texture2D, in int x, in int y)
        {
            Vector2 texCoords = Vector2.Zero;

            //Get the ratio of the X and Y values from the Width and Height of the texture
            if (texture2D.Width > 0)
                texCoords.X = x / (float)texture2D.Width;
            if (texture2D.Height > 0)
                texCoords.Y = y / (float)texture2D.Height;

            return texCoords;
        }

        #endregion

        #region SpriteFont Extensions

        /// <summary>
        /// Gets the origin of a SpriteFont by ratio instead of specifying width and height.
        /// </summary>
        /// <param name="spriteFont">The font to get the origin for.</param>
        /// <param name="text">The text to be displayed.</param>
        /// <param name="x">The X ratio of the origin, between 0 and 1.</param>
        /// <param name="y">The Y ratio of the origin, between 0 and 1.</param>
        /// <returns>A Vector2 with the origin.</returns>
        public static Vector2 GetOrigin(this SpriteFont spriteFont, in string text, in float x, in float y)
        {
            if (string.IsNullOrEmpty(text) == true) return Vector2.Zero;

            Vector2 size = spriteFont.MeasureString(text);
            size.X *= UtilityGlobals.Clamp(x, 0f, 1f);
            size.Y *= UtilityGlobals.Clamp(y, 0f, 1f);

            return size;
        }

        /// <summary>
        /// Gets the center origin of a SpriteFont.
        /// </summary>
        /// <param name="spriteFont">The font to get the origin for.</param>
        /// <param name="text">The text to be displayed.</param>
        /// <returns>A Vector2 with the center origin.</returns>
        public static Vector2 GetCenterOrigin(this SpriteFont spriteFont, in string text)
        {
            return spriteFont.GetOrigin(text, .5f, .5f);
        }

        /// <summary>
        /// Finds a Glyph associated with a particular character in a SpriteFont.
        /// </summary>
        /// <param name="spriteFont">The font to get the Glyph for.</param>
        /// <param name="c">The character to find the glyph for.</param>
        /// <returns>A Glyph? containing the Glyph found, otherwise null.</returns>
        public static SpriteFont.Glyph? FindGlyphForChar(this SpriteFont spriteFont, in char c)
        {
            for (int i = 0; i < spriteFont.Glyphs.Length; i++)
            {
                ref SpriteFont.Glyph glyph = ref spriteFont.Glyphs[i];

                if (glyph.Character == c) return glyph;
            }

            return null;
        }

        #endregion

        #region Rectangle Extensions

        /// <summary>
        /// Gets the top-left point of the Rectangle.
        /// </summary>
        /// <param name="rectangle">The Rectangle.</param>
        /// <returns>A Vector2 containing the top-left point of the rectangle.</returns>
        public static Vector2 TopLeft(this Rectangle rectangle) => new Vector2(rectangle.Left, rectangle.Top);

        /// <summary>
        /// Gets the top-right point of the Rectangle.
        /// </summary>
        /// <param name="rectangle">The Rectangle.</param>
        /// <returns>A Vector2 containing the top-right point of the rectangle.</returns>
        public static Vector2 TopRight(this Rectangle rectangle) => new Vector2(rectangle.Right, rectangle.Top);

        /// <summary>
        /// Gets the bottom-left point of the Rectangle.
        /// </summary>
        /// <param name="rectangle">The Rectangle.</param>
        /// <returns>A Vector2 containing the bottom-left point of the rectangle.</returns>
        public static Vector2 BottomLeft(this Rectangle rectangle) => new Vector2(rectangle.Left, rectangle.Bottom);

        /// <summary>
        /// Gets the bottom-right point of the Rectangle.
        /// </summary>
        /// <param name="rectangle">The Rectangle.</param>
        /// <returns>A Vector2 containing the bottom-right point of the rectangle.</returns>
        public static Vector2 BottomRight(this Rectangle rectangle) => new Vector2(rectangle.Right, rectangle.Bottom);

        /// <summary>
        /// Tells if the Rectangle intersects a Circle.
        /// </summary>
        /// <param name="rectangle">The Rectangle.</param>
        /// <param name="circle">The Circle to test intersection with.</param>
        /// <returns>true if the Rectangle intersects the Circle, otherwise false.</returns>
        public static bool Intersects(this Rectangle rectangle, Circle circle) => circle.Intersects(rectangle);

        /// <summary>
        /// Gets the origin of a Rectangle.
        /// </summary>
        /// <param name="rectangle">The Rectangle to get the origin for.</param>
        /// <param name="x">The X ratio of the origin, from 0 to 1.</param>
        /// <param name="y">The Y ratio of the origin, from 0 to 1.</param>
        /// <returns>A Vector2 with the origin.</returns>
        public static Vector2 GetOrigin(this Rectangle rectangle, in float x, in float y)
        {
            int xVal = (int)(rectangle.Width * UtilityGlobals.Clamp(x, 0f, 1f));
            int yVal = (int)(rectangle.Height * UtilityGlobals.Clamp(y, 0f, 1f));

            return new Vector2(xVal, yVal);
        }

        /// <summary>
        /// Gets the center origin of a Rectangle.
        /// </summary>
        /// <param name="rectangle">The Rectangle to get the origin for.</param>
        /// <returns>A Vector2 with the center origin.</returns>
        public static Vector2 GetCenterOrigin(this Rectangle rectangle)
        {
            return rectangle.GetOrigin(.5f, .5f);
        }

        #endregion

        #region Color Extensions

        /// <summary>
        /// Divides a Color's components by a scalar amount.
        /// </summary>
        /// <param name="color">The Color.</param>
        /// <param name="scalar">The scalar value to divide the Color components by.</param>
        /// <returns>A Color which has the components of the original Color divided by the scalar amount and floored due to integer casting.</returns>
        public static Color Divide(this Color color, in float scalar)
        {
            return new Color((int)(color.R / scalar), (int)(color.G / scalar), (int)(color.B / scalar), (int)(color.A / scalar));
        }

        /// <summary>
        /// Multiplies a Color's components by a scalar amount, using the ceiling of the resulting values.
        /// </summary>
        /// <param name="color">The Color.</param>
        /// <param name="scalar">The scalar value to multiply the Color components by.</param>
        /// <returns>A Color which has the components of the original Color multiplied by the scalar amount, using the ceiling of the results.</returns>
        public static Color CeilingMult(this Color color, in float scalar)
        {
            return new Color((int)Math.Ceiling(color.R * scalar), (int)Math.Ceiling(color.G * scalar), (int)Math.Ceiling(color.B * scalar), (int)Math.Ceiling(color.A * scalar));
        }

        #endregion

        #region Vector2 Extensions

        /// <summary>
        /// Halves the Vector2.
        /// </summary>
        /// <param name="vector2">The Vector2 to halve.</param>
        /// <returns>A Vector2 with the X and Y components halved.</returns>
        public static Vector2 Halve(this Vector2 vector2)
        {
            return vector2 / 2f;
        }

        /// <summary>
        /// Halves the Vector2, truncating the X and Y components to the nearest integer.
        /// </summary>
        /// <param name="vector2">The Vector2 to halve.</param>
        /// <returns>A Vector2 with the X and Y components halved as integer values.</returns>
        public static Vector2 HalveInt(this Vector2 vector2)
        {
            return new Vector2((int)(vector2.X / 2f), (int)(vector2.Y / 2f));
        }

        #endregion

        #region List Extensions

        /// <summary>
        /// Removes an <see cref="IList{T}"/> of elements from the <see cref="List{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the List and IList.</typeparam>
        /// <param name="list">The <see cref="List{T}"/> to remove elements from.</param>
        /// <param name="elements">The elements to remove from the <see cref="List{T}"/>.</param>
        public static void RemoveFromList<T>(this List<T> list, in IList<T> elements)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                list.Remove(elements[i]);
            }
        }

        #endregion

        #region Dictionary Extensions

        /// <summary>
        /// Copies unique keys and values from a <see cref="Dictionary{TKey, TValue}"/> into an existing <see cref="Dictionary{TKey, TValue}"/>.
        /// If the key already exists in the dictionary to copy to, it will replace it.
        /// </summary>
        /// <typeparam name="T">The type of the key.</typeparam>
        /// <typeparam name="U">The type of the value.</typeparam>
        /// <param name="dictCopiedTo">The Dictionary to copy values to.</param>
        /// <param name="dictCopiedFrom">The Dictionary to copy from.</param>
        public static void CopyDictionaryData<T, U>(this Dictionary<T, U> dictCopiedTo, in Dictionary<T, U> dictCopiedFrom)
        {
            //Don't do anything if null, since there's nothing to copy from
            if (dictCopiedFrom == null) return;

            //Go through all keys and values
            foreach (KeyValuePair<T, U> kvPair in dictCopiedFrom)
            {
                T key = kvPair.Key;

                //Replace if already exists
                if (dictCopiedTo.ContainsKey(key) == true)
                {
                    dictCopiedTo.Remove(key);
                }

                dictCopiedTo.Add(key, kvPair.Value);
            }
        }

        /// <summary>
        /// Copies the keys and values from this <see cref="Dictionary{TKey, TValue}"/> into a new <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the key.</typeparam>
        /// <typeparam name="U">The type of the value.</typeparam>
        /// <param name="dictionary">The Dictionary to copy from.</param>
        /// <returns>A new <see cref="Dictionary{TKey, TValue}"/> with the same key-value pairs as <paramref name="dictionary"/>.</returns>
        public static Dictionary<T, U> CopyDictionary<T,U>(this Dictionary<T, U> dictionary)
        {
            Dictionary<T, U > newDict = new Dictionary<T, U>();
            
            //Copy all elements into the new Dictionary
            foreach (KeyValuePair<T, U> kvPair in dictionary)
            {
                newDict.Add(kvPair.Key, kvPair.Value);
            }

            return newDict;
        }

        #endregion

        #region Random Extensions

        /// <summary>
        /// Returns a random floating-point number that is greater than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/>.
        /// </summary>
        /// <param name="random">The Random instance.</param>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. This must be greater than or equal to <paramref name="minValue"/>.</param>
        /// <returns></returns>
        public static double NextDouble(this Random random, in double minValue, in double maxValue)
        {
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }

        #endregion

        #region SpriteBatch Extensions

        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
        /// <param name="rectTex">The texture for the line.</param>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <param name="color">The color of the line.</param>
        /// <param name="layer">The layer of the line.</param>
        /// <param name="thickness">The thickness of the line.</param>
        public static void DrawLine(this SpriteBatch spriteBatch, in Texture2D lineTex, in Vector2 start, in Vector2 end, in Color color, in float layer, in int thickness)
        {
            //Get rotation with the angle between the start and end vectors
            float lineRotation = (float)UtilityGlobals.TangentAngle(start, end);

            //Get the scale; use the X as the length and the Y as the width
            Vector2 diff = end - start;
            Vector2 lineScale = new Vector2(diff.Length(), thickness);

            spriteBatch.Draw(lineTex, start, null, color, lineRotation, Vector2.Zero, lineScale, SpriteEffects.None, layer);
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
        /// <param name="rectTex">The texture for the rectangle.</param>
        /// <param name="rect">The Rectangle to draw.</param>
        /// <param name="color">The color of the rectangle.</param>
        /// <param name="layer">The layer of the rectangle.</param>
        public static void DrawRect(this SpriteBatch spriteBatch, in Texture2D rectTex, in Rectangle rect, in Color color, in float layer)
        {
            spriteBatch.Draw(rectTex, rect, null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
        }

        /// <summary>
        /// Draws a float rectangle.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
        /// <param name="rectTex">The texture for the rectangle.</param>
        /// <param name="rect">The RectangleF to draw.</param>
        /// <param name="color">The color of the rectangle.</param>
        /// <param name="layer">The layer of the rectangle.</param>
        public static void DrawRect(this SpriteBatch spriteBatch, in Texture2D rectTex, RectangleF rect, in Color color, in float layer)
        {
            spriteBatch.Draw(rectTex, rect.TopLeft, null, color, 0f, Vector2.Zero, rect.Size, SpriteEffects.None, layer);
        }

        /// <summary>
        /// Draws a hollow rectangle.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
        /// <param name="rectTex">The texture for the rectangle.</param>
        /// <param name="rect">The Rectangle to draw.</param>
        /// <param name="color">The color of the hollow rectangle.</param>
        /// <param name="layer">The layer of the hollow rectangle.</param>
        /// <param name="thickness">The thickness of the hollow rectangle.</param>
        public static void DrawHollowRect(this SpriteBatch spriteBatch, in Texture2D rectTex, Rectangle rect, in Color color, in float layer, in int thickness)
        {
            Rectangle topLine = new Rectangle(rect.X, rect.Y, rect.Width, thickness);
            Rectangle rightLine = new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height);
            Rectangle leftLine = new Rectangle(rect.X, rect.Y, thickness, rect.Height);
            Rectangle bottomLine = new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness);

            spriteBatch.Draw(rectTex, topLine, null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
            spriteBatch.Draw(rectTex, rightLine, null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
            spriteBatch.Draw(rectTex, leftLine, null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
            spriteBatch.Draw(rectTex, bottomLine, null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
        }

        /// <summary>
        /// Draws a hollow float rectangle.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
        /// <param name="rectTex">The texture for the rectangle.</param>
        /// <param name="rect">The RectangleF to draw.</param>
        /// <param name="color">The color of the hollow rectangle.</param>
        /// <param name="layer">The layer of the hollow rectangle.</param>
        /// <param name="thickness">The thickness of the hollow rectangle.</param>
        public static void DrawHollowRect(this SpriteBatch spriteBatch, in Texture2D rectTex, RectangleF rect, in Color color, in float layer, in int thickness)
        {
            RectangleF[] rects = new RectangleF[4]
            {
                new RectangleF(rect.X, rect.Y, rect.Width, thickness),
                new RectangleF(rect.Right - thickness, rect.Y, thickness, rect.Height),
                new RectangleF(rect.X, rect.Y, thickness, rect.Height),
                new RectangleF(rect.X, rect.Bottom - thickness, rect.Width, thickness)
            };

            for (int i = 0; i < rects.Length; i++)
            {
                RectangleF rectf = rects[i];

                spriteBatch.Draw(rectTex, rectf.TopLeft, null, color, 0f, Vector2.Zero, rectf.Size, SpriteEffects.None, layer);
            }
        }

        /// <summary>
        /// Draws a circle.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
        /// <param name="circleTex">The texture for the circle.</param>
        /// <param name="circle">The circle to draw.</param>
        /// <param name="color">The color of the circle.</param>
        /// <param name="layer">The layer of the circle.</param>
        /// <remarks>Brute force algorithm obtained from here: https://stackoverflow.com/a/1237519 
        /// This seems to gives a more full looking circle than Bresenham's algorithm.
        /// </remarks>
        public static void DrawCircle(this SpriteBatch spriteBatch, in Texture2D circleTex, in Circle circle, in Color color, in float layer)
        {
            float radius = (float)circle.Radius;
            Vector2 origin = circle.Center;
            float radiusSquared = radius * radius;
            float radiusSquaredPlusRadius = radiusSquared + radius;

            for (float y = -radius; y <= radius; y++)
            {
                for (float x = -radius; x <= radius; x++)
                {
                    float xSquared = x * x;
                    float ySquared = y * y;

                    if ((xSquared + ySquared) < radiusSquaredPlusRadius)
                    {
                        spriteBatch.Draw(circleTex, new Vector2(origin.X + x, origin.Y + y), null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, layer);
                    }
                }
            }
        }

        /// <summary>
        /// Draws a hollow circle.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
        /// <param name="circleTex">The texture for the circle.</param>
        /// <param name="circle">The circle to draw.</param>
        /// <param name="color">The color of the circle.</param>
        /// <param name="layer">The layer of the circle.</param>
        /// <remarks>Brute force algorithm obtained from here: https://stackoverflow.com/a/1237519 
        /// This seems to gives a more full looking circle than Bresenham's algorithm.
        /// </remarks>
        public static void DrawHollowCircle(this SpriteBatch spriteBatch, in Texture2D circleTex, in Circle circle, in Color color, in float layer)
        {
            float radius = (float)circle.Radius;
            Vector2 origin = circle.Center;
            float radiusSquared = radius * radius;
            float radiusSqMinusRadius = radiusSquared - radius;
            float radiusSqPlusRadius = radiusSquared + radius;

            for (float y = -radius; y <= radius; y++)
            {
                for (float x = -radius; x <= radius; x++)
                {
                    float xSquared = x * x;
                    float ySquared = y * y;

                    if ((xSquared + ySquared) > radiusSqMinusRadius && (xSquared + ySquared) < radiusSqPlusRadius)
                    {
                        spriteBatch.Draw(circleTex, new Vector2(origin.X + x, origin.Y + y), null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, layer);
                    }
                }
            }
        }

        /// <summary>
        /// Draws a polygon, or a set of points.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
        /// <param name="lineTex">The texture for each line in the polygon.</param>
        /// <param name="points">An <see cref="IList{T}"/> of points for the polygon.</param>
        /// <param name="color">The color of the line.</param>
        /// <param name="layer">The layer of the line.</param>
        /// <param name="thickness">The thickness of the lines to draw.</param>
        public static void DrawPolygon(this SpriteBatch spriteBatch, in Texture2D lineTex, in IList<Vector2> points, in Color color,
            in float layer, in int thickness)
        {
            //Nothing to draw
            if (points == null || points.Count == 0) return;

            //Draw a point
            if (points.Count == 1)
            {
                spriteBatch.Draw(lineTex, points[0], null, color);
                return;
            }

            //Draw all the polygon's lines minus one
            for (int i = 0; i < points.Count - 1; i++)
            {
                spriteBatch.DrawLine(lineTex, points[i], points[i + 1], color, layer, thickness);
            }

            //Draw a line from the last point to the first one if we should
            spriteBatch.DrawLine(lineTex, points[points.Count - 1], points[0], color, layer, thickness);
        }

        /// <summary>
        /// Draws an individual character. This returns the offset calculated while drawing,
        /// which can be used to get the correct position of the next character.
        /// <para>This is a near replica of MonoGame's DrawString() method, with adjustments for a single character and more comments.</para>
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to use to render the character.</param>
        /// <param name="font">The SpriteFont to render the character with.</param>
        /// <param name="c">The character to render.</param>
        /// <param name="glyph">The Glyph for the character rendered in the SpriteFont.</param>
        /// <param name="startOffset">The starting offset to render the character.
        /// This can be used to pick up where another character left off.</param>
        /// <param name="position">The position to render the character.</param>
        /// <param name="color">The color to render the character in.</param>
        /// <param name="rotation">The rotation to render the character in.</param>
        /// <param name="origin">The origin to render the character with.</param>
        /// <param name="scale">The scale to render the character in.</param>
        /// <param name="effects">The SpriteEffects to render the character in.</param>
        /// <param name="layerDepth">The depth to render the character in.</param>
        /// <returns>A Vector2 containing the offset calculated when rendering the character.</returns>
        public static Vector2 DrawCharacter(this SpriteBatch spriteBatch, SpriteFont font, in char c, SpriteFont.Glyph? glyph,
            in Vector2 startOffset, in Vector2 position, in Color color, in float rotation, in Vector2 origin, in Vector2 scale,
            in SpriteEffects effects, in float layerDepth)
        {
            Vector2 offset = startOffset;
            bool firstGlyphOfLine = (offset.X == 0f);

            //On carriage return, simply exit
            if (c == '\r')
                return offset;

            //If we encounter a newline, reset the X offset and go down by the line spacing
            if (c == '\n')
            {
                offset.X = 0;
                offset.Y += font.LineSpacing;
                firstGlyphOfLine = true;
                return offset;
            }

            //If this character can't be rendered, exit
            if (glyph == null)
                return offset;

            SpriteFont.Glyph charGlyph = glyph.Value;

            //The first character on a line might have a negative left side bearing
            //In this scenario, offset the text to the right so that the text does not hang off the left side of its rectangle
            if (firstGlyphOfLine)
            {
                offset.X = Math.Max(charGlyph.LeftSideBearing * scale.X, 0);
                firstGlyphOfLine = false;
            }
            //Add the left side bearing and the spacing
            else
            {
                offset.X += ((font.Spacing + charGlyph.LeftSideBearing) * scale.X);
            }

            //Add the cropping
            Vector2 p = offset;
            p.X += charGlyph.Cropping.X * scale.X;
            p.Y += charGlyph.Cropping.Y * scale.Y;

            //Add the position passed in to obtain the final position to render this character
            p += position;

            //Render the character using the font's texture, the character's bounds in the font, and the other information passed in
            spriteBatch.Draw(font.Texture, p, charGlyph.BoundsInTexture, color, rotation, origin, scale, effects, layerDepth);

            //Add the character's width with its right side bearing for the next character
            offset.X += ((charGlyph.Width + charGlyph.RightSideBearing) * scale.X);

            return offset;
        }

        /// <summary>
        /// Draws an individual character. This returns the offset calculated while drawing,
        /// which can be used to get the correct position of the next character.
        /// <para>This overload finds the glyph in the SpriteFont.</para>
        /// <para>This is a near replica of MonoGame's DrawString() method, with adjustments for a single character and more comments.</para>
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to use to render the character.</param>
        /// <param name="font">The SpriteFont to render the character with.</param>
        /// <param name="c">The character to render.</param>
        /// <param name="startOffset">The starting offset to render the character.
        /// This can be used to pick up where another character left off.</param>
        /// <param name="position">The position to render the character.</param>
        /// <param name="color">The color to render the character in.</param>
        /// <param name="rotation">The rotation to render the character in.</param>
        /// <param name="origin">The origin to render the character with.</param>
        /// <param name="scale">The scale to render the character in.</param>
        /// <param name="effects">The SpriteEffects to render the character in.</param>
        /// <param name="layerDepth">The depth to render the character in.</param>
        /// <returns>A Vector2 containing the offset calculated when rendering the character.</returns>
        public static Vector2 DrawCharacter(this SpriteBatch spriteBatch, SpriteFont font, in char c,
            in Vector2 startOffset, in Vector2 position, in Color color, in float rotation, in Vector2 origin, in Vector2 scale,
            in SpriteEffects effects, in float layerDepth)
        {
            SpriteFont.Glyph? charGlyph = font.FindGlyphForChar(c);

            return DrawCharacter(spriteBatch, font, c, charGlyph, startOffset, position, color, rotation, origin, scale, effects, layerDepth);
        }

        /// <summary>
        /// Draws a character with an outline.
        /// <para>Note: This method performs 4 more draw calls to simulate an outline.</para>
        /// </summary>
        public static Vector2 DrawCharacterOutline(this SpriteBatch spriteBatch, in float outlineThickness, in Color outlineColor, SpriteFont font,
            in char c, in SpriteFont.Glyph? glyph, in Vector2 startOffset, in Vector2 position, in Color color, in float rotation,
            in Vector2 origin, in Vector2 scale, in SpriteEffects effects, in float layerDepth)
        {
            DrawCharacter(spriteBatch, font, c, glyph, startOffset, position - new Vector2(outlineThickness, 0), outlineColor, rotation,
                origin, scale, effects, layerDepth);
            DrawCharacter(spriteBatch, font, c, glyph, startOffset, position - new Vector2(0, outlineThickness), outlineColor, rotation,
                origin, scale, effects, layerDepth);
            DrawCharacter(spriteBatch, font, c, glyph, startOffset, position + new Vector2(outlineThickness, 0), outlineColor, rotation,
                origin, scale, effects, layerDepth);
            DrawCharacter(spriteBatch, font, c, glyph, startOffset, position + new Vector2(0, outlineThickness), outlineColor, rotation,
                origin, scale, effects, layerDepth);

            return DrawCharacter(spriteBatch, font, c, glyph, startOffset, position, color, rotation, origin, scale, effects, layerDepth);
        }

        /// <summary>
        /// Draws a character with an outline. This overload finds the glyph in the SpriteFont.
        /// <para>Note: This method performs 4 more draw calls to simulate an outline.</para>
        /// </summary>
        public static Vector2 DrawCharacterOutline(this SpriteBatch spriteBatch, in float outlineThickness, in Color outlineColor, SpriteFont font,
            in char c, in Vector2 startOffset, in Vector2 position, in Color color, in float rotation,
            in Vector2 origin, in Vector2 scale, in SpriteEffects effects, in float layerDepth)
        {
            SpriteFont.Glyph? glyph = font.FindGlyphForChar(c);

            return DrawCharacterOutline(spriteBatch, outlineThickness, outlineColor, font, c, glyph, startOffset, position, color,
                rotation, origin, scale, effects, layerDepth);
        }

        /// <summary>
        /// Submit a text string of sprites for drawing in the current batch.
        /// <para>Note: This method performs 4 more draw calls to simulate an outline.</para>
        /// </summary>
        public static void DrawStringOutline(this SpriteBatch spriteBatch, in float outlineThickness, in Color outlineColor,
            in SpriteFont spriteFont, string text, in Vector2 position, in Color color, in float rotation, in Vector2 origin, in float scale,
            in SpriteEffects effects, in float layerDepth)
        {
            spriteBatch.DrawString(spriteFont, text, position - new Vector2(outlineThickness, 0), outlineColor, rotation, origin, scale, effects, layerDepth);
            spriteBatch.DrawString(spriteFont, text, position - new Vector2(0, outlineThickness), outlineColor, rotation, origin, scale, effects, layerDepth);
            spriteBatch.DrawString(spriteFont, text, position + new Vector2(outlineThickness, 0), outlineColor, rotation, origin, scale, effects, layerDepth);
            spriteBatch.DrawString(spriteFont, text, position + new Vector2(0, outlineThickness), outlineColor, rotation, origin, scale, effects, layerDepth);

            spriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
        }

        /// <summary>
        /// Submit a text string of sprites for drawing in the current batch.
        /// <para>Note: This method performs 4 more draw calls to simulate an outline.</para>
        /// </summary>
        public static void DrawStringOutline(this SpriteBatch spriteBatch, in float outlineThickness, in Color outlineColor,
            in SpriteFont spriteFont, string text, in Vector2 position, in Color color, in float rotation, in Vector2 origin, in Vector2 scale,
            in SpriteEffects effects, in float layerDepth)
        {
            spriteBatch.DrawString(spriteFont, text, position - new Vector2(outlineThickness, 0), outlineColor, rotation, origin, scale, effects, layerDepth);
            spriteBatch.DrawString(spriteFont, text, position - new Vector2(0, outlineThickness), outlineColor, rotation, origin, scale, effects, layerDepth);
            spriteBatch.DrawString(spriteFont, text, position + new Vector2(outlineThickness, 0), outlineColor, rotation, origin, scale, effects, layerDepth);
            spriteBatch.DrawString(spriteFont, text, position + new Vector2(0, outlineThickness), outlineColor, rotation, origin, scale, effects, layerDepth);

            spriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
        }

        #endregion

        #region Effect Extensions

        public static void SetParameterValue(this Effect effect, string name, bool value)
        {
            effect.Parameters[name]?.SetValue(value);
        }

        public static void SetParameterValue(this Effect effect, string name, float value)
        {
            effect.Parameters[name]?.SetValue(value);
        }

        public static void SetParameterValue(this Effect effect, string name, float[] value)
        {
            effect.Parameters[name]?.SetValue(value);
        }

        public static void SetParameterValue(this Effect effect, string name, int value)
        {
            effect.Parameters[name]?.SetValue(value);
        }

        public static void SetParameterValue(this Effect effect, string name, Matrix value)
        {
            effect.Parameters[name]?.SetValue(value);
        }

        public static void SetParameterValue(this Effect effect, string name, Matrix[] value)
        {
            effect.Parameters[name]?.SetValue(value);
        }

        public static void SetParameterValue(this Effect effect, string name, Quaternion value)
        {
            effect.Parameters[name]?.SetValue(value);
        }

        public static void SetParameterValue(this Effect effect, string name, Texture value)
        {
            effect.Parameters[name]?.SetValue(value);
        }

        public static void SetParameterValue(this Effect effect, string name, Vector2 value)
        {
            effect.Parameters[name]?.SetValue(value);
        }

        public static void SetParameterValue(this Effect effect, string name, Vector2[] value)
        {
            effect.Parameters[name]?.SetValue(value);
        }

        public static void SetParameterValue(this Effect effect, string name, Vector3 value)
        {
            effect.Parameters[name]?.SetValue(value);
        }

        public static void SetParameterValue(this Effect effect, string name, Vector3[] value)
        {
            effect.Parameters[name]?.SetValue(value);
        }

        public static void SetParameterValue(this Effect effect, string name, Vector4 value)
        {
            effect.Parameters[name]?.SetValue(value);
        }

        public static void SetParameterValue(this Effect effect, string name, Vector4[] value)
        {
            effect.Parameters[name]?.SetValue(value);
        }

        #endregion
    }
}
