using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BaseMonoGameEngine
{
    public sealed class PlayerDashState : PlayerStateMachine
    {
        private Vector2 Direction = Vector2.Zero;
        private bool PlayingLoop = false;

        public PlayerDashState(Player playerRef, Vector2 direction) : base(playerRef)
        {
            Direction = direction;
        }

        public override void Enter()
        {
            PlayAnimForDir(AnimationGlobals.PlayerAnimations.DashLeftStart, AnimationGlobals.PlayerAnimations.DashUpStart,
                AnimationGlobals.PlayerAnimations.DashDownStart);
        }

        public override void Exit()
        {
            
        }

        public override void HandleInput()
        {
            
        }

        public override void Update()
        {
            if (PlayingLoop == false)
            {
                if (PlayerRef.AnimationManager.CurrentAnim.CurFrameIndex == PlayerRef.AnimationManager.CurrentAnim.MaxFrameIndex)
                {
                    PlayAnimForDir(AnimationGlobals.PlayerAnimations.DashLeft, AnimationGlobals.PlayerAnimations.DashUp,
                        AnimationGlobals.PlayerAnimations.DashDown);

                    PlayingLoop = true;
                }
            }
            else
            {
                if (PlayerRef.AnimationManager.CurrentAnim.Loops >= 3)
                {
                    PlayerRef.ChangeState(new PlayerIdleState(PlayerRef, Direction, 0d));
                }
            }
        }

        private void PlayAnimForDir(string leftAnim, string upAnim, string downAnim)
        {
            PlayerRef.spriteRenderer.FlipData = SpriteEffects.None;

            if (Direction.X != 0)
            {
                PlayerRef.AnimationManager.PlayAnimation(leftAnim);
                if (Direction.X > 0)
                    PlayerRef.spriteRenderer.FlipData = SpriteEffects.FlipHorizontally;
            }
            else if (Direction.Y < 0)
            {
                PlayerRef.AnimationManager.PlayAnimation(upAnim);
            }
            else if (Direction.Y > 0)
            {
                PlayerRef.AnimationManager.PlayAnimation(downAnim);
            }
        }
    }
}
