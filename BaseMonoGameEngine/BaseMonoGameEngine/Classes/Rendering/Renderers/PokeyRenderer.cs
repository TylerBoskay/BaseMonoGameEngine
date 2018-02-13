using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// Test example of rendering multiple parts for an object.
    /// </summary>
    public class PokeyRenderer : SpriteRenderer
    {
        public readonly List<Sprite> SpritesToRender = new List<Sprite>();

        public override Rectangle Bounds
        {
            get
            {
                Rectangle bounds = Rectangle.Empty;
                bounds.X = (int)TransformData.Position.X;
                bounds.Y = (int)TransformData.Position.Y;

                int maxWidth = 0;
                int maxHeight = 0;
                
                for (int i = 0; i < SpritesToRender.Count; i++)
                {
                    if (SpritesToRender[i].SourceRect.HasValue == true)
                    {
                        Rectangle val = SpritesToRender[i].SourceRect.Value;
                        if (val.Width > maxWidth)
                        {
                            maxWidth = val.Width;
                        }
                        maxHeight += val.Height;
                    }
                }

                bounds.Width = maxWidth;
                bounds.Height = maxHeight;

                return bounds;
            }
        }

        public PokeyRenderer(Transform transformData, params Sprite[] spritesToRender) : base(transformData, null)
        {
            if (spritesToRender != null && spritesToRender.Length > 0)
            {
                SpritesToRender.AddRange(spritesToRender);
            }
        }

        public override void Render()
        {
            if (TransformData == null || SpritesToRender.Count == 0)
                return;

            int totalHeight = 0;

            for (int i = 0; i < SpritesToRender.Count; i++)
            {
                Sprite sprite = SpritesToRender[i];
                RenderingManager.Instance.DrawSprite(sprite.Tex, TransformData.Position + new Vector2(0, totalHeight), sprite.SourceRect, TintColor,
                    TransformData.Rotation, sprite.GetOrigin(), TransformData.Scale, FlipData, Depth);

                if (sprite.SourceRect.HasValue == true)
                {
                    totalHeight += sprite.SourceRect.Value.Height;
                }
            }
        }
    }
}
