using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TDMonoGameEngine
{
    public class TestEnemy2 : SceneObject
    {
        private SpriteRenderer spriteRenderer = null;

        private AnimManager AnimationManager = null;

        public TestEnemy2()
        {
            transform.Position = new Vector2(-100, 0);

            Texture2D tex = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}Enemies.png");

            spriteRenderer = new SpriteRenderer(transform, new Sprite(tex, new Rectangle(2, 2, 30, 32)));

            renderer = spriteRenderer;

            AnimationManager = new AnimManager(spriteRenderer.SpriteToRender);
            AnimationManager.AddAnimation("Idle", new SimpleAnimation(null, SimpleAnimation.AnimTypes.Looping,
                new AnimationFrame(new Rectangle(2, 2, 30, 32), 300d),
                new AnimationFrame(new Rectangle(34, 2, 26, 32), 300d)));
            AnimationManager.AddAnimation("Roll", new SimpleAnimation(null, SimpleAnimation.AnimTypes.Looping,
                new AnimationFrame(new Rectangle(175, 91, 16, 16), 100d),
                new AnimationFrame(new Rectangle(193, 91, 16, 16), 100d),
                new AnimationFrame(new Rectangle(211, 91, 16, 16), 100d)));
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetButton(0, InputActions.A))
            {
                AnimationManager.PlayAnimation("Idle");
            }
            else if (Input.GetButton(0, InputActions.B))
            {
                AnimationManager.PlayAnimation("Roll");
            }

            AnimationManager.CurrentAnim.Update();
        }
    }
}
