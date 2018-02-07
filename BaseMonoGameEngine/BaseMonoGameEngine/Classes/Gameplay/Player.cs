using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// Player object
    /// </summary>
    public class Player : SceneObject
    {
        public Vector2 Speed = Vector2.One;

        private Sprite playerSprite = null;
        private SpriteRenderer spriteRenderer = null;

        public Player()
        {
            playerSprite = new Sprite(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}Will.png"),
                new Rectangle(212, 376, 21, 34));

            //Effect outline = AssetManager.Instance.LoadAsset<Effect>($"{ContentGlobals.ShaderRoot}Outline");
            //
            //Vector2 texelSize = new Vector2((float)(1 / (double)playerSprite.Tex.Width), (float)(1 / (double)playerSprite.Tex.Height));
            //
            //outline.Parameters["outlineColor"].SetValue(new Vector4(1f, 1f, 1f, 1f));
            //outline.Parameters["texelSize"].SetValue(texelSize);

            spriteRenderer = new SpriteRenderer(transform, playerSprite);
            //spriteRenderer.Shader = outline;

            renderer = spriteRenderer;
        }

        public override void CleanUp()
        {

        }

        public override void Update()
        {
            HandleMove();
            ChangeColor();
        }

        private void ChangeColor()
        {
            if (Input.GetButtonDown(0, InputActions.A) == true)
            {
                spriteRenderer.TintColor = Color.White;
            }
            else if (Input.GetButtonDown(0, InputActions.B) == true)
            {
                spriteRenderer.TintColor = Color.Red;
            }
            else if (Input.GetButtonDown(0, InputActions.X) == true)
            {
                spriteRenderer.TintColor = Color.GreenYellow;
            }
            else if (Input.GetButtonDown(0, InputActions.Y) == true)
            {
                spriteRenderer.TintColor = Color.Blue;
            }
        }

        private void HandleMove()
        {
            Vector2 moveAmt = Vector2.Zero;

            moveAmt.X = Input.GetAxis(0, InputActions.Horizontal) * Speed.X;
            moveAmt.Y = Input.GetAxis(0, InputActions.Vertical) * Speed.Y;

            if (moveAmt != Vector2.Zero)
            {
                transform.Position += moveAmt;
            }
        }
    }
}
