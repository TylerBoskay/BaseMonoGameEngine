using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TDMonoGameEngine
{
    /// <summary>
    /// Static class for debugging
    /// </summary>
    public static class Debug
    {
        private const string ScreenshotFolderName = "Screenshots";

        /// <summary>
        /// A delegate for custom debug commands.
        /// </summary>
        public delegate void DebugCommand();

        /// <summary>
        /// A delegate for custom debug drawing methods.
        /// </summary>
        public delegate void DebugDrawing();

        /// <summary>
        /// A list of custom debug commands that can be injected into the debug input.
        /// </summary>
        private static List<DebugCommand> CustomDebugCommands = new List<DebugCommand>();

        /// <summary>
        /// A list of custom debug drawing methods that can be injected into the debug draw.
        /// </summary>
        private static List<DebugDrawing> CustomDebugDrawMethods = new List<DebugDrawing>();

        /// <summary>
        /// The types of Debug logs.
        /// <para>This is a bit field.</para>
        /// </summary>
        [Flags]
        public enum DebugLogTypes
        {
            None = 0,
            Information = 1 << 0,
            Warning = 1 << 1,
            Error = 1 << 2,
            Assert = 1 << 3
        }

        public static bool DebugEnabled { get; private set; } = false;
        public static bool LogsEnabled { get; private set; } = false;

        public static bool DebugUpdateEnabled = false;
        public static bool DebugDrawEnabled = false;

        /// <summary>
        /// Whether to store logs made in the <see cref="LogDump"/>. This defaults to true.
        /// </summary>
        public static bool StoreLogs = true;

        public static bool DebugPaused { get; private set; } = false;
        public static bool AdvanceNextFrame { get; private set; } = false;

        /// <summary>
        /// The level of logs to output. By default, it logs all types of logs.
        /// </summary>
        public static DebugLogTypes LogLevels = DebugLogTypes.Information | DebugLogTypes.Warning | DebugLogTypes.Error | DebugLogTypes.Assert;

        public static StringBuilder LogDump { get; private set; } = new StringBuilder();

        public static bool DebuggerAttached => Debugger.IsAttached;

        public static KeyboardState DebugKeyboard = default(KeyboardState);

        public static SpriteBatch DebugSpriteBatch
        {
            get
            {
                if (debugSpriteBatch == null && RenderingManager.HasInstance == true)
                {
                    debugSpriteBatch = new SpriteBatch(RenderingManager.Instance.graphicsDevice);
                }
        
                return debugSpriteBatch;
            }
        }

        public static SpriteBatch DebugUIBatch
        {
            get
            {
                if (debugUIBatch == null && RenderingManager.HasInstance == true)
                {
                    debugUIBatch = new SpriteBatch(RenderingManager.Instance.graphicsDevice);
                }

                return debugUIBatch;
            }
        }

        /// <summary>
        /// A separate SpriteBatch for debugging.
        /// </summary>
        private static SpriteBatch debugSpriteBatch = null;
        private static SpriteBatch debugUIBatch = null;

        static Debug()
        {
            #if DEBUG
                DebugEnabled = true;
                LogsEnabled = true;
                DebugUpdateEnabled = true;
                DebugDrawEnabled = false;
            #else
                DebugEnabled = false;
                LogsEnabled = true;
                DebugUpdateEnabled = false;
                DebugDrawEnabled = false;
            #endif
        }

        public static void DebugCleanup()
        {
            if (debugSpriteBatch != null)
            {
                debugSpriteBatch.Dispose();
                debugSpriteBatch = null;
            }

            if (debugUIBatch != null)
            {
                debugUIBatch.Dispose();
                debugUIBatch = null;
            }

            RemoveAllCustomDebugCommands();
            RemoveAllCustomDebugDrawMethods();
        }

        public static void ToggleDebug(in bool debugEnabled)
        {
            DebugEnabled = debugEnabled;
        }

        public static void ToggleLogs(in bool logsEnabled)
        {
            LogsEnabled = logsEnabled;
        }

        private static string GetStackInfo(in int skipFrames)
        {
            StackFrame trace = new StackFrame(skipFrames, true);
            int line = 0;
            string method = string.Empty;
            string fileName = string.Empty;

            string traceFileName = trace.GetFileName();
            if (string.IsNullOrEmpty(traceFileName) == false)
            {
                int fileIndex = traceFileName.LastIndexOf('\\');

                //If it didn't find a backslash, try looking for a forward slash
                if (fileIndex < 0)
                {
                    fileIndex = traceFileName.LastIndexOf('/');
                }

                if (fileIndex >= 0)
                {
                    string file = traceFileName.Substring(fileIndex + 1, traceFileName.Length - (fileIndex + 1));
                    fileName = file;
                }
            }

            if (string.IsNullOrEmpty(fileName) == true)
                fileName = "N/A FileName";

            line = trace.GetFileLineNumber();
            method = trace.GetMethod()?.Name;
            if (string.IsNullOrEmpty(method) == true)
                method = "N/A MethodName";

            return $"{fileName}->{method}({line}):";
        }

        private static string GetStackInfo()
        {
            return GetStackInfo(3);
        }

        private static void DebugWriteLine(in string value)
        {
            //Write to the log dump if we should
            if (StoreLogs == true)
            {
                LogDump.Append(value);
                LogDump.Append("\n");
            }

            WriteLine(value);
        }

        public static void Log(in object value)
        {
            if (LogsEnabled == false || DebugLogTypesHasFlag(LogLevels, DebugLogTypes.Information) == false) return;
            DebugWriteLine($"Information: {GetStackInfo()} {value}");
        }

        public static void LogWarning(in object value)
        {
            if (LogsEnabled == false || DebugLogTypesHasFlag(LogLevels, DebugLogTypes.Warning) == false) return;
            DebugWriteLine($"Warning: {GetStackInfo()} {value}");
        }

        public static void LogError(in object value)
        {
            if (LogsEnabled == false || DebugLogTypesHasFlag(LogLevels, DebugLogTypes.Error) == false) return;
            DebugWriteLine($"Error: {GetStackInfo()} {value}");
        }

        private static void LogAssert()
        {
            if (LogsEnabled == false || DebugLogTypesHasFlag(LogLevels, DebugLogTypes.Assert) == false) return;
            string stackInfo = GetStackInfo(3);
            stackInfo = stackInfo.Remove(stackInfo.Length - 1);

            DebugWriteLine($"ASSERT FAILURE AT: {stackInfo}");
        }

        public static void Assert(in bool condition)
        {
            if (condition == false)
                LogAssert();
        }

        public static void DebugUpdate()
        {
            //Return if debug isn't enabled
            if (DebugEnabled == false)
            {
                return;
            }

            //Toggle debug
            if (KeyboardInput.GetKey(Keys.LeftControl, DebugKeyboard) && KeyboardInput.GetKeyDown(Keys.D, DebugKeyboard))
            {
                DebugUpdateEnabled = !DebugUpdateEnabled;
            }

            //Return if debug updating isn't enabled
            if (DebugUpdateEnabled == false)
            {
                //Update the input state if debug is disabled so that the toggle functions properly
                KeyboardInput.UpdateKeyboardState(ref DebugKeyboard);
                return;
            }

            //Reset frame advance
            AdvanceNextFrame = false;

            //Debug controls
            if (KeyboardInput.GetKey(Keys.LeftControl, DebugKeyboard))
            {
                //Toggle pause
                if (KeyboardInput.GetKeyDown(Keys.P, DebugKeyboard))
                {
                    DebugPaused = !DebugPaused;
                }
                //Toggle frame advance
                else if (KeyboardInput.GetKeyDown(Keys.OemSemicolon, DebugKeyboard))
                {
                    AdvanceNextFrame = true;
                }
                //Toggle logs
                else if (KeyboardInput.GetKeyDown(Keys.L, DebugKeyboard))
                {
                    ToggleLogs(!LogsEnabled);
                }
                //Take screenshot
                else if (KeyboardInput.GetKeyDown(Keys.S, DebugKeyboard))
                {
#if WINDOWS || DEBUG
                    TakeScreenshotWindows();
#elif !WINDOWS
                    TakeScreenshotNonWindows();
#endif
                }
                else if (KeyboardInput.GetKeyDown(Keys.M, DebugKeyboard))
                {
#if WINDOWS || DEBUG
                    //Log dump
                    DumpLogs();
#endif
                }
                //FPS increase
                else if (KeyboardInput.GetKeyDown(Keys.OemPlus, DebugKeyboard))
                {
                    Time.FPS += 5d;
                }
                //FPS decrease
                else if (KeyboardInput.GetKeyDown(Keys.OemMinus, DebugKeyboard))
                {
                    Time.FPS = UtilityGlobals.Clamp(Time.FPS - 5d, 5d, double.MaxValue);
                }
                //Reset FPS
                else if (KeyboardInput.GetKeyDown(Keys.Space, DebugKeyboard))
                {
                    Time.FPS = 60d;
                }
                //Toggle VSync
                else if (KeyboardInput.GetKeyDown(Keys.Back))
                {
                    Time.VSyncEnabled = !Time.VSyncEnabled;
                }
                //Toggle fixed time step
                else if (KeyboardInput.GetKeyDown(Keys.OemCloseBrackets))
                {
                    Time.FixedTimeStep = !Time.FixedTimeStep;
                }
                //Toggle Debug Draw
                else if (KeyboardInput.GetKeyDown(Keys.Q, DebugKeyboard))
                {
                    DebugDrawEnabled = !DebugDrawEnabled;
                }
            }

            //Camera controls
            if (KeyboardInput.GetKey(Keys.LeftShift, DebugKeyboard))
            {
                Camera2D camera = null;
                if (SceneManager.HasInstance == true && SceneManager.Instance.ActiveScene != null)
                {
                    camera = SceneManager.Instance.ActiveScene.Camera;
                }

                if (KeyboardInput.GetKeyDown(Keys.Space, DebugKeyboard))
                {
                    //Reset camera coordinates
                    camera?.SetTranslation(Vector2.Zero);
                    camera?.SetRotation(0f);
                    camera?.SetZoom(1f);
                }
                else
                {
                    Vector2 translation = Vector2.Zero;
                    float rotation = 0f;
                    float zoom = 0f;

                    //Translation
                    if (KeyboardInput.GetKey(Keys.Left, DebugKeyboard)) translation.X -= 2;
                    if (KeyboardInput.GetKey(Keys.Right, DebugKeyboard)) translation.X += 2;
                    if (KeyboardInput.GetKey(Keys.Down, DebugKeyboard)) translation.Y += 2;
                    if (KeyboardInput.GetKey(Keys.Up, DebugKeyboard)) translation.Y -= 2;

                    //Rotation
                    if (KeyboardInput.GetKey(Keys.OemComma, DebugKeyboard)) rotation -= .1f;
                    if (KeyboardInput.GetKey(Keys.OemPeriod, DebugKeyboard)) rotation += .1f;

                    //Scale
                    if (KeyboardInput.GetKey(Keys.OemMinus, DebugKeyboard)) zoom -= .1f;
                    if (KeyboardInput.GetKey(Keys.OemPlus, DebugKeyboard)) zoom += .1f;

                    if (translation != Vector2.Zero) camera?.Translate(translation);
                    if (rotation != 0f) camera?.Rotate(rotation);
                    if (zoom != 0f) camera?.Zoom(zoom);
                }
            }

            //Invoke injected debug commands
            for (int i = 0; i < CustomDebugCommands.Count; i++)
            {
                CustomDebugCommands[i]();
            }

            FPSCounter.Update();
            KeyboardInput.UpdateKeyboardState(ref DebugKeyboard);
        }

#if WINDOWS || DEBUG
        /// <summary>
        /// Takes a screenshot of the screen. This uses a SaveFileDialog and only works on Windows platforms.
        /// </summary>
        private static void TakeScreenshotWindows()
        {
            //Open the file dialogue so you can name the file and place it wherever you want
            System.Windows.Forms.SaveFileDialog dialogue = new System.Windows.Forms.SaveFileDialog();
            dialogue.FileName = string.Empty;
            dialogue.Filter = "PNG (*.png)|*.png";

            if (dialogue.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //Wrap the Texture2D in the using so it's guaranteed to get disposed
                using (Texture2D screenshotTex = GetScreenshot())
                {
                    using (FileStream fstream = new FileStream(dialogue.FileName, FileMode.Create))
                    {
                        int width = RenderingManager.Instance.graphicsDevice.PresentationParameters.BackBufferWidth;
                        int height = RenderingManager.Instance.graphicsDevice.PresentationParameters.BackBufferHeight;

                        screenshotTex.SaveAsPng(fstream, width, height);
                    }
                }
            }
        }

#else
        /// <summary>
        /// Takes a screenshot of the screen. This does not use a SaveFileDialog and works on non-Windows platforms.
        /// </summary>
        private static void TakeScreenshotNonWindows()
        {
            /* Non-Windows platforms can't use System.Windows.Forms on Mono, so we'll put all screenshots in a dedicated folder */
            string screenshotFolderPath = Path.Combine(Environment.CurrentDirectory, ScreenshotFolderName);

            //Create the directory if it doesn't exist
            if (Directory.Exists(screenshotFolderPath) == false)
            {
                Directory.CreateDirectory(screenshotFolderPath);
            }

            string fileName = "PuzzleGameScreenshot";
            string filePath = Path.Combine(screenshotFolderPath, fileName);

            int? index = null;

            string finalPath = GetNextScreenshotPath(filePath, index);

            //Keep searching for the next file name
            while (File.Exists(finalPath) == true)
            {
                if (index == null) index = 0;
                else index++;

                finalPath = GetNextScreenshotPath(filePath, index);
            }

            //Wrap the Texture2D in the using so it's guaranteed to get disposed
            using (Texture2D screenshotTex = GetScreenshot())
            {
                //Save the file
                using (FileStream fstream = new FileStream(finalPath, FileMode.Create))
                {
                    int width = RenderingManager.Instance.graphicsDevice.PresentationParameters.BackBufferWidth;
                    int height = RenderingManager.Instance.graphicsDevice.PresentationParameters.BackBufferHeight;

                    screenshotTex.SaveAsPng(fstream, width, height);
                }
            }
        }
#endif

        /// <summary>
        /// Gets the path to save the next screenshot for non-Windows platforms.
        /// </summary>
        /// <param name="curPath">The current path.</param>
        /// <param name="index">The number to append to the path. This is not null if a file exists for the current path.</param>
        /// <returns>A string representing the path to save the next screenshot.</returns>
        private static string GetNextScreenshotPath(in string curPath, in int? index)
        {
            if (index == null) return curPath + ".png";
            else return curPath + index + ".png";
        }

        /// <summary>
        /// Gets what is currently rendered on the backbuffer and returns it in a Texture2D.
        /// <para>IMPORTANT: Dispose the Texture2D when you're done with it.</para>
        /// </summary>
        /// <returns>A Texture2D of what's currently rendered on the screen.</returns>
        private static Texture2D GetScreenshot()
        {
            GraphicsDevice graphicsDevice = RenderingManager.Instance.graphicsDevice;

            int width = graphicsDevice.PresentationParameters.BackBufferWidth;
            int height = graphicsDevice.PresentationParameters.BackBufferHeight;

            //Present what's drawn
            graphicsDevice.Present();

            //Fill an array with the back buffer data that's the same size as the screen
            int[] backbuffer = new int[width * height];
            graphicsDevice.GetBackBufferData(backbuffer);

            //Create a new Texture2D and set the data
            Texture2D screenshot = new Texture2D(graphicsDevice, width, height, false, graphicsDevice.PresentationParameters.BackBufferFormat);
            screenshot.SetData(backbuffer);

            return screenshot;
        }

#if WINDOWS || DEBUG

        /// <summary>
        /// Dumps the current debug logs to a .txt file.
        /// </summary>
        public static void DumpLogs()
        {
            string initFileName = "Log Dump " + DebugGlobals.GetFileFriendlyTimeStamp();

            //Open the file dialogue so you can name the file and place it wherever you want
            System.Windows.Forms.SaveFileDialog dialogue = new System.Windows.Forms.SaveFileDialog();
            dialogue.FileName = initFileName;
            dialogue.Filter = "TXT (*.txt)|*.txt";

            if (dialogue.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //Dump the logs to the file
                using (StreamWriter writer = File.CreateText(dialogue.FileName))
                {
                    writer.Write($"Log Dump:\n{Debug.LogDump.ToString()}");

                    writer.Flush();
                }
            }
        }

#endif

        /// <summary>
        /// Tells whether a set of DebugLogTypes has any of the flags in another DebugLogTypes set.
        /// </summary>
        /// <param name="debugLogTypes">The DebugLogTypes value.</param>
        /// <param name="debugLogTypesFlags">The flags to test.</param>
        /// <returns>true if any of the flags in debugLogTypes are in debugLogTypesFlags, otherwise false.</returns>
        public static bool DebugLogTypesHasFlag(in Debug.DebugLogTypes debugLogTypes, in Debug.DebugLogTypes debugLogTypesFlags)
        {
            Debug.DebugLogTypes flags = (debugLogTypes & debugLogTypesFlags);

            return (flags != 0);
        }

    #region Custom Debug Command and Drawing Methods

        /// <summary>
        /// Injects a custom debug command at the end of the debug update loop.
        /// This method will be invoked even if the game is paused through debug, provided debug is enabled.
        /// </summary>
        /// <param name="debugCommand">The debug command to inject.</param>
        /// <param name="index">The index to insert the debug command at. If less than 0, it will add it to the end of the list.</param>
        public static void InjectCustomDebugCommand(in DebugCommand debugCommand, int index = -1)
        {
            if (index < 0)
            {
                CustomDebugCommands.Add(debugCommand);
            }
            else
            {
                CustomDebugCommands.Insert(index, debugCommand);
            }
        }

        /// <summary>
        /// Removes a custom debug command.
        /// </summary>
        /// <param name="debugCommand">The debug command to remove.</param>
        public static void RemoveCustomDebugCommand(in DebugCommand debugCommand)
        {
            CustomDebugCommands.Remove(debugCommand);
        }

        /// <summary>
        /// Removes a custom debug command at a particular index.
        /// </summary>
        /// <param name="index">The index to remove the custom debug command at.</param>
        public static void RemoveCustomDebugCommand(in int index)
        {
            CustomDebugCommands.RemoveAt(index);
        }

        /// <summary>
        /// Removes all custom debug commands.
        /// </summary>
        public static void RemoveAllCustomDebugCommands()
        {
            CustomDebugCommands.Clear();
        }

        /// <summary>
        /// Adds a custom debug drawing method at the end of the debug draw loop.
        /// This method should handle drawing with the debug SpriteBatches and is not invoked if debug is disabled.
        /// </summary>
        /// <param name="debugDrawMethod">The debug drawing method to add.</param>
        public static void AddCustomDebugDrawMethod(DebugDrawing debugDrawMethod)
        {
            CustomDebugDrawMethods.Add(debugDrawMethod);
        }

        /// <summary>
        /// Removes a custom debug drawing method.
        /// </summary>
        /// <param name="debugDrawMethod">The debug drawing method to remove.</param>
        public static void RemoveCustomDebugDrawMethod(DebugDrawing debugDrawMethod)
        {
            CustomDebugDrawMethods.Remove(debugDrawMethod);
        }

        /// <summary>
        /// Removes all custom debug drawing methods.
        /// </summary>
        public static void RemoveAllCustomDebugDrawMethods()
        {
            CustomDebugDrawMethods.Clear();
        }

    #endregion

        public static void DebugStartDraw()
        {
            DebugSpriteBatch?.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, SceneManager.Instance.ActiveScene.Camera?.TransformMatrix);
            DebugUIBatch?.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, null);
        }

        public static void DebugEndDraw()
        {
            DebugSpriteBatch?.End();
            DebugUIBatch?.End();
        }

        public static void DebugDraw()
        {
            if (DebugEnabled == false || DebugDrawEnabled == false) return;

            //FPS counter
            FPSCounter.Draw();

            SpriteFont font = AssetManager.Instance.LoadAsset<SpriteFont>($"{ContentGlobals.FontRoot}Font");

#if LINUX
            string rendererStr = "Renderer: OpenGL";
#elif WINDOWS
            string rendererStr = "Renderer: DirectX";
#endif

            debugUIBatch.DrawString(font, rendererStr, new Vector2(640, 0), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, .1f);

            debugUIBatch.DrawString(font, "Fixed Timestep: " + Time.FixedTimeStep, new Vector2(640, 0), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, .1f);
            debugUIBatch.DrawString(font, "VSync: " + Time.VSyncEnabled, new Vector2(703, 20), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, .1f);

            //Rendering info
            //ClearCount is the number of times Clear was called
            //PrimitiveCount is the number of number of rendered primitives
            //DrawCount is the number of times Draw was called
            //SpriteCount is the number of sprites and text characters rendered via SpriteBatch
            //TextureCount is the number of times a texture was changed on the GPU
            //TargetCount is the number of times a target was changed on the GPU
            //PixelShaderCount is the number of times the pixel shader was changed on the GPU
            //VertexShaderCount is the number of times the vertex shader was changed on the GPU
            Vector2 renderBasePos = new Vector2(0, 40);
            GraphicsMetrics metrics = RenderingManager.Instance.RenderingMetrics;
            debugUIBatch.DrawString(font, "Clear count: " + metrics.ClearCount, renderBasePos, Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, .1f);
            debugUIBatch.DrawString(font, "Primitive count: " + metrics.PrimitiveCount, renderBasePos + new Vector2(0, 20), Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, .1f);
            debugUIBatch.DrawString(font, "Draw calls: " + metrics.DrawCount, renderBasePos + new Vector2(0, 40), Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, .1f);
            debugUIBatch.DrawString(font, "Sprite count: " + metrics.SpriteCount, renderBasePos + new Vector2(0, 60), Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, .1f);
            debugUIBatch.DrawString(font, "Texture count: " + metrics.TextureCount, renderBasePos + new Vector2(0, 80), Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, .1f);
            debugUIBatch.DrawString(font, "Render Target count: " + metrics.TargetCount, renderBasePos + new Vector2(0, 100), Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, .1f);
            debugUIBatch.DrawString(font, "Pixel shaders: " + metrics.PixelShaderCount, renderBasePos + new Vector2(0, 120), Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, .1f);
            debugUIBatch.DrawString(font, "Vertex shaders: " + metrics.VertexShaderCount, renderBasePos + new Vector2(0, 140), Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, .1f);

            //Memory Info
            Vector2 memBasePos = new Vector2(0, 200);
            debugUIBatch.DrawString(font, "Managed Memory: " + Math.Round((GC.GetTotalMemory(false) / 1024f) / 1024f, 2) + " MB", memBasePos, Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, .1f);

            //Camera info
            if (SceneManager.HasInstance == true && SceneManager.Instance.ActiveScene != null && SceneManager.Instance.ActiveScene.Camera != null)
            {
                Vector2 cameraBasePos = new Vector2(0, 390);
                DebugUIBatch?.DrawString(font, "Camera:", cameraBasePos, Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, .1f);
                DebugUIBatch?.DrawString(font, $"Pos: {SceneManager.Instance.ActiveScene.Camera.Position}", cameraBasePos + new Vector2(0, 20), Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, .1f);
                DebugUIBatch?.DrawString(font, $"Rot: {SceneManager.Instance.ActiveScene.Camera.Rotation}", cameraBasePos + new Vector2(0, 40), Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, .1f);
                DebugUIBatch?.DrawString(font, $"Zoom: {SceneManager.Instance.ActiveScene.Camera.Scale}", cameraBasePos + new Vector2(0, 60), Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, .1f);
            }

            //Invoke custom drawing methods
            for (int i = 0; i < CustomDebugDrawMethods.Count; i++)
            {
                CustomDebugDrawMethods[i]();
            }
        }

#region Classes

        /// <summary>
        /// Global values regarding debugging.
        /// </summary>
        public static class DebugGlobals
        {
            /// <summary>
            /// Gets the path for crash log files.
            /// </summary>
            /// <returns>A string with the full name of the crash log file.</returns>
            public static string GetCrashLogPath()
            {
                string time = GetFileFriendlyTimeStamp();

                string path = Path.Combine(System.Environment.CurrentDirectory, $"{Engine.GameName} Crash Log - {time}.txt");

                return path;
            }

            /// <summary>
            /// Returns a file friendly time stamp of the current local time.
            /// </summary>
            /// <returns>A string representing current local time.</returns>
            public static string GetFileFriendlyTimeStamp()
            {
                string time = DateTime.Now.ToString();
                time = time.Replace(':', '-');
                time = time.Replace('/', '-');

                return time;
            }

            /// <summary>
            /// Gets the name of the assembly.
            /// </summary>
            /// <returns>A string representing the name of the assembly.</returns>
            public static string GetAssemblyName()
            {
                //Get the name from the assembly information
                System.Reflection.Assembly assembly = typeof(Debug).Assembly;
                System.Reflection.AssemblyName asm = assembly.GetName();

                return asm.Name;
            }

            /// <summary>
            /// Gets the full build number as a string.
            /// </summary>
            /// <returns>A string representing the full build number.</returns>
            public static string GetBuildNumber()
            {
                //Get the build number from the assembly information
                System.Reflection.Assembly assembly = typeof(Debug).Assembly;
                System.Reflection.AssemblyName asm = assembly.GetName();

                return asm.Version.Major + "." + asm.Version.Minor + "." + asm.Version.Build + "." + asm.Version.Revision;
            }

            /// <summary>
            /// Gets the name, version, and word size of the operating system as a string.
            /// </summary>
            /// <returns>A string representing the OS name, version, and word size.</returns>
            public static string GetOSInfo()
            {
                string osVersion = string.Empty;

                //Getting the OS version can fail if the user is running an extremely uncommon or old OS and/or it can't retrieve the information
                try
                {
                    osVersion = Environment.OSVersion.ToString();

                    //Check for a Linux OS and get more detailed info
                    if (osVersion.ToLower().StartsWith("unix") == true)
                    {
                        string detailedLinux = GetDetailedLinuxOSInfo();

                        //If we got the info, set the OS string to the detailed version
                        if (string.IsNullOrEmpty(detailedLinux) == false)
                        {
                            osVersion = detailedLinux;
                        }
                    }
                }
                catch (InvalidOperationException)
                {
                    if (string.IsNullOrEmpty(osVersion) == true)
                        osVersion = "N/A";
                }

                //Get word size
                string osBit = Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit";

                return $"{osVersion} {osBit}";
            }

            /// <summary>
            /// Retrieves more detailed information about a Linux OS by reading from its lsb-release file.
            /// </summary>
            /// <returns>A string representing the Linux ID and Release number. If the file isn't found or accessible, then null.</returns>
            private static string GetDetailedLinuxOSInfo()
            {
                //Try to find the OS info in the "/etc/lsb-release" file
                //"/" is the root
                const string lsbRelease = "/etc/lsb-release";

                //Check if the file exists and we have permission to access it
                if (File.Exists(lsbRelease) == true)
                {
                    try
                    {
                        //Get all the text in the file
                        string releaseText = File.ReadAllText(lsbRelease);

                        //The DISTRIB_DESCRIPTION is the only thing we need, as it includes both the ID and Release number
                        const string findString = "DISTRIB_DESCRIPTION=\"";

                        //Find the location of the description in the file
                        int index = releaseText.IndexOf(findString) + findString.Length;

                        //The OS description will be here
                        //The description is in quotation marks, so exclude the last character
                        string osDescription = releaseText.Substring(index, releaseText.Length - index - 2);
                        return osDescription;
                    }
                    catch (Exception)
                    {
                        //If we ran into an error, there's nothing we can really do, so exit
                    }
                }

                return null;
            }
        }

#endregion
    }
}
