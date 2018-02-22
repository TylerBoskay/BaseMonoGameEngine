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

        /// <summary>
        /// A list of post-processing shaders to apply. They'll be applied in order.
        /// </summary>
        private readonly List<Effect> PostProcessingEffects = new List<Effect>();

        private RenderingManager()
        {
            
        }

        public void CleanUp()
        {
            RemoveAllPostProcessingEffects();
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

                List<Renderer> renderersInLayer = allRenderers.FindAll((renderer) => renderer.Order == layer.LayerOrder);

                //Render the layer
                layer.Render(renderersInLayer);
            }
        }

        public void StartBatch(SpriteBatch sb, SpriteSortMode spriteSortMode, BlendState blendState, SamplerState samplerState,
            DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect shader, Matrix? transformMatrix)
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
    }
}
