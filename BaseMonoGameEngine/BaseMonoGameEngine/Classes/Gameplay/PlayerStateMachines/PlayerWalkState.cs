using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TDMonoGameEngine
{
    public class PlayerWalkState : PlayerIdleState
    {
        private Vector2 PrevSpeed = Vector2.Zero;

        private Vector2 CurDir = Vector2.Zero;
        private Vector2 MoveDir = Vector2.Zero;

        protected override ref readonly Vector2 GetAttackVec => ref CurDir;

        public PlayerWalkState(Player playerRef, Vector2 startSpeed, double chargeTime) : base(playerRef, startSpeed, chargeTime)
        {
            MoveSpeed = startSpeed;
            PrevSpeed = MoveSpeed;
        }

        public override void Enter()
        {
            UpdateDirection(MoveSpeed);
            SetAnimForDir();
        }

        public override void Exit()
        {
            
        }

        public override void HandleInput()
        {
            PrevSpeed = MoveSpeed;
            base.HandleInput();
        }

        protected override void CheckChange()
        {
            if (MoveSpeed == Vector2.Zero)
            {
                PlayerRef.ChangeState(new PlayerIdleState(PlayerRef, PrevSpeed, ChargeTime));
            }
        }

        public override void Update()
        {
            UpdateDirection(MoveSpeed);
            SetAnimForDir();

            base.Update();
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
                PlayerRef.AnimationManager.PlayAnimationIfDiff(AnimationGlobals.PlayerAnimations.WalkDown);
            }
            else if (CurDir.Y < 0)
            {
                PlayerRef.AnimationManager.PlayAnimationIfDiff(AnimationGlobals.PlayerAnimations.WalkUp);
            }
            else if (CurDir.X != 0)
            {
                PlayerRef.AnimationManager.PlayAnimationIfDiff(AnimationGlobals.PlayerAnimations.WalkLeft);
            }

            if (MoveDir.X < 0 && PlayerRef.AnimationManager.CurrentAnim.Key == AnimationGlobals.PlayerAnimations.WalkLeft)
                PlayerRef.spriteRenderer.FlipData = SpriteEffects.FlipHorizontally;
        }
    }
}
