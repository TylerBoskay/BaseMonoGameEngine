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
    /// The base class for Renderers.
    /// </summary>
    public abstract class Renderer : IEnableable
    {
        /// <summary>
        /// The layer order the renderer is in.
        /// </summary>
        public int Order { get; set; } = 0;

        /// <summary>
        /// The shader to apply to this renderer.
        /// </summary>
        public Effect Shader { get; set; } = null;

        /// <summary>
        /// The bounds of the renderer. This is used in Frustum Culling.
        /// </summary>
        public abstract Rectangle Bounds { get; }

        /// <summary>
        /// Whether the renderer is enabled or not.
        /// </summary>
        public bool Enabled { get; set; } = true;

        protected Renderer()
        {

        }

        /// <summary>
        /// Renders to the screen.
        /// </summary>
        public abstract void Render();
    }
}
