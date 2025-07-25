using System;
using EventSystems.EventSceneManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
        public Action OnWinCheat;
        public Action OnShowTimer;
        public Action OnHideTimer;
        public Action OnShowTasks;
        public Action OnHideTasks;
        public Action<InputDevice> OnInputDevice;
        public Action OnNavigate;
        public Action OnIntructionsStart;
        public Action OnIntructionsEnd;

        public static bool isUsingController = false;


        public void HandleMoveInput(InputAction.CallbackContext context)
        {
            OnMove?.Invoke(context.ReadValue<Vector2>());
            OnInputDevice?.Invoke(context.control.device);
        }

        public void HandleInputChange(PlayerInput context)
        {
            if (context.devices[0] is Mouse || context.devices[0] is Keyboard)
            {
                isUsingController = false;
            }
            else if (context.devices[0] is Gamepad)
            {
                isUsingController = true;
            }
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

        public void HandleWinCheat(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnWinCheat?.Invoke();
            }
        }

        public void HandleTimerUI(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnShowTimer?.Invoke();
            }
            else if (context.canceled)
            {
                OnHideTimer?.Invoke();
            }

            OnInputDevice?.Invoke(context.control.device);
        }

        public void HandleTasksUI(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnShowTasks?.Invoke();
            }
            else if (context.canceled)
            {
                OnHideTasks?.Invoke();
            }

            OnInputDevice?.Invoke(context.control.device);
        }

        public void HandleNavigate(InputAction.CallbackContext context)
        {
            OnNavigate?.Invoke();
            Debug.Log("se invoca evento ui");
        }

        public void HandleShowIntructions(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnIntructionsStart?.Invoke();
            }
            else if (context.canceled)
            {
                OnIntructionsEnd?.Invoke();
            }
        }

        public void HandleRestart(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                SceneManager.LoadScene(sceneBuildIndex:0);
            }
        }
    }
}