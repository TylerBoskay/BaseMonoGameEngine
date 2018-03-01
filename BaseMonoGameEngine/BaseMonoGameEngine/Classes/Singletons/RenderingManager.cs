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
    /// Manages rendering scenes.
    /// <para>This is a Singleton.</para>
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
        private RenderTarget2D PPRenderTarget = null;

        /// <summary>
        /// A list of post-processing shaders to apply. They'll be applied in order.
        /// </summary>
        private readonly List<Effect> PostProcessingEffects = new List<Effect>();

        /// <summary>
        /// The list of layered RenderTargets. These are rendered, in order, to the MainRenderTarget, which is then drawn to the backbuffer.
        /// </summary>
        private readonly List<RenderTarget2D> LayerRenderTargets = new List<RenderTarget2D>();

        private RenderingManager()
        {
            
        }

        public void CleanUp()
        {
            RemoveAllPostProcessingEffects();
            for (int i = 0; i < LayerRenderTargets.Count; i++)
            {
                LayerRenderTargets[i].Dispose();
            }

            LayerRenderTargets.Clear();

            MainRenderTarget.Dispose();
            PPRenderTarget.Dispose();
        }

        public void Initialize(GraphicsDeviceManager graphicsDeviceMngr)
        {
            graphicsDeviceManager = graphicsDeviceMngr;
            spriteBatch = new SpriteBatch(graphicsDevice);

            MainRenderTarget = new RenderTarget2D(graphicsDevice, RenderingGlobals.WindowWidth, RenderingGlobals.WindowHeight);
            PPRenderTarget = new RenderTarget2D(graphicsDevice, RenderingGlobals.WindowWidth, RenderingGlobals.WindowHeight);
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
        /// Renders a scene.
        /// </summary>
        /// <param name="scene">The scene to render.</param>
        public void PerformRendering(in GameScene scene)
        {
            //Don't bother if the scene is null
            if (scene == null)
            {
                Debug.LogError("Attempting to render with a null scene!");
                return;
            }

            //Get all renderers and render layers in the scene
            List<Renderer> allRenderers = scene.GetActiveVisibleRenderersInScene();
            List<RenderLayer> renderLayers = scene.GetRenderLayersInScene();

            //Go through all render layers, find all Renderers that match the layer order, and render the layer with those Renderers
            for (int i = 0; i < renderLayers.Count; i++)
            {
                RenderLayer layer = renderLayers[i];

                //If the layer is disabled, don't render it
                if (layer.Enabled == false)
                    continue;

                List<Renderer> renderersInLayer = new List<Renderer>();

                for (int j = allRenderers.Count - 1; j >= 0; j--)
                {
                    Renderer rend = allRenderers[j];
                    if (rend.Order == layer.LayerOrder)
                    {
                        renderersInLayer.Add(rend);
                        allRenderers.RemoveAt(j);
                    }
                }

                //Render the layer
                layer.Render(renderersInLayer);

                //Add the RenderTarget for this layer
                if (LayerRenderTargets.Contains(layer.RendTarget) == false)
                {
                    LayerRenderTargets.Add(layer.RendTarget);
                }
            }
        }

        public void StartBatch(in SpriteBatch sb, in SpriteSortMode spriteSortMode, in BlendState blendState, in SamplerState samplerState,
            in DepthStencilState depthStencilState, in RasterizerState rasterizerState, in Effect shader, in Matrix? transformMatrix)
        {
            CurrentBatch = sb;
            CurrentBatch.Begin(spriteSortMode, blendState, samplerState, depthStencilState, rasterizerState, shader, transformMatrix);
        }

        public void EndCurrentBatch()
        {
            CurrentBatch.End();
            CurrentBatch = null;
        }

        public void StartDraw()
        {
            //Start with no RenderTarget and clear the screen
            graphicsDevice.SetRenderTarget(null);
            graphicsDevice.Clear(Color.CornflowerBlue);
        }

        public void EndDraw()
        {
            //Set to the main RenderTarget and clear the screen
            graphicsDevice.SetRenderTarget(MainRenderTarget);
            graphicsDevice.Clear(Color.CornflowerBlue);

            //Go through all RenderTargets in all layers and draw their contents, in order, to the main RenderTarget
            for (int i = 0; i < LayerRenderTargets.Count; i++)
            {
                RenderTarget2D layerTarget = LayerRenderTargets[i];
                StartBatch(spriteBatch, SpriteSortMode.Texture, BlendState.AlphaBlend, null, null, null, null, null);

                CurrentBatch.Draw(layerTarget, new Rectangle(0, 0, layerTarget.Width, layerTarget.Height), null, Color.White);

                EndCurrentBatch();
            }

            //Clear layered targets, then prepare for post-processing effects
            LayerRenderTargets.Clear();

            //If there are no post-processing effects, don't draw any
            if (PostProcessingCount <= 0)
            {
                //Render directly to the backbuffer
                graphicsDevice.SetRenderTarget(null);
                graphicsDevice.Clear(Color.CornflowerBlue);

                StartBatch(spriteBatch, SpriteSortMode.Texture, BlendState.AlphaBlend, null, null, null, null, null);

                CurrentBatch.Draw(MainRenderTarget, new Rectangle(0, 0, MainRenderTarget.Width, MainRenderTarget.Height), null, Color.White);

                EndCurrentBatch();
            }
            //Draw all post-processing effects if there are any
            else
            {
                //Handle rendering multiple post-processing effects with two RenderTargets
                RenderTarget2D renderToTarget = PPRenderTarget;
                RenderTarget2D renderTarget = MainRenderTarget;

                for (int i = 0; i < PostProcessingCount; i++)
                {
                    //Keep rendering to the RenderTarget until the last effect
                    //The last effect will be rendered to the backbuffer
                    if (i == (PostProcessingCount - 1))
                    {
                        graphicsDevice.SetRenderTarget(null);
                        graphicsDevice.Clear(Color.CornflowerBlue);
                    }
                    //Swap to the RenderTarget, which will obtain updated data from the other one
                    else
                    {
                        graphicsDevice.SetRenderTarget(renderToTarget);
                        graphicsDevice.Clear(Color.CornflowerBlue);
                    }

                    //Draw the RenderTarget with the shader
                    StartBatch(spriteBatch, SpriteSortMode.Texture, BlendState.AlphaBlend, null, null, null, PostProcessingEffects[i], null);

                    CurrentBatch.Draw(renderTarget, new Rectangle(0, 0, MainRenderTarget.Width, MainRenderTarget.Height), null, Color.White);

                    EndCurrentBatch();

                    //Swap RenderTargets; the one that was rendered to has the updated data
                    UtilityGlobals.Swap(ref renderToTarget, ref renderTarget);
                }
            }

            //Get rendering metrics
            RenderingMetrics = graphicsDevice.Metrics;
        }

        public void DrawSprite(in Texture2D tex, in Vector2 position, in Rectangle? sourceRect, in Color color, in float rotation,
            in Vector2 origin, in Vector2 scale, in SpriteEffects spriteEffects, in float depth)
        {
            CurrentBatch.Draw(tex, position, sourceRect, color, rotation, origin, scale, spriteEffects, depth);
        }

        public void DrawSprite(in Texture2D tex, in Rectangle destRectangle, in Rectangle? sourceRect, in Color color, in float rotation,
            in Vector2 origin, in SpriteEffects spriteEffects, in float depth)
        {
            CurrentBatch.Draw(tex, destRectangle, sourceRect, color, rotation, origin, spriteEffects, depth);
        }
    }
}
