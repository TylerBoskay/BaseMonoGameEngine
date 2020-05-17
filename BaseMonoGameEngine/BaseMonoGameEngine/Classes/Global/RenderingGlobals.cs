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
    /// Class for global values dealing with rendering
    /// </summary>
    public static class RenderingGlobals
    {
        public const int BaseResolutionWidth = 640;
        public const int BaseResolutionHeight = 360;

        private static readonly Vector2 Resolution = new Vector2(BaseResolutionWidth, BaseResolutionHeight);
        private static readonly Vector2 ResolutionHalf = Resolution / 2;

        public static ref readonly Vector2 BaseResolution => ref Resolution;
        public static ref readonly Vector2 BaseResolutionHalved => ref ResolutionHalf;

        /// <summary>
        /// Resizes a RenderTarget by disposing it and pointing it to a new RenderTarget instance with the desired size.
        /// <para>If the RenderTarget is already the new size, nothing happens.
        /// If the RenderTarget is null, it will be created.</para>
        /// </summary>
        /// <param name="renderTarget">The RenderTarget to resize.</param>
        /// <param name="newSize">The new size of the RenderTarget.</param>
        public static void ResizeRenderTarget(ref RenderTarget2D renderTarget, in Vector2 newSize)
        {
            int newWidth = (int)newSize.X;
            int newHeight = (int)newSize.Y;

            if (renderTarget != null)
            {
                //Return if the RenderTarget is already this size
                if (renderTarget.Width == newWidth && renderTarget.Height == newHeight)
                    return;

                //Dispose the current RenderTarget, as they're not resizable
                if (renderTarget.IsDisposed == false)
                    renderTarget.Dispose();
            }

            //Point the reference to a new RenderTarget with the new size
            renderTarget = new RenderTarget2D(RenderingManager.Instance.graphicsDevice, newWidth, newHeight);
        }
    }
}
