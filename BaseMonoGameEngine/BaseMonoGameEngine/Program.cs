using System;

namespace TDMonoGameEngine
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

            using (Engine game = new Engine())
                game.Run();

            crashHandler.CleanUp();
            crashHandler = null;
        }
    }
}
