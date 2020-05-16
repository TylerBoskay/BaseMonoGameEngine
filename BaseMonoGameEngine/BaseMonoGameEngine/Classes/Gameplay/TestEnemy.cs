using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BaseMonoGameEngine
{
    public sealed class TestEnemy : SceneObject
    {
        public TestEnemy()
        {
            Name = "Pokey";

            Texture2D tex = AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.SpriteRoot}Pokey");

            PokeyRenderer pokeyRender = new PokeyRenderer(transform, null);

            Sprite pokeyBody = new Sprite(tex, new Rectangle(96, 37, 34, 25));

            pokeyRender.SpritesToRender.Add(new Sprite(tex, new Rectangle(96, 65, 32, 30)));
            pokeyRender.SpritesToRender.Add(pokeyBody);
            pokeyRender.SpritesToRender.Add(pokeyBody);
            pokeyRender.SpritesToRender.Add(pokeyBody);

            renderer = pokeyRender;

            //Effect outlineShader = AssetManager.Instance.LoadAsset<Effect>($"{ContentGlobals.ShaderRoot}Outline");
            //outlineShader.Parameters["outlineColor"].SetValue(new Vector4(1, 1, 1, 1));
            //outlineShader.Parameters["sheetSize"].SetValue(new Vector2(tex.Width, tex.Height));
            //
            //renderer.Shader = outlineShader;


            transform.Position = new Vector2(100, 0);
        }
    }
}
