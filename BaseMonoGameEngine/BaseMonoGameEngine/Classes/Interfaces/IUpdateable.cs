using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDMonoGameEngine
{
    /// <summary>
    /// An interface for objects that update.
    /// </summary>
    public interface IUpdateable
    {
        /// <summary>
        /// Performs update logic.
        /// </summary>
        void Update();
    }
}
