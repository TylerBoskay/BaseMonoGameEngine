using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// Manages scenes.
    /// <para>This is a Singleton.</para>
    /// </summary>
    public sealed class SceneManager : ICleanup
    {
        #region Singleton Fields

        public static SceneManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SceneManager();
                }

                return instance;
            }
        }

        public static bool HasInstance => (instance != null);

        private static SceneManager instance = null;

        #endregion

        /// <summary>
        /// The active scene.
        /// </summary>
        public GameScene ActiveScene { get; private set; } = null;

        private SceneManager()
        {

        }

        public void CleanUp()
        {
            ActiveScene?.CleanUp();
            ActiveScene = null;

            instance = null;
        }

        public void LoadScene(in GameScene scene)
        {
            ActiveScene?.CleanUp();
            ActiveScene = null;

            ActiveScene = scene;
        }
    }
}
