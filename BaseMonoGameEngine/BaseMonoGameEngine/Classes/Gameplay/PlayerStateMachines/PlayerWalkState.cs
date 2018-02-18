using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BaseMonoGameEngine
{
    public class PlayerWalkState : PlayerStateMachine
    {
        private Vector2 MoveSpeed = Vector2.Zero;
        private Vector2 PrevSpeed = Vector2.Zero;

        private Vector2 CurDir = Vector2.Zero;
        private Vector2 MoveDir = Vector2.Zero;

        public PlayerWalkState(Player playerRef, Vector2 startSpeed) : base(playerRef)
        {
            MoveSpeed = startSpeed;
            PrevSpeed = MoveSpeed;
        }

        public override void Enter()
        {
            UpdateDirection(MoveSpeed);
            MoveDir = Vector2.Zero;
            SetAnimForDir();
        }

        public override void Exit()
        {
            
        }

        public override void Update()
        {
            HandleMove();
        }

        private void UpdateDirection(Vector2 moveAmt)
        {
            MoveDir = moveAmt;
            MoveDir.X = (MoveDir.X < 0) ? -1 : (MoveDir.X > 0) ? 1 : 0;
            MoveDir.Y = (MoveDir.Y < 0) ? -1 : (MoveDir.Y > 0) ? 1 : 0;

            CurDir = Vector2.Zero;

            if (moveAmt.Y > 0)
            {
                CurDir.Y = 1f;
            }
            else if (moveAmt.Y < 0)
            {
                CurDir.Y = -1f;
            }
            else if (moveAmt.X < 0)
            {
                CurDir.X = -1f;
            }
            else if (moveAmt.X > 0)
            {
                CurDir.X = 1f;
            }
        }

        private void SetAnimForDir()
        {
            PlayerRef.spriteRenderer.FlipData = SpriteEffects.None;

            if (CurDir.Y > 0)
            {
                PlayerRef.AnimationManager.PlayAnimationIfDiff("WalkD");
            }
            else if (CurDir.Y < 0)
            {
                PlayerRef.AnimationManager.PlayAnimationIfDiff("WalkU");
            }
            else if (CurDir.X != 0)
            {
                PlayerRef.AnimationManager.PlayAnimationIfDiff("WalkL");
            }

            if (MoveDir.X < 0 && PlayerRef.AnimationManager.CurrentAnim.Key == "WalkL")
                PlayerRef.spriteRenderer.FlipData = SpriteEffects.FlipHorizontally;
        }

        private void HandleMove()
        {
            MoveSpeed = Vector2.Zero;

            MoveSpeed.X = Input.GetAxis(0, InputActions.Horizontal) * PlayerRef.Speed.X;
            MoveSpeed.Y = Input.GetAxis(0, InputActions.Vertical) * PlayerRef.Speed.Y;

            UpdateDirection(MoveSpeed);
            SetAnimForDir();

            if (MoveSpeed == Vector2.Zero)
            {
                PlayerRef.ChangeState(new PlayerIdleState(PlayerRef, PrevSpeed));
                return;
            }
            else
            {
                PlayerRef.transform.Position += MoveSpeed;
            }

            PrevSpeed = MoveSpeed;
        }
    }
}
