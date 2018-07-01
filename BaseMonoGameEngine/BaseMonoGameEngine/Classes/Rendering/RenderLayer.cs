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
    /// A render layer of the scene. All objects in it are rendered with certain settings.
    /// </summary>
    public sealed class RenderLayer : IEnableable, ICleanup
    {
        /// <summary>
        /// The RenderTarget of the layer.
        /// All layers render to their own RenderTarget.
        /// </summary>
        public RenderTarget2D RendTarget { get; private set; } = null;

        private RenderTarget2D RTarget = null;
        private RenderTarget2D PPRTarget = null;

        /// <summary>
        /// The order of the layer. Higher orders are rendered after.
        /// Layers with the same order are rendered in an arbitrary order in relation to each other.
        /// </summary>
        public int LayerOrder { get; set; } = 0;

        /// <summary>
        /// The RenderingSettings used for the layer.
        /// </summary>
        public RenderingSettings RenderSettings = default(RenderingSettings);

        /// <summary>
        /// Whether the layer is enabled or not. Disabled layers are not rendered.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The post-processing layer effects being applied to this layer.
        /// </summary>
        private readonly List<Effect> LayerEffects = new List<Effect>();

        /// <summary>
        /// The current list of RenderBatches.
        /// </summary>
        private readonly List<RenderBatch> RenderBatches = new List<RenderBatch>();

        public RenderLayer(in int layerOrder, in RenderingSettings renderSettings)
        {
            //Keep the render targets at base resolution
            Vector2 rtSize = new Vector2(RenderingGlobals.BaseResolutionWidth, RenderingGlobals.BaseResolutionHeight);

            RTarget = new RenderTarget2D(RenderingManager.Instance.graphicsDevice, (int)rtSize.X, (int)rtSize.Y);
            PPRTarget = new RenderTarget2D(RenderingManager.Instance.graphicsDevice, (int)rtSize.X, (int)rtSize.Y);
            RendTarget = RTarget;

            LayerOrder = layerOrder;
            RenderSettings = renderSettings;

            RenderingManager.Instance.ScreenResizedEvent -= ScreenResized;
            RenderingManager.Instance.ScreenResizedEvent += ScreenResized;
        }

        private void ScreenResized(in Vector2 newSize)
        {
            
        }

        public void CleanUp()
        {
            //Clear all current batches
            RenderBatches.Clear();

            //Clear all current layer effects
            RemoveAllLayerEffects();

            RTarget.Dispose();
            PPRTarget.Dispose();

            RenderingManager.Instance.ScreenResizedEvent -= ScreenResized;
        }

        /// <summary>
        /// Adds a post-processing effect for this layer.
        /// </summary>
        /// <param name="layerEffect">The Effect to add.</param>
        public void AddLayerEffect(in Effect layerEffect)
        {
            if (layerEffect == null) return;
        
            LayerEffects.Add(layerEffect);
        }
        
        /// <summary>
        /// Removes a post-processing effect from this layer.
        /// </summary>
        /// <param name="layerEffect">The Effect to remove.</param>
        public void RemoveLayerEffect(in Effect layerEffect)
        {
            LayerEffects.Remove(layerEffect);
        }

        /// <summary>
        /// Removes all post-processing effects from a layer.
        /// </summary>
        public void RemoveAllLayerEffects()
        {
            LayerEffects.Clear();
        }

        /// <summary>
        /// Renders the RenderLayer with a set of Renderers.
        /// </summary>
        /// <param name="renderers">An IList of Renderers to render.</param>
        /// /// <param name="camera">The Camera for the scene.</param>
        public void Render(in IList<Renderer> renderers, in Camera2D camera)
        {
            //Return if there's nothing to render
            if (renderers == null || renderers.Count == 0)
                return;
            //Set target reference
            RendTarget = RTarget;

            //Draw to this layer's RenderTarget and initially fill it with transparency
            RenderingManager.Instance.graphicsDevice.SetRenderTarget(RendTarget);
            RenderingManager.Instance.graphicsDevice.Clear(Color.Transparent);

            //Go through all the Renderers and put them into batches
            for (int i = 0; i < renderers.Count; i++)
            {
                Renderer renderer = renderers[i];

                //Find the batch using the Renderer's shader
                RenderBatch batch = RenderBatches.Find((rBatch) => rBatch.AppliedEffect == renderer.Shader);

                //If no such batch exists, add a new one
                if (batch == null)
                {
                    batch = new RenderBatch(renderer.Shader);
                    RenderBatches.Add(batch);
                }

                //Add this Renderer to the batch
                batch.Renderers.Add(renderer);
            }

            //Render the batches
            for (int i = 0; i < RenderBatches.Count; i++)
            {
                RenderBatch batch = RenderBatches[i];

                Matrix? matrix = null;
                if (RenderSettings.UseCameraMatrix == true) matrix = camera?.TransformMatrix;

                //Start the batch
                RenderingManager.Instance.StartBatch(RenderSettings.spriteBatch, RenderSettings.spriteSortMode, RenderSettings.blendState,
                    RenderSettings.samplerState, RenderSettings.depthStencilState, RenderSettings.rasterizerState, batch.AppliedEffect, matrix);

                //Make all Renderers render in this batch
                for (int j = 0; j < batch.Renderers.Count; j++)
                {
                    batch.Renderers[j].Render();
                }

                //End the batch
                RenderingManager.Instance.EndCurrentBatch();
            }

            //Clear the batches
            RenderBatches.Clear();

            //Handle rendering multiple post-processing effects with two RenderTargets
            RenderTarget2D renderToTarget = PPRTarget;
            RenderTarget2D renderTarget = RTarget;

            for (int i = 0; i < LayerEffects.Count; i++)
            {
                RenderingManager.Instance.graphicsDevice.SetRenderTarget(renderToTarget);
                RenderingManager.Instance.graphicsDevice.Clear(Color.Transparent);

                //Add the layer post-processing effects to this layer
                RenderingManager.Instance.StartBatch(RenderSettings.spriteBatch, SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, LayerEffects[i], null);
            
                RenderingManager.Instance.CurrentBatch.Draw(renderTarget, new Rectangle(0, 0, RTarget.Width, RTarget.Height), null, Color.White);
            
                RenderingManager.Instance.EndCurrentBatch();

                //Swap RenderTargets; the one that was rendered to has the updated data
                UtilityGlobals.Swap(ref renderToTarget, ref renderTarget);
            }

            //Update the RenderTarget to the one with the most updated data
            RendTarget = renderTarget;
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
            public bool UseCameraMatrix;

            public RenderingSettings(in SpriteBatch sb, in SpriteSortMode ssm, in BlendState bs, in SamplerState ss,
                in DepthStencilState dss, in RasterizerState rs, in bool useCameraMatrix)
            {
                spriteBatch = sb;
                spriteSortMode = ssm;
                blendState = bs;
                samplerState = ss;
                depthStencilState = dss;
                rasterizerState = rs;
                UseCameraMatrix = useCameraMatrix;
            }
        }
    }
}
