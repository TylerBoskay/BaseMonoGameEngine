using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// The scene.
    /// </summary>
    public class GameScene : IUpdateable, ICleanup
    {
        /// <summary>
        /// The camera for the scene.
        /// </summary>
        public Camera2D Camera { get; private set; } = null;

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

        /// <summary>
        /// Sets the Camera2D for the GameScene.
        /// </summary>
        /// <param name="camera">The new Camera for the scene.</param>
        public void SetCamera(in Camera2D camera)
        {
            Camera = camera;
        }

        public void CleanUp()
        {
            RemoveAllSceneObjects();

            Camera?.CleanUp();
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

        public void AddSceneObject(in SceneObject sceneObject)
        {
            sceneObject.OnAddedToScene(this);
            SceneObjects.Add(sceneObject);
        }

        public void RemoveSceneObject(in SceneObject sceneObject)
        {
            //Clean up before removing
            if (sceneObject != null)
            {
                sceneObject.CleanUp();
            }

            sceneObject.OnRemovedFromScene(this);
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
    }
}
