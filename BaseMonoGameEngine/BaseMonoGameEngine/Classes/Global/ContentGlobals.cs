using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// Class for global values dealing with loading and unloading content
    /// </summary>
    public static class ContentGlobals
    {
        public const string ContentRoot = "Content/";
        public const string AudioRoot = "Audio/";
        public const string SFXRoot = AudioRoot + "SFX/";
        public const string MusicRoot = AudioRoot + "Music/";
        public const string SpriteRoot = "Sprites/";
        public const string ShaderRoot = "Shaders/";
        public const string ShaderTextureRoot = ShaderRoot + "ShaderTextures/";
        public const string FontRoot = "Fonts/";

        #region Assets

        public const string BoxTex = "Box";
        public static readonly Rectangle BoxRect = new Rectangle(0, 0, 1, 1);

        #endregion
    }
}
