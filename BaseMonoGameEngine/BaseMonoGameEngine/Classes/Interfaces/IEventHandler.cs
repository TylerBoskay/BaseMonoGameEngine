using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDMonoGameEngine
{
    /// <summary>
    /// Handles events.
    /// </summary>
    public interface IEventHandler
    {
        /// <summary>
        /// Handles an event.
        /// </summary>
        /// <param name="evt">The event to handle.</param>
        void HandleEvent(IEvent evt);
    }
}
