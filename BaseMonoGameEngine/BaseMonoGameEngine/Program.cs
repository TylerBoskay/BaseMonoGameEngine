using System;

namespace BaseMonoGameEngine
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
            using (Main game = new Main())
                game.Run();
        }
    }
}
