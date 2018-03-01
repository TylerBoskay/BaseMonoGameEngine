using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TDMonoGameEngine
{
    /// <summary>
    /// The scene.
    /// </summary>
    public class GameScene : IUpdateable, ICleanup
    {
        /// <summary>
        /// The list of Render Layers in the scene
        /// </summary>
        private readonly List<RenderLayer> RenderLayers = new List<RenderLayer>();

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
            //Add a default render layer
            RenderLayers.Add(new RenderLayer(0, new RenderLayer.RenderingSettings(RenderingManager.Instance.spriteBatch,
                SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, true)));
        }

        public void CleanUp()
        {
            RemoveAllSceneObjects();

            for (int i = 0; i < RenderLayers.Count; i++)
            {
                RenderLayers[i].CleanUp();
            }

            RenderLayers.Clear();
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
        /// Adds a RenderLayer to the scene and sorts all RenderLayers in the scene by their LayerOrder.
        /// </summary>
        /// <param name="renderLayer">The RenderLayer to add.</param>
        public void AddRenderLayer(RenderLayer renderLayer)
        {
            RenderLayers.Add(renderLayer);
            RenderLayers.Sort(RenderLayerSorter);
        }

        /// <summary>
        /// Gets all RenderLayers in the scene in a new list.
        /// </summary>
        /// <returns>A new list of RenderLayers.</returns>
        public List<RenderLayer> GetRenderLayersInScene()
        {
            return new List<RenderLayer>(RenderLayers);
        }

        /// <summary>
        /// Adds a layer effect for a particular RenderLayer.
        /// </summary>
        /// <param name="layerOrder">The layer order to apply the effect to.</param>
        /// <param name="layerEffect">The effect.</param>
        public void AddRenderLayerEffect(int layerOrder, Effect layerEffect)
        {
            if (layerEffect == null) return;
        
            RenderLayer layer = null;

            //Find the layer
            for (int i = 0; i < RenderLayers.Count; i++)
            {
                if (RenderLayers[i].LayerOrder == layerOrder)
                {
                    layer = RenderLayers[i];
                    break;
                }
            }
        
            if (layer != null)
                layer.AddLayerEffect(layerEffect);
        }

        /// <summary>
        /// Removes a layer effect from a particular RenderLayer.
        /// </summary>
        /// <param name="layerOrder">The layer order to remove the effect from.</param>
        /// <param name="layerEffect">The effect.</param>
        public void RemoveRenderLayerEffect(int layerOrder, Effect layerEffect)
        {
            if (layerEffect == null) return;
        
            RenderLayer layer = null;
        
            //Find the layer
            for (int i = 0; i < RenderLayers.Count; i++)
            {
                if (RenderLayers[i].LayerOrder == layerOrder)
                {
                    layer = RenderLayers[i];
                    break;
                }
            }
        
            if (layer != null)
                layer.RemoveLayerEffect(layerEffect);
        }

        /// <summary>
        /// Removes all layer effects from a particular RenderLayer.
        /// </summary>
        /// <param name="layerOrder">The layer order to remove all effects from.</param>
        public void RemoveAllRenderLayerEffects(int layerOrder)
        {
            RenderLayer layer = null;

            //Find the layer
            for (int i = 0; i < RenderLayers.Count; i++)
            {
                if (RenderLayers[i].LayerOrder == layerOrder)
                {
                    layer = RenderLayers[i];
                    break;
                }
            }

            if (layer != null)
                layer.RemoveAllLayerEffects();
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

            //Cache visible area
            Rectangle visibleArea = Camera2D.Instance.VisibleArea;

            for (int i = 0; i < SceneObjects.Count; i++)
            {
                SceneObject sceneObj = SceneObjects[i];
                if (sceneObj.Enabled == false) continue;

                if (sceneObj.renderer != null && sceneObj.renderer.Enabled == true)
                {
                    if (Camera2D.Instance.IsInCameraView(visibleArea, sceneObj.renderer.Bounds) == true)
                    {
                        renderers.Add(sceneObj.renderer);
                    }
                }
            }

            return renderers;
        }

        private int RenderLayerSorter(RenderLayer layer1, RenderLayer layer2)
        {
            if (layer1 == null)
                return 1;
            if (layer2 == null)
                return -1;

            if (layer1.LayerOrder < layer2.LayerOrder)
                return -1;
            if (layer1.LayerOrder > layer2.LayerOrder)
                return 1;

            return 0;
        }
    }
}
