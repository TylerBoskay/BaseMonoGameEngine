using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// Class for global values dealing with game configuration.
    /// </summary>
    public static class ConfigGlobals
    {
        public const string ApplicationDataFolderRoot = "BaseMGEngine";
        public const string ApplicationDataFolderName = Engine.GameName;
        private static string ApplicationDataPath = string.Empty;

        public const string ConfigRoot = "Config";

        private static string InstallDataFolderPath = string.Empty;

        /// <summary>
        /// Gets the install path, or the path the game executable resides.
        /// </summary>
        /// <returns>A string representing the install path.</returns>
        public static string GetInstallDataPath()
        {
            if (string.IsNullOrEmpty(InstallDataFolderPath) == true)
            {
                InstallDataFolderPath = AppDomain.CurrentDomain.BaseDirectory;

                //For macOS, look in the Resources folder for a .app bundle
#if MACOS && !WINDOWS && !LINUX
                try
                {
                    string resourcesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Resources");

                    if (Directory.Exists(resourcesPath) == true)
                    {
                        InstallDataFolderPath = resourcesPath;
                    }
                }
                catch (Exception e) when (e is ArgumentException || e is PlatformNotSupportedException || e is ArgumentNullException)
                {
                    Debug.LogError($"Error combining paths for install data folder; resorting to executable's local directory. Message: {e.Message}");
                    InstallDataFolderPath = AppDomain.CurrentDomain.BaseDirectory;
                }
#endif
            }

            return InstallDataFolderPath;
        }

        /// <summary>
        /// Gets the path where the game stores its data.
        /// </summary>
        /// <para>This is the typical local application data path for the OS.</para>
        /// <returns>A string representing the path the game stores its data.</returns>
        public static string GetApplicationDataPath()
        {
            if (string.IsNullOrEmpty(ApplicationDataPath))
            {
                try
                {
                    //macOS typically has local application data in "~/Library/Application Support", while Mono would place it in "~/.local/share"
#if MACOS && !WINDOWS && !LINUX
                    string applicationDataPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library", "Application Support");
#else
                    string applicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
#endif
                    ApplicationDataPath = System.IO.Path.Combine(applicationDataPath, ApplicationDataFolderRoot, ApplicationDataFolderName);
                }
                catch (Exception e) when (e is ArgumentException || e is PlatformNotSupportedException || e is ArgumentNullException)
                {
                    Debug.LogError($"Error retrieving application data folder path; resorting to local game folder as path. Message: {e.Message}");
                    ApplicationDataPath = string.Empty;
                }
            }

            return ApplicationDataPath;
        }
    }
}