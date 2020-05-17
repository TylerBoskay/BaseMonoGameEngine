using System;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        private static CrashHandler crashHandler = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        internal static void Main()
        {
            crashHandler = new CrashHandler();

            //Set current working directory
            try
            {
                Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                //Debug.Log($"Set current working directory to: {Environment.CurrentDirectory}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Attempted to set current directory to AppDomain base directory, but something bad happened: {e.Message}");
            }

            Debug.Log("Initializing engine");

            using (Engine game = new Engine())
                game.Run();

            crashHandler.CleanUp();
            crashHandler = null;
        }
    }
}
