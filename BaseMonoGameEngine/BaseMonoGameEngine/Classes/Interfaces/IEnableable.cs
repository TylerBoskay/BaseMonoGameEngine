using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDMonoGameEngine
{
    /// <summary>
    /// An interface for objects that can be enabled and disabled.
    /// </summary>
    public interface IEnableable
    {
        /// <summary>
        /// Whether the object is enabled or not.
        /// </summary>
        bool Enabled { get; set; }

        // <summary>
        // What happens when the object is enabled.
        // </summary>
        //void OnEnabled();

        // <summary>
        // What happens when the object is disabled.
        // </summary>
        //void OnDisabled();
    }
}
