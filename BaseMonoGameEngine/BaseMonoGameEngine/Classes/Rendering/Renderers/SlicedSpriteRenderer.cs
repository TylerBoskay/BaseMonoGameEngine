using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TDMonoGameEngine
{
    public class SlicedSpriteRenderer : SpriteRenderer
    {
        private SlicedSprite SlicedSpriteToRender = null;

        public override Rectangle Bounds
        {
            get
            {
                return UtilityGlobals.CreateRect(TransformData.Position, TransformData.Scale);
            }
        }

        public SlicedSpriteRenderer(Transform transform, SlicedSprite slicedSpriteToRender) : base(transform, slicedSpriteToRender)
        {
            SlicedSpriteToRender = slicedSpriteToRender;
        }

        public override void Render()
        {
            if (TransformData == null || SpriteToRender == null)
                return;

            Rectangle overlayRect = Bounds;

            for (int i = 0; i < SlicedSpriteToRender.Slices; i++)
            {
                Rectangle rect = SlicedSpriteToRender.GetRectForIndex(overlayRect, i);

                RenderingManager.Instance.DrawSprite(SpriteToRender.Tex, rect, SlicedSpriteToRender.Regions[i], TintColor, TransformData.Rotation,
                    SlicedSpriteToRender.GetOrigin(), FlipData, Depth);
            }
        }
    }
}
