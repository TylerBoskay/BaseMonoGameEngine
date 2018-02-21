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
    /// Manages rendering the scene.
    /// <para>This is a singleton.</para>
    /// </summary>
    public sealed class RenderingManager : ICleanup
    {
        #region Singleton Fields

        public static RenderingManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RenderingManager();
                }

                return instance;
            }
        }

        public static bool HasInstance => (instance != null);

        private static RenderingManager instance = null;

        #endregion

        /// <summary>
        /// Rendering metrics. This data is obtained after all rendering is complete for the frame.
        /// </summary>
        public GraphicsMetrics RenderingMetrics = default(GraphicsMetrics);

        public GraphicsDeviceManager graphicsDeviceManager { get; private set; } = null;
        public GraphicsDevice graphicsDevice => graphicsDeviceManager?.GraphicsDevice;

        //public RenderingSettings RenderSettings = default(RenderingSettings);

        public SpriteBatch spriteBatch { get; private set; } = null;

        /// <summary>
        /// The current SpriteBatch that has started. If null, then a batch hasn't started.
        /// </summary>
        public SpriteBatch CurrentBatch { get; private set; } = null;

        /// <summary>
        /// The dimensions of the back buffer.
        /// </summary>
        public Vector2 BackBufferDimensions => new Vector2(graphicsDevice.PresentationParameters.BackBufferWidth, graphicsDevice.PresentationParameters.BackBufferHeight);

        /// <summary>
        /// The number of post-processing shaders in effect.
        /// </summary>
        public int PostProcessingCount => PostProcessingEffects.Count;

        private RenderTarget2D MainRenderTarget = null;

        /// <summary>
        /// A list of post-processing shaders to apply. They'll be applied in order.
        /// </summary>
        private readonly List<Effect> PostProcessingEffects = new List<Effect>();

        //private readonly List<RenderBatch> RenderBatches = new List<RenderBatch>();

        private readonly List<RenderingGroup> RenderGroups = new List<RenderingGroup>();

        private RenderingManager()
        {
            
        }

        public void CleanUp()
        {
            RemoveAllPostProcessingEffects();

            //RenderBatches.Clear();
            RenderGroups.Clear();
        }

        public void Initialize(GraphicsDeviceManager graphicsDeviceMngr)
        {
            graphicsDeviceManager = graphicsDeviceMngr;
            spriteBatch = new SpriteBatch(graphicsDevice);

            MainRenderTarget = new RenderTarget2D(graphicsDevice, RenderingGlobals.WindowWidth, RenderingGlobals.WindowHeight);
        }

        /// <summary>
        /// Adds a post-processing effect.
        /// </summary>
        /// <param name="effect">The post-processing effect to add.</param>
        /// <param name="index">The index to insert the effect at. If less than 0, it will add it to the end of the list.</param>
        public void AddPostProcessingEffect(Effect effect, int index = -1)
        {
            if (index < 0)
            {
                PostProcessingEffects.Add(effect);
            }
            else
            {
                PostProcessingEffects.Insert(index, effect);
            }
        }

        /// <summary>
        /// Removes a post-processing effect.
        /// </summary>
        /// <param name="effectToRemove">The post-processing effect to remove.</param>
        public void RemovePostProcessingEffect(Effect effectToRemove)
        {
            PostProcessingEffects.Remove(effectToRemove);
        }

        /// <summary>
        /// Removes a post-processing effect at a particular index.
        /// </summary>
        /// <param name="index">The index to remove the post-processing effect at.</param>
        public void RemovePostProcessingEffect(int index)
        {
            PostProcessingEffects.RemoveAt(index);
        }

        public void RemoveAllPostProcessingEffects()
        {
            PostProcessingEffects.Clear();
        }

        /// <summary>
        /// Prepares a list of Renderers with the specified RenderingSettings for rendering.
        /// Renderers with the same shader are grouped together for batching.
        /// </summary>
        /// <param name="renderingSettings">The RenderingSettings to apply for these renderers.</param>
        /// <param name="renderers">A list of Renderers to prepare for rendering.</param>
        public void SetupRendering(RenderingSettings renderingSettings, List<Renderer> renderers)
        {
            //RenderSettings = renderingSettings;

            List<RenderBatch> renderBatches = new List<RenderBatch>();

            //Put the renderers into batches with the appropriate shader
            for (int i = 0; i < renderers.Count; i++)
            {
                Renderer renderer = renderers[i];

                RenderBatch batch = renderBatches.Find((rBatch) => rBatch.AppliedEffect == renderer.Shader);

                if (batch == null)
                {
                    batch = new RenderBatch(renderer.Shader);
                    renderBatches.Add(batch);
                }

                batch.Renderers.Add(renderer);
            }

            RenderGroups.Add(new RenderingGroup(renderingSettings, renderBatches));
        }

        public void PerformRendering()
        {
            for (int i = 0; i < RenderGroups.Count; i++)
            {
                RenderingGroup renderGroup = RenderGroups[i];
                RenderingSettings renderSettings = renderGroup.Rendersettings;

                for (int j = 0; j < renderGroup.RenderBatches.Count; j++)
                {
                    RenderBatch curRenderBatch = renderGroup.RenderBatches[j];

                    DrawBatch(renderSettings.spriteBatch, renderSettings.spriteSortMode, renderSettings.blendState, renderSettings.samplerState,
                        curRenderBatch.AppliedEffect, renderSettings.transformMatrix, curRenderBatch.Renderers);
                }

                //Clear each RenderingGroup's RenderBatches
                renderGroup.RenderBatches.Clear();
            }

            //Clear all rendering groups
            RenderGroups.Clear();

            //for (int i = 0; i < RenderBatches.Count; i++)
            //{
            //    RenderBatch curRenderBatch = RenderBatches[i];
            //
            //    DrawBatch(RenderSettings.spriteBatch, RenderSettings.spriteSortMode, RenderSettings.blendState, RenderSettings.samplerState,
            //        curRenderBatch.AppliedEffect, RenderSettings.transformMatrix, curRenderBatch.Renderers);
            //}
            //
            //RenderBatches.Clear();
        }

        private void StartBatch(SpriteBatch sb, SpriteSortMode spriteSortMode, BlendState blendState, SamplerState samplerState,
            DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect shader, Matrix? transformMatrix)
        {
            CurrentBatch = sb;
            CurrentBatch.Begin(spriteSortMode, blendState, samplerState, depthStencilState, rasterizerState, shader, transformMatrix);
        }

        private void EndCurrentBatch()
        {
            CurrentBatch.End();
            CurrentBatch = null;
        }

        private void DrawBatch(SpriteBatch sb, SpriteSortMode spriteSortMode, BlendState blendState, SamplerState samplerState,
            Effect effect, Matrix? transformMatrix, List<Renderer> renderableObjs)
        {
            StartBatch(sb, spriteSortMode, blendState, samplerState, null, null, effect, transformMatrix);

            for (int i = 0; i < renderableObjs.Count; i++)
            {
                renderableObjs[i].Render();
            }

            EndCurrentBatch();
        }

        public void StartDraw()
        {
            graphicsDevice.SetRenderTarget(MainRenderTarget);
            graphicsDevice.Clear(Color.CornflowerBlue);
        }

        public void EndDraw()
        {
            //If there are no post-processing effects, don't draw any
            if (PostProcessingCount <= 0)
            {
                //Render directly to the backbuffer
                graphicsDevice.SetRenderTarget(null);
                graphicsDevice.Clear(Color.CornflowerBlue);

                StartBatch(spriteBatch, SpriteSortMode.Texture, null, null, null, null, null, null);

                CurrentBatch.Draw(MainRenderTarget, new Rectangle(0, 0, MainRenderTarget.Width, MainRenderTarget.Height), null, Color.White);

                EndCurrentBatch();
            }
            //Draw all post-processing effects if there are any
            else
            {
                for (int i = 0; i < PostProcessingCount; i++)
                {
                    //Keep rendering to the RenderTarget until the last effect
                    //The last effect will be rendered to the backbuffer
                    if (i == (PostProcessingCount - 1))
                    {
                        graphicsDevice.SetRenderTarget(null);
                        graphicsDevice.Clear(Color.CornflowerBlue);
                    }
                    
                    StartBatch(spriteBatch, SpriteSortMode.Texture, null, null, null, null, PostProcessingEffects[i], null);

                    CurrentBatch.Draw(MainRenderTarget, new Rectangle(0, 0, MainRenderTarget.Width, MainRenderTarget.Height), null, Color.White);

                    EndCurrentBatch();
                }
            }

            //Get rendering metrics
            RenderingMetrics = graphicsDevice.Metrics;
        }

        public void DrawSprite(Texture2D tex, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin,
            Vector2 scale, SpriteEffects spriteEffects, float depth)
        {
            CurrentBatch.Draw(tex, position, sourceRect, color, rotation, origin, scale, spriteEffects, depth);
        }

        public void DrawSprite(Texture2D tex, Rectangle destRectangle, Rectangle? sourceRect, Color color, float rotation, Vector2 origin,
            SpriteEffects spriteEffects, float depth)
        {
            CurrentBatch.Draw(tex, destRectangle, sourceRect, color, rotation, origin, spriteEffects, depth);
        }

        /// <summary>
        /// Contains a list of RenderBatches to render with the specified RenderingSettings.
        /// </summary>
        private class RenderingGroup
        {
            public RenderingSettings Rendersettings = default(RenderingSettings);
            public List<RenderBatch> RenderBatches = null;

            public RenderingGroup(RenderingSettings renderSettings, List<RenderBatch> renderBatches)
            {
                Rendersettings = renderSettings;
                RenderBatches = renderBatches;
            }
        }

        /// <summary>
        /// Contains a lists of Renderers to render with the specified shader.
        /// </summary>
        private class RenderBatch
        {
            public Effect AppliedEffect = null;
            public List<Renderer> Renderers = new List<Renderer>();

            public RenderBatch(Effect appliedEffect)
            {
                AppliedEffect = appliedEffect;
            }
        }

        /// <summary>
        /// Settings for rendering.
        /// </summary>
        public struct RenderingSettings
        {
            public SpriteBatch spriteBatch;
            public SpriteSortMode spriteSortMode;
            public BlendState blendState;
            public SamplerState samplerState;
            public DepthStencilState depthStencilState;
            public RasterizerState rasterizerState;
            public Matrix? transformMatrix;

            public RenderingSettings(SpriteBatch sb, SpriteSortMode ssm, BlendState bs, SamplerState ss, DepthStencilState dss,
                RasterizerState rs, Matrix? transMatrix)
            {
                spriteBatch = sb;
                spriteSortMode = ssm;
                blendState = bs;
                samplerState = ss;
                depthStencilState = dss;
                rasterizerState = rs;
                transformMatrix = transMatrix;
            }
        }
    }
}
