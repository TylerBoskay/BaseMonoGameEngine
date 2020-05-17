using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BaseMonoGameEngine
{
    public class BlankGameState : IGameState
    {
        private Sprite BoxRect = null;

        public BlankGameState()
        {
            
        }

        public void Enter()
        {
            Texture2D boxTex = AssetManager.Instance.LoadTexture(ContentGlobals.BoxTex);

            BoxRect = new Sprite(boxTex, ContentGlobals.BoxRect);
        }

        public void Exit()
        {
            
        }

        public void Update()
        {
            
        }

        public void Render()
        {
            RenderingManager.Instance.StartBatch(RenderingManager.Instance.spriteBatch, SpriteSortMode.Deferred,
                BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            RenderingManager.Instance.CurrentBatch.Draw(BoxRect.Tex, RenderingGlobals.BaseResolutionHalved,
                BoxRect.SourceRect, Color.Red, 0f, BoxRect.GetOrigin(), new Vector2(100, 100), SpriteEffects.None, .1f);

            RenderingManager.Instance.EndCurrentBatch();
        }
    }
}
