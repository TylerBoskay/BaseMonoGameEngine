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
        public int Health = 31;
        public int MaxHealth = 38;
        
        public Vector2 Speed = new Vector2(1f, 1f);

        public AnimManager AnimationManager { get; private set; } = null;
        public SpriteRenderer spriteRenderer { get; private set; } = null;
        private Vector2 PrevSpeed = Vector2.Zero;

        private PlayerStateMachine CurStateMachine = null;

        public Player()
        {
            Sprite playerSprite = new Sprite(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.SpriteRoot}Will"),
                new Rectangle(210, 374, 25, 38));
            playerSprite.Pivot.Y = 1f;

            spriteRenderer = new SpriteRenderer(transform, playerSprite);
            spriteRenderer.Depth = .1f;

            renderer = spriteRenderer;

            AnimationManager = new AnimManager(spriteRenderer.SpriteToRender);
            AnimationManager.AddAnimation(AnimationGlobals.PlayerAnimations.StandDown, new SimpleAnimation(null, SimpleAnimation.AnimTypes.Normal,
                new AnimationFrame(new Rectangle(211, 375, 23, 36), 1000d)));
            AnimationManager.AddAnimation(AnimationGlobals.PlayerAnimations.StandUp, new SimpleAnimation(null, SimpleAnimation.AnimTypes.Normal,
                new AnimationFrame(new Rectangle(238, 374, 22, 37), 1000d)));
            AnimationManager.AddAnimation(AnimationGlobals.PlayerAnimations.StandLeft, new SimpleAnimation(null, SimpleAnimation.AnimTypes.Normal,
                new AnimationFrame(new Rectangle(266, 373, 18, 38), 1000d)));

            AnimationManager.AddAnimation(AnimationGlobals.PlayerAnimations.WalkDown, new SimpleAnimation(null, SimpleAnimation.AnimTypes.Looping,
                new AnimationFrame(new Rectangle(16, 45, 21, 38), 110d),
                new AnimationFrame(new Rectangle(41, 43, 23, 40), 110d),
                new AnimationFrame(new Rectangle(68, 45, 20, 38), 110d),
                new AnimationFrame(new Rectangle(92, 43, 23, 40), 110d)));
            AnimationManager.AddAnimation(AnimationGlobals.PlayerAnimations.WalkUp, new SimpleAnimation(null, SimpleAnimation.AnimTypes.Looping,
                new AnimationFrame(new Rectangle(18, 137, 22, 37), 110d),
                new AnimationFrame(new Rectangle(44, 135, 21, 39), 110d),
                new AnimationFrame(new Rectangle(69, 137, 22, 37), 110d),
                new AnimationFrame(new Rectangle(94, 135, 21, 39), 110d)));
            AnimationManager.AddAnimation(AnimationGlobals.PlayerAnimations.WalkLeft, new SimpleAnimation(null, SimpleAnimation.AnimTypes.Looping,
                new AnimationFrame(new Rectangle(20, 89, 19, 40), 110d),
                new AnimationFrame(new Rectangle(47, 91, 18, 38), 110d),
                new AnimationFrame(new Rectangle(71, 89, 18, 40), 110d),
                new AnimationFrame(new Rectangle(95, 92, 18, 37), 110d)));

            AnimationManager.AddAnimation(AnimationGlobals.PlayerAnimations.AttackDown, new SimpleAnimation(null, SimpleAnimation.AnimTypes.Normal,
                new AnimationFrame(new Rectangle(33, 710, 19, 35), 50d, new Vector2(.5f, .5f)),
                new AnimationFrame(new Rectangle(59, 710, 21, 35), 50d, new Vector2(.5f, .4f)),
                new AnimationFrame(new Rectangle(87, 710, 21, 40), 50d, new Vector2(.5f, .3f)),
                new AnimationFrame(new Rectangle(115, 710, 21, 43), 50d, new Vector2(.5f, .2f)),
                new AnimationFrame(new Rectangle(142, 711, 20, 44), 50d, new Vector2(.5f, .2f)),
                new AnimationFrame(new Rectangle(171, 710, 23, 35), 50d, new Vector2(.5f, .35f))));
            AnimationManager.AddAnimation(AnimationGlobals.PlayerAnimations.AttackUp, new SimpleAnimation(null, SimpleAnimation.AnimTypes.Normal,
                new AnimationFrame(new Rectangle(27, 606, 22, 36), 50d, new Vector2(.5f, .5f)),
                new AnimationFrame(new Rectangle(57, 606, 22, 36), 50d, new Vector2(.5f, .6f)),
                new AnimationFrame(new Rectangle(88, 598, 22, 44), 50d, new Vector2(.5f, .7f)),
                new AnimationFrame(new Rectangle(118, 594, 22, 48), 50d, new Vector2(.5f, .8f)),
                new AnimationFrame(new Rectangle(150, 594, 20, 48), 50d, new Vector2(.5f, .8f)),
                new AnimationFrame(new Rectangle(178, 606, 22, 36), 50d, new Vector2(.5f, .65f))));
            AnimationManager.AddAnimation(AnimationGlobals.PlayerAnimations.AttackLeft, new SimpleAnimation(null, SimpleAnimation.AnimTypes.Normal,
                new AnimationFrame(new Rectangle(32, 657, 16, 38), 50d, new Vector2(.5f, .5f)),
                new AnimationFrame(new Rectangle(58, 656, 23, 39), 50d, new Vector2(.6f, .5f)),
                new AnimationFrame(new Rectangle(85, 656, 39, 39), 50d, new Vector2(.7f, .5f)),
                new AnimationFrame(new Rectangle(131, 655, 39, 40), 50d, new Vector2(.8f, .5f)),
                new AnimationFrame(new Rectangle(178, 656, 39, 39), 50d, new Vector2(.8f, .5f)),
                new AnimationFrame(new Rectangle(226, 656, 39, 39), 50d, new Vector2(.8f, .5f)),
                new AnimationFrame(new Rectangle(58, 656, 23, 39), 50d, new Vector2(.65f, .5f))));

            AnimationManager.AddAnimation(AnimationGlobals.PlayerAnimations.DashDownStart, new SimpleAnimation(null, SimpleAnimation.AnimTypes.Normal,
                new AnimationFrame(new Rectangle(19, 379, 19, 36), 200d)));
            AnimationManager.AddAnimation(AnimationGlobals.PlayerAnimations.DashUpStart, new SimpleAnimation(null, SimpleAnimation.AnimTypes.Normal,
                new AnimationFrame(new Rectangle(19, 303, 20, 38), 200d)));
            AnimationManager.AddAnimation(AnimationGlobals.PlayerAnimations.DashLeftStart, new SimpleAnimation(null, SimpleAnimation.AnimTypes.Normal,
                new AnimationFrame(new Rectangle(17, 228, 20, 36), 200d)));

            AnimationManager.AddAnimation(AnimationGlobals.PlayerAnimations.DashDown, new SimpleAnimation(null, SimpleAnimation.AnimTypes.Looping,
                new AnimationFrame(new Rectangle(46, 375, 24, 40), 50d),
                new AnimationFrame(new Rectangle(78, 375, 24, 40), 50d),
                new AnimationFrame(new Rectangle(111, 375, 24, 40), 50d),
                new AnimationFrame(new Rectangle(144, 375, 24, 40), 50d)));
            AnimationManager.AddAnimation(AnimationGlobals.PlayerAnimations.DashUp, new SimpleAnimation(null, SimpleAnimation.AnimTypes.Looping,
                new AnimationFrame(new Rectangle(49, 299, 24, 42), 50d),
                new AnimationFrame(new Rectangle(83, 300, 24, 41), 50d),
                new AnimationFrame(new Rectangle(114, 299, 24, 42), 50d),
                new AnimationFrame(new Rectangle(146, 300, 24, 41), 50d)));
            AnimationManager.AddAnimation(AnimationGlobals.PlayerAnimations.DashLeft, new SimpleAnimation(null, SimpleAnimation.AnimTypes.Looping,
                new AnimationFrame(new Rectangle(51, 223, 23, 41), 50d),
                new AnimationFrame(new Rectangle(84, 224, 23, 40), 50d),
                new AnimationFrame(new Rectangle(117, 223, 24, 41), 50d),
                new AnimationFrame(new Rectangle(149, 224, 23, 40), 50d)));

            ChangeState(new PlayerIdleState(this, new Vector2(0, 1), 0d));
        }

        public override void CleanUp()
        {
            
        }

        public override void Update()
        {
            //ChangeColor();

            CurStateMachine.HandleInput();
            CurStateMachine.Update();

            AnimationManager.CurrentAnim?.Update();
        }

        public void ChangeState(PlayerStateMachine stateMachine)
        {
            //Exit the previous state machine
            CurStateMachine?.Exit();

            CurStateMachine = stateMachine;

            //Enter the new state
            CurStateMachine.Enter();
        }

        private void ChangeColor()
        {
            if (Input.GetButtonDown(0, InputActions.A) == true)
            {
                spriteRenderer.TintColor = Color.White;
            }
            else if (Input.GetButtonDown(0, InputActions.Y) == true)
            {
                spriteRenderer.TintColor = Color.White * .5f;
            }
            else if (Input.GetButtonDown(0, InputActions.X) == true)
            {
                spriteRenderer.TintColor = Color.Blue * (spriteRenderer.TintColor.A / 255f);
            }

            //if (Input.GetButtonDown(0, InputActions.A) == true)
            //{
            //    Health = UtilityGlobals.Clamp(Health + 1, 0, MaxHealth);
            //    spriteRenderer.TintColor = Color.White;
            //}
            //else if (Input.GetButtonDown(0, InputActions.B) == true)
            //{
            //    Health = UtilityGlobals.Clamp(Health - 1, 0, MaxHealth);
            //    spriteRenderer.TintColor = Color.Red;
            //}
            //else if (Input.GetButtonDown(0, InputActions.X) == true)
            //{
            //    spriteRenderer.TintColor = Color.GreenYellow;
            //}
            //else if (Input.GetButtonDown(0, InputActions.Y) == true)
            //{
            //    spriteRenderer.TintColor = Color.Blue;
            //}
        }
    }
}
