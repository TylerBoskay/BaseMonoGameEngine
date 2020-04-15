using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TDMonoGameEngine
{
    /// <summary>
    /// Handles mapping inputs on gamepads.
    /// </summary>
    public sealed class GamePadMapper : IUpdateable, ICleanup
    {
        public delegate void OnInputRemapCancelled(in string inputAction);
        public event OnInputRemapCancelled InputRemapCancelledEvent = null;

        public delegate void OnInputRemapped(in string actionRemapped, in int buttonMapped);
        public event OnInputRemapped InputRemappedEvent = null;

        private const double StartInputTimer = 5000d;

        private int InputIndex = 0;
        public string InputAction { get; private set; } = string.Empty;
        private int GamePadIndex = 0;
        public double InputTimeRemaining { get; private set; } = 0d;

        public bool IsActive { get; private set; } = false;
        private Buttons[] ButtonsToCheck = null;

        private GamePadState GPState = GamePadState.Default;
        private GamePadState CompareGPState = GamePadState.Default;

        public GamePadMapper(in int inputIndex)
        {
            InputIndex = inputIndex;

            ButtonsToCheck = EnumUtility.GetValues<Buttons>.EnumValues;
        }

        public void CleanUp()
        {
            InputRemapCancelledEvent = null;
            InputRemappedEvent = null;
        }

        public void StartMapInput(string inputAction, in int gamePadIndex)
        {
            InputAction = inputAction;
            GamePadIndex = gamePadIndex;
            InputTimeRemaining = StartInputTimer;

            GPState = GamePadInput.GetGamePadState(GamePadIndex);
            CompareGPState = GPState;

            IsActive = true;
        }

        public void EndMapInput()
        {
            InputTimeRemaining = 0d;
            IsActive = false;
        }

        public void Update()
        {
            if (IsActive == false)
                return;

            InputTimeRemaining -= Time.ElapsedTime.TotalMilliseconds;

            //End if the time is up or the gamepad isn't connected
            if (InputTimeRemaining <= 0d || GPState.IsConnected == false)
            {
                InputRemapCancelledEvent?.Invoke(InputAction);

                EndMapInput();
                return;
            }

            GamePadInput.GetGamePadState(GamePadIndex, ref CompareGPState);

            for (int i = 0; i < ButtonsToCheck.Length; i++)
            {
                Buttons button = ButtonsToCheck[i];

                if (GamePadInput.GetButtonDown(GamePadIndex, button, GPState, CompareGPState) == true)
                {
                    InputRemappedEvent?.Invoke(InputAction, (int)button);

                    EndMapInput();
                    return;
                }
            }

            GamePadInput.GetGamePadState(GamePadIndex, ref GPState);
        }
    }
}
