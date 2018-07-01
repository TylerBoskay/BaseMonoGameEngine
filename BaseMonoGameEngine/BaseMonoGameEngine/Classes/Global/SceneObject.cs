using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDMonoGameEngine
{
    /// <summary>
    /// An object in a game scene.
    /// </summary>
    public abstract class SceneObject : INameable, ITransformable, IUpdateable, IRenderable, IEnableable, ICleanup
    {
        /// <summary>
        /// The GameScene the SceneObject belongs to.
        /// </summary>
        public GameScene Scene { get; private set; } = null;

        /// <summary>
        /// The name of the SceneObject.
        /// </summary>
        public string Name { get; set; } = "SceneObject";

        /// <summary>
        /// The Transform of the SceneObject.
        /// </summary>
        public Transform transform { get; set; } = new Transform();

        /// <summary>
        /// The Renderer of the SceneObject.
        /// </summary>
        public Renderer renderer { get; set; } = null;

        /// <summary>
        /// Whether the SceneObject is enabled or not.
        /// Enabled SceneObjects are updated and rendered, while disabled ones are not.
        /// </summary>
        public bool Enabled { get; set; } = true;

        protected SceneObject()
        {

        }

        public virtual void CleanUp()
        {

        }

        public virtual void Update()
        {

        }

        public void OnAddedToScene(in GameScene scene)
        {
            Scene = scene;
        }

        public void OnRemovedFromScene(in GameScene scene)
        {
            Scene = null;
        }
    }
}
