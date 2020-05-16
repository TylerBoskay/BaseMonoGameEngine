using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// Test HUD renderer.
    /// </summary>
    public sealed class HUDRenderer : Renderer
    {
        private TestGameHUD hud = null;
        public float Depth = 0f;

        public HUDRenderer(TestGameHUD gameHud)
        {
            hud = gameHud;
        }

        public override Rectangle Bounds
        {
            get
            {
                return Rectangle.Empty;
            }
        }

        public override void Render()
        {
            int amountDrawn = (int)Math.Ceiling(hud.MaxHealth / 2f);

            int width = hud.FullHealth.SourceRect.Value.Width;
            int height = hud.FullHealth.SourceRect.Value.Height;

            float scale = 2f;

            for (int i = 0; i < amountDrawn; i++)
            {
                Vector2 drawPos = hud.BaseHealthPos + new Vector2((i % hud.NumPerRow) * (width * scale), (i / hud.NumPerRow) * (height * scale));
                Sprite spriteToUse = hud.EmptyHealth;

                int diff = hud.DisplayHealth - (i * 2);
                if (diff >= 2) spriteToUse = hud.FullHealth;
                if (diff == 1) spriteToUse = hud.HalfHealth;

                RenderingManager.Instance.DrawSprite(spriteToUse.Tex, drawPos, spriteToUse.SourceRect, Color.White, 0f,
                    spriteToUse.GetOrigin(), new Vector2(scale, scale), SpriteEffects.None, Depth);
            }
        }
    }
}
