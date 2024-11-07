using System;
using EventSystems.EventSceneManager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.FSM
{
    public class InputReader : MonoBehaviour
    {
        public Action<Vector2> OnMove;
        public Action<Vector2> OnAimingVacuum;
        public Action<bool> OnClick;
        public Action OnClickStart;
        public Action OnClickPerform;
        public Action OnClickEnd;
        public Action OnSwitchTool;
        public Action OnSpaceInputStart;
        public Action OnPauseStart;
        public Action OnSeeTimerStart;
        public Action OnSeeTasksStart;
        public Action<InputDevice> OnInputDevice;

        public void HandleMoveInput(InputAction.CallbackContext context)
        {
            OnMove?.Invoke(context.ReadValue<Vector2>());
            OnInputDevice?.Invoke(context.control.device);
        }

        public void HandleMouseInput(InputAction.CallbackContext context)
        {
            OnAimingVacuum?.Invoke(context.ReadValue<Vector2>());
            OnInputDevice?.Invoke(context.control.device);
        }

        public void HandleClickInput(InputAction.CallbackContext context)
        {
            OnClick?.Invoke(context.performed);

            if (context.started)
            {
                OnClickStart?.Invoke();
            }

            if (context.performed)
            {
                OnClickPerform?.Invoke();
            }

            if (context.canceled)
            {
                OnClickEnd?.Invoke();
            }
            
            OnInputDevice?.Invoke(context.control.device);
        }

        public void HandleSwitchTool(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnSwitchTool?.Invoke();
            }
            OnInputDevice?.Invoke(context.control.device);
        }

        public void HandleSpaceInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnSpaceInputStart?.Invoke();
            }
            OnInputDevice?.Invoke(context.control.device);
        }
        
        public void HandlePause(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnPauseStart?.Invoke();
            }
            OnInputDevice?.Invoke(context.control.device);
        }

        public void HandleTimerUI(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnSeeTimerStart?.Invoke();
            }
            OnInputDevice?.Invoke(context.control.device);
        }

        public void HandleTasksUI(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnSeeTasksStart?.Invoke();
            }
            OnInputDevice?.Invoke(context.control.device);
        }
    }
}
