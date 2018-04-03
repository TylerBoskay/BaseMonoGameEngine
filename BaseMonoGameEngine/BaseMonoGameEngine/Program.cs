using System;

namespace TDMonoGameEngine
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        internal static void Main()
        {
            using (Engine game = new Engine())
                game.Run();
        }
    }
}
