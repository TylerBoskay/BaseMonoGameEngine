using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TDMonoGameEngine
{
    public class PlayerIdleState : PlayerStateMachine
    {
        private Vector2 StopSpeed = Vector2.Zero;
        protected Vector2 MoveSpeed = Vector2.Zero;

        protected virtual ref readonly Vector2 GetAttackVec => ref StopSpeed;

        public PlayerIdleState(Player playerRef, Vector2 stopSpeed) : base(playerRef)
        {
            StopSpeed = stopSpeed;
        }

        public override void Enter()
        {
            if (StopSpeed == Vector2.Zero)
            {
                PlayerRef.spriteRenderer.FlipData = SpriteEffects.None;
                PlayerRef.AnimationManager.PlayAnimation(AnimationGlobals.PlayerAnimations.StandDown);
                return;
            }

            PlayerRef.spriteRenderer.FlipData = SpriteEffects.None;
            if (StopSpeed.X != 0)
            {
                PlayerRef.AnimationManager.PlayAnimation(AnimationGlobals.PlayerAnimations.StandLeft);
                if (StopSpeed.X > 0)
                    PlayerRef.spriteRenderer.FlipData = SpriteEffects.FlipHorizontally;
            }
            else if (StopSpeed.Y < 0) PlayerRef.AnimationManager.PlayAnimation(AnimationGlobals.PlayerAnimations.StandUp);
            else if (StopSpeed.Y > 0) PlayerRef.AnimationManager.PlayAnimation(AnimationGlobals.PlayerAnimations.StandDown);
        }

        public override void Exit()
        {
            
        }

        public override void HandleInput()
        {
            MoveSpeed = Vector2.Zero;

            MoveSpeed.X = Input.GetAxis(0, InputActions.Horizontal) * PlayerRef.Speed.X;
            MoveSpeed.Y = Input.GetAxis(0, InputActions.Vertical) * PlayerRef.Speed.Y;

            if (Input.GetButtonDown(0, InputActions.B) == true)
            {
                PlayerRef.ChangeState(new PlayerAttackState(PlayerRef, GetAttackVec));
            }
        }

        protected void UpdatePos()
        {
            if (MoveSpeed != Vector2.Zero)
            {
                PlayerRef.transform.Position += MoveSpeed;
            }
        }

        protected virtual void CheckChange()
        {
            if (MoveSpeed != Vector2.Zero)
            {
                PlayerRef.ChangeState(new PlayerWalkState(PlayerRef, MoveSpeed));
            }
        }

        public override void Update()
        {
            UpdatePos();
            CheckChange();
        }

        private void HandleMove()
        {
            MoveSpeed = Vector2.Zero;

            MoveSpeed.X = Input.GetAxis(0, InputActions.Horizontal) * PlayerRef.Speed.X;
            MoveSpeed.Y = Input.GetAxis(0, InputActions.Vertical) * PlayerRef.Speed.Y;

            if (MoveSpeed != Vector2.Zero)
            {
                PlayerRef.transform.Position += MoveSpeed;

                PlayerRef.ChangeState(new PlayerWalkState(PlayerRef, MoveSpeed));
                return;
            }

            if (Input.GetButtonDown(0, InputActions.B) == true)
            {
                PlayerRef.ChangeState(new PlayerAttackState(PlayerRef, StopSpeed));
            }
        }
    }
}
