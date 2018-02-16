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
        public Vector2 Speed = new Vector2(2f, 2f);

        private AnimManager AnimationManager = null;
        private SpriteRenderer spriteRenderer = null;
        private Vector2 PrevSpeed = Vector2.Zero;

        public Player()
        {
            Sprite playerSprite = new Sprite(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}Will.png"),
                new Rectangle(210, 374, 25, 38));
            playerSprite.Pivot.Y = 1f;

            spriteRenderer = new SpriteRenderer(transform, playerSprite);
            spriteRenderer.Depth = .1f;

            renderer = spriteRenderer;

            AnimationManager = new AnimManager(spriteRenderer.SpriteToRender);
            AnimationManager.AddAnimation("StandD", new SimpleAnimation(null, SimpleAnimation.AnimTypes.Normal,
                new AnimationFrame(new Rectangle(211, 375, 23, 36), 1000d)));
            AnimationManager.AddAnimation("StandU", new SimpleAnimation(null, SimpleAnimation.AnimTypes.Normal,
                new AnimationFrame(new Rectangle(238, 374, 22, 37), 1000d)));
            AnimationManager.AddAnimation("StandL", new SimpleAnimation(null, SimpleAnimation.AnimTypes.Normal,
                new AnimationFrame(new Rectangle(266, 373, 18, 38), 1000d)));

            AnimationManager.AddAnimation("WalkD", new SimpleAnimation(null, SimpleAnimation.AnimTypes.Looping,
                new AnimationFrame(new Rectangle(16, 45, 21, 38), 110d),
                new AnimationFrame(new Rectangle(41, 43, 23, 40), 110d),
                new AnimationFrame(new Rectangle(68, 45, 20, 38), 110d),
                new AnimationFrame(new Rectangle(92, 43, 23, 40), 110d)));
            AnimationManager.AddAnimation("WalkU", new SimpleAnimation(null, SimpleAnimation.AnimTypes.Looping,
                new AnimationFrame(new Rectangle(18, 137, 22, 37), 110d),
                new AnimationFrame(new Rectangle(44, 135, 21, 39), 110d),
                new AnimationFrame(new Rectangle(69, 137, 22, 37), 110d),
                new AnimationFrame(new Rectangle(94, 135, 21, 39), 110d)));
            AnimationManager.AddAnimation("WalkL", new SimpleAnimation(null, SimpleAnimation.AnimTypes.Looping,
                new AnimationFrame(new Rectangle(20, 89, 19, 40), 110d),
                new AnimationFrame(new Rectangle(47, 91, 18, 38), 110d),
                new AnimationFrame(new Rectangle(71, 89, 18, 40), 110d),
                new AnimationFrame(new Rectangle(95, 92, 18, 37), 110d)));
        }

        public override void CleanUp()
        {
            
        }

        public override void Update()
        {
            HandleMove();
            ChangeColor();

            AnimationManager.CurrentAnim?.Update();
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
                spriteRenderer.FlipData = SpriteEffects.None;
                transform.Position += moveAmt;

                if (moveAmt.Y > 0)
                {
                    if (moveAmt.X == 0 && AnimationManager.CurrentAnim.Key != "WalkD")
                    {
                        AnimationManager.PlayAnimation("WalkD");
                    }
                }
                else if (moveAmt.Y < 0)
                {
                    if (moveAmt.X == 0 && AnimationManager.CurrentAnim.Key != "WalkU")
                    {
                        AnimationManager.PlayAnimation("WalkU");
                    }
                }
                else if (moveAmt.X != 0)
                {
                    if (moveAmt.Y == 0 && AnimationManager.CurrentAnim.Key != "WalkL")
                    {
                        AnimationManager.PlayAnimation("WalkL");
                    }
                }

                if (moveAmt.X < 0 && AnimationManager.CurrentAnim.Key == "WalkL")
                    spriteRenderer.FlipData = SpriteEffects.FlipHorizontally;
            }
            else
            {
                if (PrevSpeed != Vector2.Zero)
                {
                    spriteRenderer.FlipData = SpriteEffects.None;
                    if (PrevSpeed.X != 0)
                    {
                        AnimationManager.PlayAnimation("StandL");
                        if (PrevSpeed.X > 0)
                            spriteRenderer.FlipData = SpriteEffects.FlipHorizontally;
                    }
                    else if (PrevSpeed.Y < 0) AnimationManager.PlayAnimation("StandU");
                    else if (PrevSpeed.Y > 0) AnimationManager.PlayAnimation("StandD");
                }
            }

            PrevSpeed = moveAmt;
        }
    }
}
