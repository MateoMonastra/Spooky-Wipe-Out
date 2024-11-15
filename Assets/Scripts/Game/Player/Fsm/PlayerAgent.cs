using System;
using System.Collections.Generic;
using Minigames;
using Player.FSM;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using VacuumCleaner;


namespace Fsm_Mk2
{
    public class PlayerAgent : MonoBehaviour
    {
        public Action<Transform> OnHunted;

        public UnityEvent<bool> OnWalk;
        public UnityEvent<bool> OnStruggle;
        public UnityEvent<bool> OnCleaning;

        private List<State> _states = new List<State>();

        [SerializeField] private InputReader inputReader;
        [SerializeField] private WalkIdleModel walkIdleModel;

        [SerializeField] private ADController adController;
        [SerializeField] private SkillCheckController skillCheckController;

        [SerializeField] private CleanerController cleanerController;

        [SerializeField] private LayerMask layerRaycast;

        private Fsm _fsm;

        private Transition _walkIdleToTrapped;
        private Transition _walkIdleToWalkIdle;
        private Transition _trappedToWalkIdle;
        private Transition _walkIdleToStruggle;
        private Transition _struggleToWalkIdle;

        private int currentCleaner;


        public void Start()
        {
            currentCleaner = 1;

            inputReader.OnMove += SetMoveStateDirection;
            inputReader.OnAimingVacuum += SetAimingVacuumDirection;
            inputReader.OnClick += SetIsClickPressed;

            OnHunted += SetTrappedState;
            adController.OnLose += SetTrappedToMoveState;
            adController.OnWin += SetTrappedToMoveState;

            inputReader.OnClickStart += ActiveCleaner;
            inputReader.OnClickEnd += SetCleanerIdleMode;

            inputReader.OnSwitchTool += SwitchTool;

            skillCheckController.OnStart += SetWalkIdleToStruggle;
            skillCheckController.OnWin += SetStruggleToWalkIdle;
            skillCheckController.OnLose += SetStruggleToWalkIdle;
            skillCheckController.OnStop += SetStruggleToWalkIdle;

            skillCheckController.OnWin += SetCleanerIdleMode;
            skillCheckController.OnLose += SetCleanerIdleMode;
            skillCheckController.OnStop += SetCleanerIdleMode;

            State _walkIdle = new WalkIdle(this.gameObject, walkIdleModel, layerRaycast, OnWalkAction);
            _states.Add(_walkIdle);

            State _trapped = new Trapped(this.gameObject);
            _states.Add(_trapped);

            State _struggle = new Struggle(this.gameObject, cleanerController);
            _states.Add(_struggle);

            _walkIdleToTrapped = new Transition() { From = _walkIdle, To = _trapped };
            _walkIdle.transitions.Add(_walkIdleToTrapped);

            _walkIdleToWalkIdle = new Transition() { From = _walkIdle, To = _walkIdle };
            _walkIdle.transitions.Add(_walkIdleToWalkIdle);

            _trappedToWalkIdle = new Transition() { From = _trapped, To = _walkIdle };
            _trapped.transitions.Add(_trappedToWalkIdle);

            _walkIdleToStruggle = new Transition() { From = _walkIdle, To = _struggle };
            _walkIdle.transitions.Add(_walkIdleToStruggle);

            _struggleToWalkIdle = new Transition() { From = _struggle, To = _walkIdle };
            _struggle.transitions.Add(_struggleToWalkIdle);

            _fsm = new Fsm(_walkIdle);
        }

        private void OnWalkAction(bool obj)
        {
            OnWalk?.Invoke(obj);
        }

        private void SetMoveStateDirection(Vector2 direction)
        {
            //                                  Local direction (in relation to the world)
            Vector3 moveDirection = new Vector3(direction.x, 0, direction.y);
            var cameraTransform = Camera.main.transform;
            //                                      World direction (passed through the camera transform matrix)
            var cameraBasedMoveDirection = cameraTransform.TransformDirection(moveDirection);
            cameraBasedMoveDirection.y = 0;

            bool stateFound = false;

            foreach (var state in _states)
            {
                if (_fsm.GetCurrentState() == state)
                {
                    if (state is WalkIdle walkIdle)
                    {
                        _fsm.ApplyTransition(_walkIdleToWalkIdle);
                        walkIdle.SetDir(cameraBasedMoveDirection);
                        stateFound = true;

                        break;
                    }
                }
            }

            if (!stateFound)
            {
                Debug.Log("Current state not found in the list of states.");
            }
        }

        private void SetAimingVacuumDirection(Vector2 position)
        {
            Vector3 mousePosition = InputReader.isUsingController ? new Vector3(position.x, 0, position.y) : new Vector3(position.x, position.y);
            bool stateFound = false;

            foreach (var state in _states)
            {
                if (_fsm.GetCurrentState() == state)
                {
                    if (state is WalkIdle walkIdle)
                    {
                        walkIdle.SetMousePosition(mousePosition);
                        stateFound = true;
                        break;
                    }
                }
            }

            if (!stateFound)
            {
                Debug.Log("Current state not found in the list of states.");
            }
        }

        private void SetIsClickPressed(bool isPressed)
        {
            bool stateFound = false;

            foreach (var state in _states)
            {
                if (_fsm.GetCurrentState() == state)
                {
                    if (state is WalkIdle walkIdle)
                    {
                        walkIdle.SetIsClickPressedState(isPressed);
                        stateFound = true;
                        break;
                    }
                }
            }

            if (!stateFound)
            {
                Debug.Log("Current state not found in the list of states.");
            }
        }

        private void SetTrappedState(Transform trappedPos)
        {
            _fsm.ApplyTransition(_walkIdleToTrapped);

            bool stateFound = false;

            foreach (var state in _states)
            {
                if (_fsm.GetCurrentState() == state)
                {
                    if (state is Trapped trapped)
                    {
                        trapped.SetPos(trappedPos);
                        stateFound = true;
                        break;
                    }
                }
            }

            if (!stateFound)
            {
                Debug.Log("Current state not found in the list of states.");
            }
        }

        private void SetTrappedToMoveState()
        {
            _fsm.ApplyTransition(_trappedToWalkIdle);

            bool stateFound = false;

            foreach (var state in _states)
            {
                if (_fsm.GetCurrentState() == state)
                {
                    if (state is WalkIdle walkIdle)
                    {
                        _fsm.ApplyTransition(_walkIdleToWalkIdle);
                        walkIdle.SetDir(Vector2.zero);
                        stateFound = true;
                        break;
                    }
                }
            }

            if (!stateFound)
            {
                Debug.Log("Current state not found in the list of states.");
            }
        }

        private void ActiveCleaner()
        {
            OnCleaning?.Invoke(true);
            StartCoroutine(cleanerController.SwitchToTool(currentCleaner));
        }

        private void SetCleanerIdleMode()
        {
            OnCleaning?.Invoke(false);
            StartCoroutine(cleanerController.SwitchToTool(0));
        }
        private void SwitchTool()
        {
            currentCleaner += 1;

            switch (currentCleaner)
            {
                case 1:
                    CleanerSelectionUIControler.GetInstance().PowerOnVacuum();
                    break;

                case 2:
                    CleanerSelectionUIControler.GetInstance().PowerOnWashFloor();
                    break;

                default:
                    currentCleaner = 1;
                    CleanerSelectionUIControler.GetInstance().PowerOnVacuum();
                    break;
            }
        }

        private void SetStruggleToWalkIdle()
        {
            OnStruggle?.Invoke(false);
            _fsm.ApplyTransition(_struggleToWalkIdle);

        }

        private void SetWalkIdleToStruggle()
        {
            OnStruggle?.Invoke(true);
            SetIsClickPressed(false);
            _fsm.ApplyTransition(_walkIdleToStruggle);
        }

        private void Update()
        {
            _fsm.Update();
        }

        private void FixedUpdate()
        {
            _fsm.FixedUpdate();
        }

        public CleanerController GetCleanerController()
        {
            return cleanerController;
        }
    }
}