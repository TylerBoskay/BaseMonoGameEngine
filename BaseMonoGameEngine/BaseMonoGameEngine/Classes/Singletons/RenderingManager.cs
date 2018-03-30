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

        #region Event Fields

        public delegate void ScreenResized(Vector2 newSize);
        public event ScreenResized ScreenResizedEvent = null;

        #endregion

        /// <summary>
        /// Rendering metrics. This data is obtained after all rendering is complete for the frame.
        /// </summary>
        public GraphicsMetrics RenderingMetrics = default(GraphicsMetrics);

        public GraphicsDeviceManager graphicsDeviceManager { get; private set; } = null;
        public GraphicsDevice graphicsDevice => graphicsDeviceManager?.GraphicsDevice;

        private GameWindow gameWindow { get; set; } = null;

        public SpriteBatch spriteBatch { get; private set; } = null;

        /// <summary>
        /// The current SpriteBatch that has started. If null, then a batch hasn't started.
        /// </summary>
        public SpriteBatch CurrentBatch { get; private set; } = null;

        /// <summary>
        /// The dimensions of the back buffer.
        /// </summary>
        public Vector2 BackBufferDimensions
        {
            get => new Vector2(graphicsDevice.PresentationParameters.BackBufferWidth, graphicsDevice.PresentationParameters.BackBufferHeight);
        }

        /// <summary>
        /// The number of post-processing shaders in effect.
        /// </summary>
        public int PostProcessingCount => PostProcessingEffects.Count;

        /// <summary>
        /// The RenderTarget with the final image data at the end of rendering.
        /// </summary>
        public RenderTarget2D FinalRenderTarget { get; private set; } = null;

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

        private Color clearColor = Color.CornflowerBlue;

        /// <summary>
        /// The color to clear the screen with.
        /// </summary>
        public ref readonly Color ClearColor => ref clearColor;

        /// <summary>
        /// Whether rendering has started or not.
        /// </summary>
        public bool StartedRendering { get; private set; } = false;

        private RenderingManager()
        {
            
        }

        public void CleanUp()
        {
            RemoveAllPostProcessingEffects();
            
            LayerRenderTargets.Clear();

            MainRenderTarget.Dispose();
            PPRenderTarget.Dispose();
            spriteBatch.Dispose();

            StartedRendering = false;

            ScreenResizedEvent = null;

            gameWindow.ClientSizeChanged -= GameWindowSizeChanged;
        }

        public void Initialize(GraphicsDeviceManager graphicsDeviceMngr, GameWindow gameWdw, Vector2 screenSize)
        {
            graphicsDeviceManager = graphicsDeviceMngr;
            gameWindow = gameWdw;
            spriteBatch = new SpriteBatch(graphicsDevice);

            ResizeWindow(screenSize);

            StartedRendering = false;

            gameWindow.ClientSizeChanged -= GameWindowSizeChanged;
            gameWindow.ClientSizeChanged += GameWindowSizeChanged;
        }

        private void GameWindowSizeChanged(object sender, EventArgs e)
        {
            GameWindow window = gameWindow;

            if (window != null)
            {
                ResizeWindow(new Vector2(window.ClientBounds.Width, window.ClientBounds.Height));
            }
        }

        /// <summary>
        /// Resizes the window.
        /// </summary>
        /// <param name="newSize"></param>
        public void ResizeWindow(Vector2 newSize)
        {
            graphicsDeviceManager.PreferredBackBufferWidth = (int)newSize.X;
            graphicsDeviceManager.PreferredBackBufferHeight = (int)newSize.Y;

            graphicsDeviceManager.ApplyChanges();

            FinalRenderTarget = null;

            RenderingGlobals.ResizeRenderTarget(ref MainRenderTarget, newSize);
            RenderingGlobals.ResizeRenderTarget(ref PPRenderTarget, newSize);

            FinalRenderTarget = MainRenderTarget;

            ScreenResizedEvent?.Invoke(newSize);
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
                Debug.LogError("Attempting to render a null scene!");
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

                //Only render and add this layer's RenderTarget if there's something to render
                if (renderersInLayer.Count != 0)
                {
                    //Render the layer
                    layer.Render(renderersInLayer, scene.Camera);

                    //Add the RenderTarget for this layer
                    if (LayerRenderTargets.Contains(layer.RendTarget) == false)
                    {
                        LayerRenderTargets.Add(layer.RendTarget);
                    }
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
            //Start with no RenderTarget
            graphicsDevice.SetRenderTarget(null);

            StartedRendering = true;
        }

        public void EndDraw()
        {
            //Set to the main RenderTarget and clear the screen
            graphicsDevice.SetRenderTarget(MainRenderTarget);
            graphicsDevice.Clear(ClearColor);

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

            //Handle rendering multiple post-processing effects with two RenderTargets
            RenderTarget2D renderToTarget = PPRenderTarget;
            RenderTarget2D renderTarget = MainRenderTarget;

            //Draw all post-processing effects
            for (int i = 0; i < PostProcessingCount; i++)
            {
                //Swap to the RenderTarget, which will obtain updated data from the other one
                graphicsDevice.SetRenderTarget(renderToTarget);
                graphicsDevice.Clear(ClearColor);

                //Draw the RenderTarget with the shader
                StartBatch(spriteBatch, SpriteSortMode.Texture, BlendState.AlphaBlend, null, null, null, PostProcessingEffects[i], null);

                CurrentBatch.Draw(renderTarget, new Rectangle(0, 0, renderTarget.Width, renderTarget.Height), null, Color.White);

                EndCurrentBatch();

                //Swap RenderTargets; the one that was just rendered to has the updated image data
                UtilityGlobals.Swap(ref renderToTarget, ref renderTarget);
            }

            //Set the final RenderTarget to the one with the final image data
            FinalRenderTarget = renderTarget;

            //Perform a final draw to the backbuffer using the final RenderTarget
            graphicsDevice.SetRenderTarget(null);
            graphicsDevice.Clear(ClearColor);

            StartBatch(spriteBatch, SpriteSortMode.Texture, BlendState.AlphaBlend, null, null, null, null, null);
            CurrentBatch.Draw(FinalRenderTarget, new Rectangle(0, 0, FinalRenderTarget.Width, FinalRenderTarget.Height), null, Color.White);
            EndCurrentBatch();

            //Get rendering metrics
            RenderingMetrics = graphicsDevice.Metrics;

            StartedRendering = false;
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
