using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TDMonoGameEngine
{
    /// <summary>
    /// Represents a Sprite.
    /// </summary>
    public class Sprite
    {
        public Texture2D Tex = null;
        public Rectangle? SourceRect = null;

        /// <summary>
        /// The pivot of the Sprite, from 0 to 1.
        /// </summary>
        public Vector2 Pivot = new Vector2(.5f, .5f);

        public Sprite(Texture2D texture, Rectangle? sourceRect)
        {
            Tex = texture;
            SourceRect = sourceRect;
        }

        public Sprite(Texture2D texture, Rectangle? sourceRect, Vector2 pivot)
            : this(texture, sourceRect)
        {
            Pivot = pivot;
        }

        public Vector2 GetOrigin()
        {
            Rectangle rect = Rectangle.Empty;

            if (SourceRect != null)
            {
                rect = SourceRect.Value;
            }
            else if (Tex != null)
            {
                rect = Tex.Bounds;
            }

            return rect.GetOrigin(Pivot.X, Pivot.Y);
        }
    }
}
