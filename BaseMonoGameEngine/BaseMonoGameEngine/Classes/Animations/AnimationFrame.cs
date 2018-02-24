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
    /// Represents an animation frame.
    /// </summary>
    public struct AnimationFrame : ICopyable<AnimationFrame>
    {
        public Rectangle DrawRegion;
        public double Duration;

        public AnimationFrame(Rectangle drawRegion, double duration)
        {
            DrawRegion = drawRegion;
            Duration = duration;
        }

        public AnimationFrame Copy()
        {
            return new AnimationFrame(DrawRegion, Duration);
        }
    }
}
