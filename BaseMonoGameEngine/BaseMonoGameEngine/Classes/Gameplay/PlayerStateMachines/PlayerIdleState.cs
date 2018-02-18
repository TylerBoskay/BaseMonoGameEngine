using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BaseMonoGameEngine
{
    public class PlayerIdleState : PlayerStateMachine
    {
        private Vector2 StopSpeed = Vector2.Zero;

        public PlayerIdleState(Player playerRef, Vector2 stopSpeed) : base(playerRef)
        {
            StopSpeed = stopSpeed;
        }

        public override void Enter()
        {
            if (StopSpeed == Vector2.Zero)
            {
                PlayerRef.spriteRenderer.FlipData = SpriteEffects.None;
                PlayerRef.AnimationManager.PlayAnimation("StandD");
                return;
            }

            PlayerRef.spriteRenderer.FlipData = SpriteEffects.None;
            if (StopSpeed.X != 0)
            {
                PlayerRef.AnimationManager.PlayAnimation("StandL");
                if (StopSpeed.X > 0)
                    PlayerRef.spriteRenderer.FlipData = SpriteEffects.FlipHorizontally;
            }
            else if (StopSpeed.Y < 0) PlayerRef.AnimationManager.PlayAnimation("StandU");
            else if (StopSpeed.Y > 0) PlayerRef.AnimationManager.PlayAnimation("StandD");
        }

        public override void Exit()
        {
            
        }

        public override void Update()
        {
            HandleMove();
        }

        private void HandleMove()
        {
            Vector2 moveAmt = Vector2.Zero;

            moveAmt.X = Input.GetAxis(0, InputActions.Horizontal) * PlayerRef.Speed.X;
            moveAmt.Y = Input.GetAxis(0, InputActions.Vertical) * PlayerRef.Speed.Y;

            if (moveAmt != Vector2.Zero)
            {
                PlayerRef.transform.Position += moveAmt;

                PlayerRef.ChangeState(new PlayerWalkState(PlayerRef, moveAmt));
            }
        }
    }
}
