using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// The scene.
    /// </summary>
    public class GameScene : IUpdateable, ICleanup
    {
        /// <summary>
        /// The objects in the scene.
        /// </summary>
        private readonly List<SceneObject> SceneObjects = new List<SceneObject>();

        /// <summary>
        /// The number of SceneObjects in the scene.
        /// </summary>
        public int SceneObjectCount => SceneObjects.Count;

        public GameScene()
        {

        }

        public void CleanUp()
        {
            RemoveAllSceneObjects();
        }

        /// <summary>
        /// Updates the game scene.
        /// </summary>
        public void Update()
        {
            for (int i = 0; i < SceneObjects.Count; i++)
            {
                SceneObject sceneObj = SceneObjects[i];
                if (sceneObj.Enabled == false) continue;

                sceneObj.Update();
            }
        }

        public void AddSceneObject(SceneObject sceneObject)
        {
            SceneObjects.Add(sceneObject);
        }

        public void RemoveSceneObject(SceneObject sceneObject)
        {
            //Clean up before removing
            if (sceneObject != null)
            {
                sceneObject.CleanUp();
            }

            SceneObjects.Remove(sceneObject);
        }

        /// <summary>
        /// Removes all SceneObjects and cleans them up.
        /// </summary>
        private void RemoveAllSceneObjects()
        {
            for (int i = 0; i < SceneObjects.Count; i++)
            {
                SceneObject sceneObj = SceneObjects[i];
                sceneObj.CleanUp();

                SceneObjects.RemoveAt(i);
                i--;
            }
        }

        /// <summary>
        /// Returns all SceneObjects in the game scene in a new list.
        /// </summary>
        /// <returns>A new List of SceneObjects.</returns>
        public List<SceneObject> GetAllSceneObjects()
        {
            return new List<SceneObject>(SceneObjects);
        }

        /// <summary>
        /// Gets all the active visible renderers in the game scene.
        /// Renderers on SceneObjects that are disabled, as well as disabled and null renderers, are not included in this list.
        /// Renderers not visible at all by the camera are excluded as well.
        /// </summary>
        /// <returns>A new List of Renderers.</returns>
        public List<Renderer> GetActiveVisibleRenderersInScene()
        {
            List<Renderer> renderers = new List<Renderer>();

            for (int i = 0; i < SceneObjects.Count; i++)
            {
                SceneObject sceneObj = SceneObjects[i];
                if (sceneObj.Enabled == false) continue;

                if (sceneObj.renderer != null && sceneObj.renderer.Enabled == true)
                {
                    if (Camera2D.Instance.IsInCameraView(sceneObj.renderer.Bounds) == true)
                    {
                        renderers.Add(sceneObj.renderer);
                    }
                }
            }

            return renderers;
        }
    }
}
