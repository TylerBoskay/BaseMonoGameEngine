using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// Handles crashes through unhandled exceptions.
    /// <para>This currently works only for debug builds if a debugger is not attached.</para>
    /// </summary>
    public class CrashHandler : ICleanup
    {
        public CrashHandler()
        {
            //If a debugger is not present, handle the crash
            //If a debugger is present (Ex. IDE) then we can see the cause of the crash directly
            if (Debug.DebuggerAttached == false)
            {
                AppDomain.CurrentDomain.UnhandledException -= HandleCrash;
                AppDomain.CurrentDomain.UnhandledException += HandleCrash;
            }
        }

        public void CleanUp()
        {
            AppDomain.CurrentDomain.UnhandledException -= HandleCrash;
        }

        /// <summary>
        /// A crash handler for unhandled exceptions.
        /// </summary>
        /// <param name="sender">The source of the unhandled exception event.</param>
        /// <param name="e">An UnhandledExceptionEventArgs that contains the event data.</param>
        private void HandleCrash(object sender, UnhandledExceptionEventArgs e)
        {
            //Get the exception object
            string exceptionMessage = "N/A";
            string exceptionStackTrace = "Unavailable";

            Exception exc = e.ExceptionObject as Exception;
            if (exc != null)
            {
                exceptionMessage = exc.Message;
                exceptionStackTrace = exc.StackTrace;
            }

            string folderName = Debug.DebugGlobals.GetCrashFolderPath();
            DateTime crashTime = DateTime.Now;

            //If the crash directory doesn't exist, create it
            if (Directory.Exists(folderName) == false)
            {
                //If we fail to create the directory here, put the log in the current directory
                try
                {
                    Directory.CreateDirectory(folderName);
                }
                catch (Exception)
                {
                    folderName = Environment.CurrentDirectory;
                }
            }

            string fileName = Path.Combine(folderName, Debug.DebugGlobals.GetCrashLogFilename(crashTime));

            StringBuilder sb = new StringBuilder();

            //Dump the message, stack trace, and logs
            sb.Append($"Uh oh, looks like {Engine.GameName} crashed :(. Please report this crash by submitting this log file to the developer.\n\n");
            sb.Append($"OS Version: {Debug.DebugGlobals.GetOSInfo()}\n");

            sb.Append("Platform: ").Append(MonoGame.Framework.Utilities.PlatformInfo.MonoGamePlatform.ToString()).Append("\n");
            sb.Append("Renderer: ").Append(MonoGame.Framework.Utilities.PlatformInfo.GraphicsBackend.ToString()).Append("\n\n");
            
            sb.Append($"Message: {exceptionMessage}\n\nStack Trace:\n");
            sb.Append($"{exceptionStackTrace}\n\n");

            //Don't write the log dump unless there are logs
            if (Debug.LogDump.Length > 0)
            {
                sb.Append($"Log Dump:\n{Debug.LogDump.ToString()}");
            }

            string crashDump = sb.ToString();

            //Dump the message, stack trace, and logs to a file
            using (StreamWriter writer = File.CreateText(fileName))
            {
                writer.Write(crashDump);
                
                writer.Flush();
            }

            //On Windows, show an error message box
#if false//WINDOWS || DEBUG
            string title = $"{Engine.GameName} crashed!";
            string message = $"Uh oh, looks like {Engine.GameName} crashed :(. Please report this crash by submitting the file at the following path to the developer.\n\n\"{fileName}\"\n\nCrash Message: {exceptionMessage}";

            System.Windows.Forms.MessageBox.Show(message, title, System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Error);
#endif
        }
    }
}
