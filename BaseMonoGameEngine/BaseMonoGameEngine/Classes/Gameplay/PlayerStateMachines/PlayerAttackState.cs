using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TDMonoGameEngine
{
    public class PlayerAttackState : PlayerStateMachine
    {
        private Vector2 Direction = Vector2.Zero;

        public PlayerAttackState(Player playerRef, Vector2 direction) : base(playerRef)
        {
            Direction = direction;
        }

        public override void Enter()
        {
            SoundManager.Instance.PlayRawSound($"{ContentGlobals.AudioRoot}SFX/Attack.wav");

            PlayerRef.spriteRenderer.FlipData = SpriteEffects.None;

            if (Direction.X != 0)
            {
                PlayerRef.AnimationManager.PlayAnimation(AnimationGlobals.PlayerAnimations.AttackLeft);
                if (Direction.X > 0)
                    PlayerRef.spriteRenderer.FlipData = SpriteEffects.FlipHorizontally;
            }
            else if (Direction.Y < 0)
            {
                PlayerRef.AnimationManager.PlayAnimation(AnimationGlobals.PlayerAnimations.AttackUp);
            }
            else if (Direction.Y > 0)
            {
                PlayerRef.AnimationManager.PlayAnimation(AnimationGlobals.PlayerAnimations.AttackDown);
            }
        }

        public override void Exit()
        {
            
        }

        public override void HandleInput()
        {
            
        }

        public override void Update()
        {
            if (PlayerRef.AnimationManager.CurrentAnim.CurFrameIndex == PlayerRef.AnimationManager.CurrentAnim.MaxFrameIndex)
            {
                PlayerRef.ChangeState(new PlayerIdleState(PlayerRef, Direction, 0d));
            }
        }
    }
}
