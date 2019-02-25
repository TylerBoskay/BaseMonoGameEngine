using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDMonoGameEngine
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
            Exception exc = e.ExceptionObject as Exception;
            if (exc != null)
            {
                string fileName = Debug.DebugGlobals.GetCrashLogPath();

                StringBuilder sb = new StringBuilder();

                //Dump the message, stack trace, and logs
                sb.Append($"Uh oh, looks like {Engine.GameName} crashed :(. Please report this crash by submitting this log file to emailhere@email.com.\n\n");
                sb.Append($"OS Version: {Debug.DebugGlobals.GetOSInfo()}\n");
#if LINUX
                sb.Append("Renderer: OpenGL\n\n");
#elif WINDOWS
                sb.Append("Renderer: DirectX\n\n");
#endif
                sb.Append($"Message: {exc.Message}\n\nStack Trace:\n");
                sb.Append($"{exc.StackTrace}\n\n");

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
#if WINDOWS || DEBUG
                string thing = $"Uh oh, looks like {Engine.GameName} crashed :(. Please report this crash by submitting the file found at the following path to emailhere@email.com.\n\n\"{fileName}\"\n\nCrash Message: { exc.Message}";

                System.Windows.Forms.MessageBox.Show(thing, $"{Engine.GameName} crashed!", System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
#endif
            }
        }
    }
}
