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
    /// A render layer of the scene. All objects in it are rendered with certain settings.
    /// </summary>
    public sealed class RenderLayer
    {
        /// <summary>
        /// The order of the layer. Higher orders are rendered after.
        /// Layers with the same order are rendered in an arbitrary order in relation to each other.
        /// </summary>
        public int LayerOrder { get; set; } = 0;

        /// <summary>
        /// The RenderingSettings used for the layer.
        /// </summary>
        public RenderingSettings RenderSettings = default(RenderingSettings);

        public RenderLayer(int layerOrder, RenderingSettings renderSettings)
        {
            LayerOrder = layerOrder;
            RenderSettings = renderSettings;
        }

        /// <summary>
        /// Renders the RenderLayer with a set of Renderers.
        /// </summary>
        /// <param name="renderers">An IList of Renderers to render.</param>
        public void Render(in IList<Renderer> renderers)
        {
            //Return if there's nothing to render
            if (renderers == null || renderers.Count == 0)
                return;

            List<RenderBatch> renderBatches = new List<RenderBatch>();

            //Go through all the Renderers and put them into batches
            for (int i = 0; i < renderers.Count; i++)
            {
                Renderer renderer = renderers[i];

                //Find the batch using the Renderer's shader
                RenderBatch batch = renderBatches.Find((rBatch) => rBatch.AppliedEffect == renderer.Shader);

                //If no such batch exists, add a new one
                if (batch == null)
                {
                    batch = new RenderBatch(renderer.Shader);
                    renderBatches.Add(batch);
                }

                //Add this Renderer to the batch
                batch.Renderers.Add(renderer);
            }

            //Render the batches
            for (int i = 0; i < renderBatches.Count; i++)
            {
                RenderBatch batch = renderBatches[i];

                Matrix? matrix = null;
                if (RenderSettings.UseCameraMatrix == true) matrix = Camera2D.Instance.TransformMatrix;

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

            renderBatches.Clear();
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

            public RenderingSettings(SpriteBatch sb, SpriteSortMode ssm, BlendState bs, SamplerState ss, DepthStencilState dss,
                RasterizerState rs, bool useCameraMatrix)
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
