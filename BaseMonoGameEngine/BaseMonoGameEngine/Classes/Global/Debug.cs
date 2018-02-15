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

namespace BaseMonoGameEngine
{
    /// <summary>
    /// Static class for debugging
    /// </summary>
    public static class Debug
    {
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

        public static bool DebugPaused { get; private set; } = false;
        public static bool AdvanceNextFrame { get; private set; } = false;

        /// <summary>
        /// The level of logs to output. By default, it logs all types of logs.
        /// </summary>
        public static DebugLogTypes LogLevels = DebugLogTypes.Information | DebugLogTypes.Warning | DebugLogTypes.Error | DebugLogTypes.Assert;

        public static StringBuilder LogDump { get; private set; } = new StringBuilder();

        public static bool DebuggerAttached => Debugger.IsAttached;

        private static KeyboardState DebugKeyboard = default(KeyboardState);

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
            #else
                DebugEnabled = false;
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
        }

        private static void ToggleDebug()
        {
            #if DEBUG
                DebugEnabled = !DebugEnabled;
            #else
                //Failsafe
                DebugEnabled = false;
            #endif
        }

        private static void ToggleLogs()
        {
            LogsEnabled = !LogsEnabled;
        }

        private static string GetStackInfo(int skipFrames)
        {
            StackFrame trace = new StackFrame(skipFrames, true);
            int line = 0;
            string method = "";

            string traceFileName = trace.GetFileName();
            if (string.IsNullOrEmpty(traceFileName) == true)
                traceFileName = "N/A";

            string[] file = traceFileName.Split('\\');
            string fileName = file?[file.Length - 1];
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

        private static void DebugWriteLine(string value)
        {
            //Write to the log dump
            LogDump.Append(value);
            LogDump.Append("\n");

            WriteLine(value);
        }

        public static void Log(object value)
        {
            if (LogsEnabled == false || DebugLogTypesHasFlag(LogLevels, DebugLogTypes.Information) == false) return;
            DebugWriteLine($"Information: {GetStackInfo()} {value}");
        }

        public static void LogWarning(object value)
        {
            if (LogsEnabled == false || DebugLogTypesHasFlag(LogLevels, DebugLogTypes.Warning) == false) return;
            DebugWriteLine($"Warning: {GetStackInfo()} {value}");
        }

        public static void LogError(object value)
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

        public static void Assert(bool condition)
        {
            if (condition == false)
                LogAssert();
        }

        public static void DebugUpdate()
        {
            #if DEBUG
                //Toggle debug
                if (KeyboardInput.GetKey(Keys.LeftControl, DebugKeyboard) && KeyboardInput.GetKeyDown(Keys.D, DebugKeyboard))
                {
                    ToggleDebug();
                }
            #endif

            //Return if debug isn't enabled
            if (DebugEnabled == false)
            {
                if (Time.InGameTimeEnabled == false)
                    Time.ToggleInGameTime(true);

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
                    ToggleLogs();
                }
                //Take screenshot
                else if (KeyboardInput.GetKeyDown(Keys.S, DebugKeyboard))
                {
                    TakeScreenshot();
                }
                else if (KeyboardInput.GetKeyDown(Keys.M, DebugKeyboard))
                {
                    //Log dump
                    DumpLogs();
                }
            }

            //Camera controls
            if (KeyboardInput.GetKey(Keys.LeftShift, DebugKeyboard))
            {
                if (KeyboardInput.GetKeyDown(Keys.Space, DebugKeyboard))
                {
                    //Reset camera coordinates
                    Camera2D.Instance.SetTranslation(Vector2.Zero);
                    Camera2D.Instance.SetRotation(0f);
                    Camera2D.Instance.SetZoom(1f);
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

                    if (translation != Vector2.Zero) Camera2D.Instance.Translate(translation);
                    if (rotation != 0f) Camera2D.Instance.Rotate(rotation);
                    if (zoom != 0f) Camera2D.Instance.Zoom(zoom);
                }
            }

            //If a pause is eventually added that can be performed normally, put a check for it in here to
            //prevent the in-game timer from turning on when it shouldn't
            Time.ToggleInGameTime(DebugPaused == false || AdvanceNextFrame == true);

            FPSCounter.Update();
            KeyboardInput.UpdateKeyboardState(ref DebugKeyboard);
        }

        /// <summary>
        /// Takes a screenshot of the screen.
        /// </summary>
        public static void TakeScreenshot()
        {
            //Wrap the Texture2D in the using so it's guaranteed to get disposed
            using (Texture2D screenshotTex = GetScreenshot())
            {
                //Open the file dialogue so you can name the file and place it wherever you want
                System.Windows.Forms.SaveFileDialog dialogue = new System.Windows.Forms.SaveFileDialog();
                dialogue.FileName = string.Empty;
                dialogue.Filter = "PNG (*.png)|*.png";

                if (dialogue.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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

        /// <summary>
        /// Tells whether a set of DebugLogTypes has any of the flags in another DebugLogTypes set.
        /// </summary>
        /// <param name="debugLogTypes">The DebugLogTypes value.</param>
        /// <param name="debugLogTypesFlags">The flags to test.</param>
        /// <returns>true if any of the flags in debugLogTypes are in debugLogTypesFlags, otherwise false.</returns>
        public static bool DebugLogTypesHasFlag(Debug.DebugLogTypes debugLogTypes, Debug.DebugLogTypes debugLogTypesFlags)
        {
            Debug.DebugLogTypes flags = (debugLogTypes & debugLogTypesFlags);

            return (flags != 0);
        }

        #region Debug Drawing Methods

        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <param name="color">The color of the line.</param>
        /// <param name="layer">The layer of the line.</param>
        /// <param name="thickness">The thickness of the line.</param>
        /// <param name="uiBatch">Whether to draw with the UI batch or not.</param>
        public static void DebugDrawLine(Vector2 start, Vector2 end, Color color, float layer, int thickness, bool uiBatch)
        {
            if (DebugEnabled == false) return;

            Texture2D box = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}Box.png");

            //Get rotation with the angle between the start and end vectors
            float lineRotation = (float)UtilityGlobals.TangentAngle(start, end);

            //Get the scale; use the X as the length and the Y as the width
            Vector2 diff = end - start;
            Vector2 lineScale = new Vector2(diff.Length(), thickness);

            SpriteBatch batch = (uiBatch == false) ? DebugSpriteBatch : DebugUIBatch;
            batch?.Draw(box, start, null, color, lineRotation, Vector2.Zero, lineScale, SpriteEffects.None, layer);
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="rect">The Rectangle to draw.</param>
        /// <param name="color">The color of the rectangle.</param>
        /// <param name="layer">The layer of the rectangle.</param>
        /// <param name="uiBatch">Whether to draw with the UI batch or not.</param>
        public static void DebugDrawRect(Rectangle rect, Color color, float layer, bool uiBatch)
        {
            if (DebugEnabled == false) return;

            Texture2D box = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}Box.png");

            SpriteBatch batch = (uiBatch == false) ? DebugSpriteBatch : DebugUIBatch;
            batch?.Draw(box, rect, null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
        }

        /// <summary>
        /// Draws a hollow rectangle.
        /// </summary>
        /// <param name="rect">The Rectangle to draw.</param>
        /// <param name="color">The color of the hollow rectangle.</param>
        /// <param name="layer">The layer of the hollow rectangle.</param>
        /// <param name="thickness">The thickness of the hollow rectangle.</param>
        /// <param name="uiBatch">Whether to draw with the UI batch or not.</param>
        public static void DebugDrawHollowRect(Rectangle rect, Color color, float layer, int thickness, bool uiBatch)
        {
            if (DebugEnabled == false) return;

            Rectangle[] rects = new Rectangle[4]
            {
                new Rectangle(rect.X, rect.Y, rect.Width, thickness),
                new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height),
                new Rectangle(rect.X, rect.Y, thickness, rect.Height),
                new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness)
            };

            Texture2D box = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}Box.png");

            for (int i = 0; i < rects.Length; i++)
            {
                SpriteBatch batch = (uiBatch == false) ? DebugSpriteBatch : DebugUIBatch;
                batch?.Draw(box, rects[i], null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
            }
        }

        /// <summary>
        /// Draws a circle.
        /// </summary>
        /// <param name="circle">The circle to draw.</param>
        /// <param name="color">The color of the circle.</param>
        /// <param name="layer">The layer of the circle.</param>
        /// <param name="uiBatch">Whether to draw with the UI batch or not.</param>
        /// <remarks>Brute force algorithm obtained from here: https://stackoverflow.com/a/1237519 
        /// This seems to gives a more full looking circle than Bresenham's algorithm.
        /// </remarks>
        public static void DebugDrawCircle(Circle circle, Color color, float layer, bool uiBatch)
        {
            Texture2D box = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}Box.png");
            float radius = (float)circle.Radius;
            Vector2 origin = circle.Center;
            float radiusSquared = radius * radius;
            float radiusSquaredPlusRadius = radiusSquared + radius;

            for (float y = -radius; y <= radius; y++)
            {
                for (float x = -radius; x <= radius; x++)
                {
                    float xSquared = x * x;
                    float ySquared = y * y;

                    if ((xSquared + ySquared) < radiusSquaredPlusRadius)
                    {
                        SpriteBatch batch = (uiBatch == false) ? DebugSpriteBatch : DebugUIBatch;
                        batch?.Draw(box, new Vector2(origin.X + x, origin.Y + y), null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, layer);
                    }
                }
            }
        }

        /// <summary>
        /// Draws a hollow circle.
        /// </summary>
        /// <param name="circle">The hollow circle to draw.</param>
        /// <param name="color">The color of the hollow circle.</param>
        /// <param name="layer">The layer of the hollow circle.</param>
        /// <param name="uiBatch">Whether to draw with the UI batch or not.</param>
        /// <remarks>Brute force algorithm obtained from here: https://stackoverflow.com/a/1237519 
        /// This seems to gives a more full looking circle than Bresenham's algorithm.
        /// </remarks>
        public static void DebugDrawHollowCircle(Circle circle, Color color, float layer, bool uiBatch)
        {
            Texture2D box = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}Box.png");
            float radius = (float)circle.Radius;
            Vector2 origin = circle.Center;
            float radiusSquared = radius * radius;
            float radiusSqMinusRadius = radiusSquared - radius;
            float radiusSqPlusRadius = radiusSquared + radius;

            for (float y = -radius; y <= radius; y++)
            {
                for (float x = -radius; x <= radius; x++)
                {
                    float xSquared = x * x;
                    float ySquared = y * y;

                    if ((xSquared + ySquared) > radiusSqMinusRadius && (xSquared + ySquared) < radiusSqPlusRadius)
                    {
                        SpriteBatch batch = (uiBatch == false) ? DebugSpriteBatch : DebugUIBatch;
                        batch?.Draw(box, new Vector2(origin.X + x, origin.Y + y), null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, layer);
                    }
                }
            }
        }

        #endregion

        public static void DebugStartDraw()
        {
            DebugSpriteBatch?.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, Camera2D.Instance.TransformMatrix);
            DebugUIBatch?.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, null);
        }

        public static void DebugEndDraw()
        {
            DebugSpriteBatch?.End();
            DebugUIBatch?.End();
        }

        public static void DebugDraw()
        {
            if (DebugEnabled == false) return;

            //FPS counter
            FPSCounter.Draw();

            SpriteFont font = AssetManager.Instance.LoadAsset<SpriteFont>($"{ContentGlobals.FontRoot}Font");

            //Camera info
            Vector2 cameraBasePos = new Vector2(0, 390);
            DebugUIBatch?.DrawString(font, "Camera:", cameraBasePos, Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, .1f);
            DebugUIBatch?.DrawString(font, $"Pos: {Camera2D.Instance.Position}", cameraBasePos + new Vector2(0, 20), Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, .1f);
            DebugUIBatch?.DrawString(font, $"Rot: {Camera2D.Instance.Rotation}", cameraBasePos + new Vector2(0, 40), Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, .1f);
            DebugUIBatch?.DrawString(font, $"Zoom: {Camera2D.Instance.Scale}", cameraBasePos + new Vector2(0, 60), Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, .1f);
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

                string path = $"{System.IO.Directory.GetCurrentDirectory()}\\{GetAssemblyName()} {GetBuildNumber()} Crash Log - {time}.txt";

                return path;
            }

            /// <summary>
            /// Returns a file friendly time stamp of the current time.
            /// </summary>
            /// <returns>A string representing current time.</returns>
            public static string GetFileFriendlyTimeStamp()
            {
                string time = DateTime.Now.ToUniversalTime().ToString();
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
        }

        #endregion
    }
}
