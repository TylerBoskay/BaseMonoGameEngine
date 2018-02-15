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
    /// Handles simple sprite animation.
    /// </summary>
    public class SimpleAnimation : IUpdateable
    {
        public Sprite SpriteToChange = null;
        protected readonly AnimationFrame[] AnimFrames = null;

        private int MaxFrameIndex => (AnimFrames.Length - 1);
        private int CurFrameIndex = 0;

        public ref AnimationFrame CurFrame => ref AnimFrames[CurFrameIndex];

        private double ElapsedFrameTime = 0d;

        public SimpleAnimation(Sprite spriteToChange, params AnimationFrame[] frames)
        {
            SpriteToChange = spriteToChange;
            AnimFrames = frames;
        }
        
        protected void Progress()
        {
            CurFrameIndex = UtilityGlobals.Wrap(CurFrameIndex + 1, 0, MaxFrameIndex);

            ElapsedFrameTime = 0d;

            SpriteToChange.SourceRect = CurFrame.DrawRegion;
        }

        public void Update()
        {
            ElapsedFrameTime += Time.ElapsedMilliseconds;

            if (ElapsedFrameTime >= CurFrame.Duration)
            {
                Progress();
            }
        }
    }
}
