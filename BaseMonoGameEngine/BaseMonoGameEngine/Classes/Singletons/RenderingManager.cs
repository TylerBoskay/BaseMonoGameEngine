using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
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

        #region Event Fields

        public delegate void ScreenResized(in Vector2 newSize);

        /// <summary>
        /// The event invoked when the game window is resized.
        /// </summary>
        public event ScreenResized ScreenResizedEvent = null;

        #endregion

        /// <summary>
        /// Rendering metrics. This data is obtained after all rendering is complete for the frame.
        /// </summary>
        public GraphicsMetrics RenderingMetrics { get; private set; } = default(GraphicsMetrics);

        public GraphicsDeviceManager graphicsDeviceManager { get; private set; } = null;
        public GraphicsDevice graphicsDevice => graphicsDeviceManager?.GraphicsDevice;

        /// <summary>
        /// The size of the monitor screen the game is being played on.
        /// </summary>
        public Vector2 ScreenSize => new Vector2(graphicsDevice.Adapter.CurrentDisplayMode.Width, graphicsDevice.Adapter.CurrentDisplayMode.Height);
        public float AspectRatio => graphicsDevice.Adapter.CurrentDisplayMode.AspectRatio;

        private GameWindow gameWindow { get; set; } = null;

        public SpriteBatch spriteBatch { get; private set; } = null;

        /// <summary>
        /// The current SpriteBatch that has started. If null, then a batch hasn't started.
        /// </summary>
        public SpriteBatch CurrentBatch { get; private set; } = null;

        /// <summary>
        /// The dimensions of the back buffer.
        /// </summary>
        public Vector2 BackBufferDimensions => new Vector2(graphicsDevice.PresentationParameters.BackBufferWidth,
            graphicsDevice.PresentationParameters.BackBufferHeight);

        /// <summary>
        /// The minimum size of the game window.
        /// </summary>
        public Vector2 MinWindowSize = Vector2.Zero;

        /// <summary>
        /// The maximum size of the game window.
        /// </summary>
        public Vector2 MaxWindowSize = Vector2.Zero;

        /// <summary>
        /// The number of post-processing shaders in effect.
        /// </summary>
        public int PostProcessingCount => PostProcessingEffects.Count;

        /// <summary>
        /// The RenderTarget with the final image data at the end of rendering.
        /// </summary>
        public RenderTarget2D FinalRenderTarget { get; private set; } = null;

        /// <summary>
        /// Tells whether the game is full screen or not.
        /// </summary>
        public bool IsFullscreen => graphicsDeviceManager.IsFullScreen;

        /// <summary>
        /// Tells whether the game window is borderless or not.
        /// </summary>
        public bool IsBorderless => gameWindow.IsBorderless;

        /// <summary>
        /// The size to render the final RenderTarget at in relation to the base resolution.
        /// This is always the back buffer size unless set to fullscreen.
        /// </summary>
        private Vector2 RTRenderSize = RenderingGlobals.BaseResolution;

        public RenderTarget2D GetMainRenderTarget => MainRenderTarget;

        private RenderTarget2D MainRenderTarget = null;
        private RenderTarget2D PPRenderTarget = null;

        /// <summary>
        /// The origin to render the RenderTarget.
        /// It's set to the center for better fullscreen scaling.
        /// </summary>
        private Vector2 RTOrigin = Vector2.Zero;

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

        public void Initialize(in GraphicsDeviceManager graphicsDeviceMngr, in GameWindow gameWdw, in Vector2 screenSize)
        {
            graphicsDeviceManager = graphicsDeviceMngr;
            gameWindow = gameWdw;
            spriteBatch = new SpriteBatch(graphicsDevice);

            ResizeWindow(screenSize, true);

            CenterWindow();

            //Keep the render targets at base resolution
            //They'll be upscaled or downscaled to the window size
            Vector2 size = new Vector2(RenderingGlobals.BaseResolutionWidth, RenderingGlobals.BaseResolutionHeight);

            RenderingGlobals.ResizeRenderTarget(ref MainRenderTarget, size);
            RenderingGlobals.ResizeRenderTarget(ref PPRenderTarget, size);

            FinalRenderTarget = MainRenderTarget;

            RTOrigin = FinalRenderTarget.GetCenterOrigin();

            StartedRendering = false;

            gameWindow.ClientSizeChanged -= GameWindowSizeChanged;
            gameWindow.ClientSizeChanged += GameWindowSizeChanged;
        }

        private void GameWindowSizeChanged(object sender, EventArgs e)
        {
            GameWindow window = gameWindow;

            if (window != null)
            {
                ResizeWindow(new Vector2(window.ClientBounds.Width, window.ClientBounds.Height), false);
            }
        }

        /// <summary>
        /// Clamps the window size to the minimum and maximum defined values.
        /// </summary>
        /// <param name="windowSize">The new size of the game window.</param>
        private void CheckWindowSize(ref Vector2 windowSize)
        {
            //If the max window size is 0, it's the default value and we shouldn't clamp to it
            float maxX = (MaxWindowSize.X > 0f) ? MaxWindowSize.X : int.MaxValue;
            float maxY = (MaxWindowSize.Y > 0f) ? MaxWindowSize.Y : int.MaxValue;

            windowSize.X = UtilityGlobals.Clamp(windowSize.X, MinWindowSize.X, maxX);
            windowSize.Y = UtilityGlobals.Clamp(windowSize.Y, MinWindowSize.Y, maxY);
        }

        /// <summary>
        /// Resizes the window. If this is called from <see cref="GameWindowSizeChanged(object, EventArgs)"/>, <paramref name="manualSet"/>
        /// should be false.
        /// </summary>
        /// <param name="newSize">The new size of the game window.</param>
        /// <param name="manualSet">Whether this new size should be manually set.
        /// This should be false if the game window is resized natively.</param>
        public void ResizeWindow(Vector2 newSize, in bool manualSet)
        {
            //Confirm window size
            CheckWindowSize(ref newSize);

            //We don't need to set the back buffer width and height since it's already set when resizing the window
            //Only set this explicitly if we should do so manually
            if (manualSet == true)
            {
                //Set the back buffer width and height and apply the changes
                graphicsDeviceManager.PreferredBackBufferWidth = (int)newSize.X;
                graphicsDeviceManager.PreferredBackBufferHeight = (int)newSize.Y;

                graphicsDeviceManager.ApplyChanges();
            }

            RTRenderSize = newSize;

            //If fullscreen, scale the RenderTarget appropriately
            if (IsFullscreen == true)
            {
                SetRenderTargetToFullscreenScale();
            }

            ScreenResizedEvent?.Invoke(newSize);
        }

        /// <summary>
        /// Sets whether the game is full screen or not.
        /// </summary>
        /// <param name="fullScreen">The value indicating full screen.</param>
        public void SetFullScreen(in bool fullScreen)
        {
            graphicsDeviceManager.IsFullScreen = fullScreen;
            graphicsDeviceManager.ApplyChanges();
        }

        /// <summary>
        /// Sets the borderless state of the game window.
        /// </summary>
        /// <param name="borderless">Whether to make the game window borderless or not.</param>
        public void SetWindowBorderless(in bool borderless)
        {
            gameWindow.IsBorderless = borderless;
        }

        /// <summary>
        /// Sets the position of the game window.
        /// </summary>
        /// <param name="pos">A Point representing the position of the game window.</param>
        public void SetWindowPosition(in Point pos)
        {
            gameWindow.Position = pos;
        }

        /// <summary>
        /// Sets the position of the game window to the center of the monitor screen.
        /// </summary>
        public void CenterWindow()
        {
            SetWindowPosition(((RenderingManager.Instance.ScreenSize / 2) - (RenderingManager.Instance.BackBufferDimensions / 2)).ToPoint());
        }

        /// <summary>
        /// Sets the final RenderTarget to fit within the size of the screen.
        /// </summary>
        private void SetRenderTargetToFullscreenScale()
        {
            //Set to the screen size over the base resolution - this fits it into the whole screen
            Vector2 finalSize = RenderingManager.Instance.ScreenSize / RenderingGlobals.BaseResolution;

            //Choose the lowest value out of the X or Y so it fits on the screen
            //We need 1:1 scaling of the RenderTarget to avoid stretching
            if (finalSize.X < finalSize.Y)
                finalSize.Y = finalSize.X;
            else if (finalSize.Y < finalSize.X)
                finalSize.X = finalSize.Y;

            //Finally, multiply by the base resolution to get the actual size
            RTRenderSize = finalSize * RenderingGlobals.BaseResolution;
        }

        /// <summary>
        /// Adds a post-processing effect.
        /// </summary>
        /// <param name="effect">The post-processing effect to add.</param>
        /// <param name="index">The index to insert the effect at. If less than 0, it will add it to the end of the list.</param>
        public void AddPostProcessingEffect(in Effect effect, in int index = -1)
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
        public void RemovePostProcessingEffect(in Effect effectToRemove)
        {
            PostProcessingEffects.Remove(effectToRemove);
        }

        /// <summary>
        /// Removes a post-processing effect at a particular index.
        /// </summary>
        /// <param name="index">The index to remove the post-processing effect at.</param>
        public void RemovePostProcessingEffect(in int index)
        {
            PostProcessingEffects.RemoveAt(index);
        }

        /// <summary>
        /// Removes all post-processing effects.
        /// </summary>
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

            //Everything was drawn at native resolution; scale to the window/screen size
            Vector2 resScale = RTRenderSize / new Vector2(RenderingGlobals.BaseResolutionWidth, RenderingGlobals.BaseResolutionHeight);

            StartBatch(spriteBatch, SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, null, null);
            CurrentBatch.Draw(FinalRenderTarget, BackBufferDimensions / 2, null, Color.White, 0f, RTOrigin, resScale, SpriteEffects.None, 0f);
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
