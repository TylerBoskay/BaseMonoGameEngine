using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDMonoGameEngine
{
    /// <summary>
    /// Any object with a <see cref="Transform"/>.
    /// </summary>
    public interface ITransformable
    {
        /// <summary>
        /// The Transform of the object.
        /// </summary>
        Transform transform { get; set; }
    }
}
