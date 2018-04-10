using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TDMonoGameEngine
{
    /// <summary>
    /// The entry point to the engine.
    /// </summary>
    public class Engine : Game
    {
        private GraphicsDeviceManager graphics;
        private CrashHandler crashHandler = null;

        /// <summary>
        /// The game window.
        /// </summary>
        public GameWindow GameWindow => Window;

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);

            crashHandler = new CrashHandler();
            
            //false for variable timestep, true for fixed
            Time.FixedTimeStep = true;
            Time.VSyncEnabled = true;
            
            Window.AllowUserResizing = true;

            //MonoGame sets x32 MSAA by default if enabled
            //If enabled and we want a lower value, set the value in the PreparingDeviceSettings event
            graphics.PreferMultiSampling = false;
            graphics.SynchronizeWithVerticalRetrace = Time.VSyncEnabled;

            //Make switching to full screen fast but less efficient; we want to switch as fast as possible
            //On top of that, this allows for borderless full screen on DesktopGL
            graphics.HardwareModeSwitch = false;

            graphics.PreparingDeviceSettings -= OnPreparingDeviceSettings;
            graphics.PreparingDeviceSettings += OnPreparingDeviceSettings;
        }

        private void OnPreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            //Prepare any graphics device settings here
            //Note that OpenGL does not provide a way to set the adapter; the driver is responsible for that
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsFixedTimeStep = Time.FixedTimeStep;

            AssetManager.Instance.Initialize(Content);
            RenderingManager.Instance.Initialize(graphics, GameWindow, new Vector2(RenderingGlobals.BaseResolutionWidth, RenderingGlobals.BaseResolutionHeight));

            Camera2D camera = new Camera2D();
            camera.SetBounds(new Rectangle(0, 0, (int)RenderingManager.Instance.BackBufferDimensions.X, (int)RenderingManager.Instance.BackBufferDimensions.Y));

            GameScene scene = new GameScene();
            SceneManager.Instance.LoadScene(scene);
            SceneManager.Instance.ActiveScene.SetCamera(camera);

            scene.AddRenderLayer(new RenderLayer(1, new RenderLayer.RenderingSettings(RenderingManager.Instance.spriteBatch,
                SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, false)));

            Player player1 = new Player();
            scene.AddSceneObject(player1);
            scene.AddSceneObject(new TestEnemy());
            scene.AddSceneObject(new TestEnemy2());
            scene.AddSceneObject(new TestGameHUD(player1, 1));
            scene.AddSceneObject(new AfterImages(player1.spriteRenderer, player1.AnimationManager, 3, 8, .25f,
                AfterImages.AfterImageAlphaSetting.FadeOff, AfterImages.AfterImageAnimSetting.Current));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            graphics.PreparingDeviceSettings -= OnPreparingDeviceSettings;

            AssetManager.Instance.CleanUp();
            SoundManager.Instance.CleanUp();
            SceneManager.Instance.CleanUp();
            RenderingManager.Instance.CleanUp();

            if (EventManager.HasInstance == true)
                EventManager.Instance.CleanUp();

            Debug.DebugCleanup();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            PreUpdate(gameTime);

            //This conditional is for enabling frame advance debugging
            if (Debug.DebugPaused == false || Debug.AdvanceNextFrame == true)
                MainUpdate(gameTime);

            PostUpdate(gameTime);
        }

        /// <summary>
        /// Any update logic that should occur immediately before the main Update loop
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void PreUpdate(GameTime gameTime)
        {
            Time.UpdateTime(gameTime);
            Debug.DebugUpdate();
        }

        /// <summary>
        /// The main update loop
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void MainUpdate(GameTime gameTime)
        {
            if (EventManager.HasInstance == true)
                EventManager.Instance.Update();

            SceneManager.Instance.ActiveScene.Update();
        }
        
        /// <summary>
        /// Any update logic that should occur immediately after the main Update loop
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void PostUpdate(GameTime gameTime)
        {
            SoundManager.Instance.Update();

            //Frame advance debugging for input
            if (Debug.DebugPaused == false || Debug.AdvanceNextFrame == true)
            {
                MouseInput.UpdateMouseState();
                KeyboardInput.UpdateKeyboardState();
                Input.UpdateInput();
            }

            base.Update(gameTime);

            //Set time step and VSync settings
            IsFixedTimeStep = Time.FixedTimeStep;
            graphics.SynchronizeWithVerticalRetrace = Time.VSyncEnabled;

            //This should always be at the end of PostUpdate()
            if (Time.UpdateFPS == true)
            {
                //Set the FPS - TimeSpan normally rounds, so to be precise we'll create them from ticks
                double val = Math.Round(Time.FPS <= 0d ? 0d : (1d / Time.FPS), 7);
                TargetElapsedTime = TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond * val));
            }
        }

        /// <summary>
        /// Anything that should occur immediately before the main Draw method
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void PreDraw()
        {
            Time.UpdateFrames();
            
            RenderingManager.Instance.StartDraw();
            if (Debug.DebugEnabled == true)
            {
                Debug.DebugStartDraw();
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            PreDraw();

            RenderingManager.Instance.PerformRendering(SceneManager.Instance.ActiveScene);
            Debug.DebugDraw();

            base.Draw(gameTime);

            PostDraw();
        }

        private void PostDraw()
        {
            RenderingManager.Instance.EndDraw();

            //Draw debug information on top of everything else
            if (Debug.DebugEnabled == true)
            {
                Debug.DebugEndDraw();
            }
        }
    }
}
