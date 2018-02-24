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
    /// Handles simple sprite animation.
    /// </summary>
    public class SimpleAnimation : IUpdateable
    {
        public enum AnimTypes
        {
            Normal,
            Looping
        }

        public Sprite SpriteToChange = null;
        protected readonly AnimationFrame[] AnimFrames = null;

        /// <summary>
        /// A key identifying the animation.
        /// </summary>
        public string Key { get; private set; } = string.Empty;

        public int MaxFrameIndex => (AnimFrames.Length - 1);
        public int CurFrameIndex { get; protected set; } = 0;

        public ref AnimationFrame CurFrame => ref AnimFrames[CurFrameIndex];

        public AnimTypes AnimType = AnimTypes.Normal;

        private double ElapsedFrameTime = 0d;

        public SimpleAnimation(Sprite spriteToChange, AnimTypes animType, params AnimationFrame[] frames)
        {
            SpriteToChange = spriteToChange;
            AnimType = animType;
            AnimFrames = frames;
        }
        
        public void SetKey(string key)
        {
            Key = key;
        }

        protected void Progress()
        {
            if (AnimType == AnimTypes.Normal)
            {
                CurFrameIndex = UtilityGlobals.Clamp(CurFrameIndex + 1, 0, MaxFrameIndex);
            }
            else
            {
                CurFrameIndex = UtilityGlobals.Wrap(CurFrameIndex + 1, 0, MaxFrameIndex);
            }

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

        /// <summary>
        /// Plays an animation from the start.
        /// </summary>
        public void Play()
        {
            CurFrameIndex = 0;

            ElapsedFrameTime = 0;

            SpriteToChange.SourceRect = CurFrame.DrawRegion;
        }
    }
}
