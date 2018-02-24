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
    /// Renders a sprite.
    /// </summary>
    public class SpriteRenderer : Renderer
    {
        public Transform TransformData = null;
        public Sprite SpriteToRender = null;
        public Color TintColor = Color.White;

        public SpriteEffects FlipData = SpriteEffects.None;
        public float Depth = 0f;

        public override Rectangle Bounds
        {
            get
            {
                Rectangle bounds = Rectangle.Empty;
                Vector2 pivotOrigin = SpriteToRender.GetOrigin();
                bounds.X = (int)TransformData.Position.X - (int)pivotOrigin.X;
                bounds.Y = (int)TransformData.Position.Y - (int)pivotOrigin.Y;

                if (SpriteToRender.SourceRect != null)
                {
                    Rectangle rect = SpriteToRender.SourceRect.Value;

                    bounds.Width = rect.Width;
                    bounds.Height = rect.Height;
                }
                else
                {
                    bounds.Width = SpriteToRender.Tex.Width;
                    bounds.Height = SpriteToRender.Tex.Height;
                }

                return bounds;
            }
        }

        public SpriteRenderer(Transform transform, Sprite spriteToRender)
        {
            TransformData = transform;
            SpriteToRender = spriteToRender;
        }

        public override void Render()
        {
            if (TransformData == null || SpriteToRender == null)
                return;
            
            RenderingManager.Instance.DrawSprite(SpriteToRender.Tex, TransformData.Position, SpriteToRender.SourceRect, TintColor,
                TransformData.Rotation, SpriteToRender.GetOrigin(), TransformData.Scale, FlipData, Depth);

           //Debug.DebugDrawHollowRect(Bounds, Color.Red, 1f, 1, false);
        }
    }
}
