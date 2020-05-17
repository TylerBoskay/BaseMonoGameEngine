using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// The entry point to the engine.
    /// </summary>
    public class Engine : Game
    {
        //Delegate and event for losing window focus
        public delegate void OnLostFocus();

        /// <summary>
        /// The event invoked when the window loses focus. This is invoked at the start of the update loop.
        /// </summary>
        public event OnLostFocus LostFocusEvent = null;

        //Delegate and event for regaining window focus
        public delegate void OnRegainedFocus();

        /// <summary>
        /// The event invoked when the window regains focus. This is invoked at the start of the update loop.
        /// </summary>
        public event OnRegainedFocus RegainedFocusEvent = null;

        /// <summary>
        /// Tells if the game window was focused at the start of the update loop.
        /// </summary>
        public bool WasFocused { get; private set; } = false;

        private GraphicsDeviceManager graphics;

        /// <summary>
        /// The game window.
        /// </summary>
        public GameWindow GameWindow => Window;

        public static bool ShouldQuit { get; private set; } = false;

        /// <summary>
        /// The name of this game.
        /// </summary>
        public const string GameName = "BaseMonoGameEngine";

        /// <summary>
        /// Indicates whether to ignore audio errors. This is true if no audio device can be found at the start of the game.
        /// </summary>
        public static bool IgnoreAudioErrors { get; private set; } = false;

        public Engine()
        {
            Debug.Log("Starting up engine");

            Debug.Log("Initializing graphics device");

            Debug.Log($"OS: {Debug.DebugGlobals.GetOSInfo()} | Platform: {MonoGame.Framework.Utilities.PlatformInfo.MonoGamePlatform} | Renderer: {MonoGame.Framework.Utilities.PlatformInfo.GraphicsBackend}");

            graphics = new GraphicsDeviceManager(this);
            
            //false for variable timestep, true for fixed
            Time.TimeStep = TimestepSettings.Variable;
            Time.VSyncSetting = VSyncSettings.Enabled;
            
            Window.AllowUserResizing = true;
            IsMouseVisible = true;

            //MonoGame sets x32 MSAA by default if enabled
            //If enabled and we want a lower value, set the value in the PreparingDeviceSettings event
            graphics.PreferMultiSampling = false;

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
            //Set the culture to invariant
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Window.Title = GameName;
            MaxElapsedTime = Time.MaxElapsedTime;
            IsFixedTimeStep = Time.TimeStep == TimestepSettings.Fixed;

            Debug.Log($"GPU: {GraphicsDevice.Adapter.Description} | Display: {GraphicsDevice.Adapter.CurrentDisplayMode} | Widescreen: {GraphicsDevice.Adapter.IsWideScreen}");

            //Initialize the audio system
            try
            {
                Microsoft.Xna.Framework.Audio.SoundEffect.Initialize();
            }
            catch (Microsoft.Xna.Framework.Audio.NoAudioHardwareException)
            {
                Debug.Log("No audio hardware present. Disabling audio exception logs.");
                IgnoreAudioErrors = true;
            }

            AssetManager.Instance.Initialize(Content);

            //Dispose the original ContentManager since we set up our own and are no longer using it
            Content.Dispose();

            RenderingManager.Instance.Initialize(graphics, GameWindow, RenderingGlobals.BaseResolution);
            RenderingManager.Instance.CenterWindow();

            GameStateManager.Instance.ChangeGameState(new BlankGameState());

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

            SoundManager.Instance.CleanUp();
            AssetManager.Instance.CleanUp();
            GameStateManager.Instance.CleanUp();
            RenderingManager.Instance.CleanUp();

            Debug.DebugCleanup();

            LostFocusEvent = null;
            RegainedFocusEvent = null;
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
        private void PreUpdate(in GameTime gameTime)
        {
            //Tell if we change window focus state
            bool focused = IsActive;

            //Lost focus
            if (focused == false && WasFocused == true)
            {
                LostFocusEvent?.Invoke();
            }
            //Regained focus
            else if (focused == true && WasFocused == false)
            {
                RegainedFocusEvent?.Invoke();
            }

            //Set focus state
            WasFocused = focused;

            Debug.DebugUpdate();
            Time.UpdateTime(gameTime);
        }

        /// <summary>
        /// The main update loop
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void MainUpdate(in GameTime gameTime)
        {
            GameStateManager.Instance.Update();
        }
        
        /// <summary>
        /// Any update logic that should occur immediately after the main Update loop
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void PostUpdate(in GameTime gameTime)
        {
            SoundManager.Instance.Update();

            //Update mouse visibility
            MouseHider.Update();
            IsMouseVisible = MouseHider.MouseVisible;

            //Frame advance debugging for input
            if (Debug.DebugPaused == false || Debug.AdvanceNextFrame == true)
            {
                MouseInput.UpdateMouseState();
                KeyboardInput.UpdateKeyboardState();
                Input.UpdateInput();
            }

            base.Update(gameTime);

            //Set time step and VSync settings
            IsFixedTimeStep = Time.TimeStep == TimestepSettings.Fixed ? true : false;

            bool vSync = Time.VSyncSetting == VSyncSettings.Enabled ? true : false;

            if (graphics.SynchronizeWithVerticalRetrace != vSync)
            {
                graphics.SynchronizeWithVerticalRetrace = vSync;
                graphics.ApplyChanges();
            }

            MaxElapsedTime = Time.MaxElapsedTime;

            //If we should exit, do so
            if (ShouldQuit == true)
            {
                Exit();
                return;
            }

            /* This should always be at the end of PostUpdate() */
            TargetElapsedTime = Time.GetTimeSpanFromFPS(Time.FPS);

            //Prevent a game freeze if the elapsed time will be greater than the max elapsed time
            if (TargetElapsedTime > MaxElapsedTime)
                TargetElapsedTime = MaxElapsedTime;
        }

        /// <summary>
        /// Anything that should occur immediately before the main Draw method
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void PreDraw()
        {
            Time.UpdateFrames();
            
            RenderingManager.Instance.StartDraw();
            if (Debug.DebugEnabled == true && Debug.DebugDrawEnabled == true)
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
            
            GameStateManager.Instance.Render();
            Debug.DebugDraw();

            base.Draw(gameTime);

            PostDraw();
        }

        private void PostDraw()
        {
            RenderingManager.Instance.EndDraw();

            //Draw debug information on top of everything else
            if (Debug.DebugEnabled == true && Debug.DebugDrawEnabled == true)
            {
                Debug.DebugEndDraw();
            }
        }

        /// <summary>
        /// Notifies the engine to terminate the game at the end of the next update loop.
        /// </summary>
        public static void QuitGame()
        {
            ShouldQuit = true;
        }
    }
}
