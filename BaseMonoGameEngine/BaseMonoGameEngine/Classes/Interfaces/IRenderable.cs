using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// An interface for objects that can be rendered.
    /// </summary>
    public interface IRenderable
    {
        /// <summary>
        /// The Renderer for the object.
        /// </summary>
        Renderer renderer { get; set; }
    }
}
