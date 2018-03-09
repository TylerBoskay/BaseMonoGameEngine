using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TDMonoGameEngine
{
    /// <summary>
    /// A renderer for after-images.
    /// </summary>
    public sealed class AfterImageRenderer : Renderer
    {
        public override Rectangle Bounds => Rectangle.Empty;

        public List<SpriteRenderer> SpriteRenderers = null;

        public AfterImageRenderer(int maxAfterImages)
        {
            SpriteRenderers = new List<SpriteRenderer>(maxAfterImages);
        }

        public override void Render()
        {
            for (int i = 0; i < SpriteRenderers.Count; i++)
            {
                if (SpriteRenderers[i].Enabled == false) continue;

                SpriteRenderers[i].Render();
            }
        }
    }
}
