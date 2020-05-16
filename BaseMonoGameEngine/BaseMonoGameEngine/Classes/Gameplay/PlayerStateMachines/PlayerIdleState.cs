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
        private const double MAX_CHARGE = 3500d;
        private const double CHARGE_COLOR_RATE = 1000d;
        private const double FULL_CHARGE_COLOR_RATE = 110d;

        private Vector2 StopSpeed = Vector2.Zero;
        protected Vector2 MoveSpeed = Vector2.Zero;

        protected virtual ref readonly Vector2 GetAttackVec => ref StopSpeed;

        protected double ChargeTime = 0d;

        public PlayerIdleState(Player playerRef, Vector2 stopSpeed, double chargeTime) : base(playerRef)
        {
            StopSpeed = stopSpeed;
            ChargeTime = chargeTime;
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

            if (Input.GetButton(InputActions.Left) == true)
            {
                MoveSpeed.X = -1f;
            }
            else if (Input.GetButton(InputActions.Right) == true)
            {
                MoveSpeed.X = 1f;
            }

            if (Input.GetButton(InputActions.Up) == true)
            {
                MoveSpeed.Y = -1f;
            }
            else if (Input.GetButton(InputActions.Down) == true)
            {
                MoveSpeed.Y = 1f;
            }

            MoveSpeed.X *= PlayerRef.Speed.X;
            MoveSpeed.Y *= PlayerRef.Speed.Y;

            if (Input.GetButtonDown(InputActions.B) == true)
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
                PlayerRef.ChangeState(new PlayerWalkState(PlayerRef, MoveSpeed, ChargeTime));
            }
        }

        public override void Update()
        {
            UpdatePos();
            CheckChange();

            if (Input.GetButton(InputActions.B) == true)
            {
                ChargeTime += Time.ElapsedTime.TotalMilliseconds;
                double chargeRate = CHARGE_COLOR_RATE;
                if (ChargeTime > MAX_CHARGE)
                    chargeRate = FULL_CHARGE_COLOR_RATE;

                float time = UtilityGlobals.PingPong(ChargeTime, (float)chargeRate);
                PlayerRef.spriteRenderer.TintColor = Color.Lerp(Color.White, Color.GreenYellow, time / (float)chargeRate);
            }
            else
            {
                //Do special move
                if (ChargeTime > MAX_CHARGE)
                {
                    PlayerRef.ChangeState(new PlayerDashState(PlayerRef, GetAttackVec));
                }

                if (ChargeTime > 0d)
                {
                    ChargeTime = 0d;
                    PlayerRef.spriteRenderer.TintColor = Color.White;
                }
            }
        }
    }
}
