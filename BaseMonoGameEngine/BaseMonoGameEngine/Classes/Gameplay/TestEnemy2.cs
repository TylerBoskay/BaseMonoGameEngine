using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BaseMonoGameEngine
{
    public class TestEnemy2 : SceneObject
    {
        private SpriteRenderer spriteRenderer = null;
        private SimpleAnimation Anim = null;

        public TestEnemy2()
        {
            transform.Position = new Vector2(-100, 0);

            Texture2D tex = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}Enemies.png");

            spriteRenderer = new SpriteRenderer(transform, new Sprite(tex, new Rectangle(2, 2, 30, 32)));

            renderer = spriteRenderer;

            Anim = new SimpleAnimation(spriteRenderer.SpriteToRender, new AnimationFrame(new Rectangle(2, 2, 30, 32), 300d), 
                new AnimationFrame(new Rectangle(34, 2, 26, 32), 300d));
        }

        public override void Update()
        {
            base.Update();

            Anim.Update();
        }
    }
}
