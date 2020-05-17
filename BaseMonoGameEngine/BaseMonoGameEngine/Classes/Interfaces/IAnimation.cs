using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// An interface for animations.
    /// </summary>
    public interface IAnimation : IUpdateable
    {
        Sprite SpriteToChange { get; set; }

        string Key { get; set; }

        AnimationFrame CurFrame { get; }

        AnimationFrame GetFrame(in int index);

        void Play();
    }
}
