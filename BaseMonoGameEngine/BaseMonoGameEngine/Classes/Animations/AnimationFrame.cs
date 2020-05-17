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
    /// Represents an animation frame.
    /// </summary>
    public struct AnimationFrame : ICopyable<AnimationFrame>
    {
        public Rectangle DrawRegion;
        public double Duration;
        public Vector2 Pivot;

        public AnimationFrame(Rectangle drawRegion, double duration)
        {
            DrawRegion = drawRegion;
            Duration = duration;
            Pivot = new Vector2(.5f, .5f);
        }

        public AnimationFrame(Rectangle drawRegion, double duration, Vector2 pivot)
        {
            DrawRegion = drawRegion;
            Duration = duration;
            Pivot = pivot;
        }

        public override bool Equals(object obj)
        {
            if (obj is AnimationFrame animFrame)
            {
                return (DrawRegion == animFrame.DrawRegion && Duration == animFrame.Duration && Pivot == animFrame.Pivot);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 19) + DrawRegion.GetHashCode();
                hash = (hash * 19) + Duration.GetHashCode();
                hash = (hash * 19) + Pivot.GetHashCode();
                return hash;
            }
        }

        public AnimationFrame Copy()
        {
            return new AnimationFrame(DrawRegion, Duration, Pivot);
        }
    }
}
