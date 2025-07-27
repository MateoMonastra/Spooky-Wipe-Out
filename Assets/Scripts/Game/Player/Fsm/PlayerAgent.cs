using System;
using System.Collections.Generic;
using FSM;
using Minigames;
using Player.FSM;
using UnityEngine;
using UnityEngine.Events;
using VacuumCleaner;

namespace Game.Player
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

        private string _toTrappedID = "ToTrapped";
        private string _toWalkIdleID = "ToWalkIdle";
        private string _toStruggleID = "ToStruggle";

        private int _currentCleaner;


        public void Start()
        {
            _currentCleaner = 1;

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

            State _trapped = new Trapped(this.gameObject);

            State _struggle = new Struggle(this.gameObject, cleanerController);

            _walkIdle.AddTransition(new Transition{ From = _walkIdle, To = _trapped, ID = _toTrappedID });
            _walkIdle.AddTransition(new Transition{ From = _walkIdle, To = _walkIdle, ID = _toWalkIdleID });
            _walkIdle.AddTransition(new Transition{ From = _walkIdle, To = _struggle, ID = _toStruggleID });
            
            _trapped.AddTransition(new Transition{ From = _trapped, To = _walkIdle, ID = _toWalkIdleID });

            _struggle.AddTransition(new Transition{ From = _struggle, To = _walkIdle, ID = _toWalkIdleID });

            _states.Add(_walkIdle);
            _states.Add(_trapped);
            _states.Add(_struggle);

            _fsm = new Fsm(_walkIdle);
        }

        private void OnWalkAction(bool obj)
        {
            OnWalk?.Invoke(obj);
        }

        private void SetMoveStateDirection(Vector2 direction)
        {
            var cameraBasedMoveDirection = CalculateMoveDirection(direction);

            _fsm.TryTransitionTo(_toWalkIdleID);
            
            foreach (var state in _states)
            {
                if (_fsm.GetCurrentState() == state)
                {
                    if (state is WalkIdle walkIdle)
                    {
                        walkIdle.SetDir(cameraBasedMoveDirection);
                        break;
                    }
                }
            }
        }

        private static Vector3 CalculateMoveDirection(Vector2 direction)
        {
            //Local direction (in relation to the world)
            Vector3 moveDirection = new Vector3(direction.x, 0, direction.y);
            var cameraTransform = Camera.main.transform;
            
            //World direction (passed through the camera transform matrix)
            var cameraBasedMoveDirection = cameraTransform.TransformDirection(moveDirection);
            cameraBasedMoveDirection.y = 0;
            return cameraBasedMoveDirection;
        }

        private void SetAimingVacuumDirection(Vector2 position)
        {
            Vector3 mousePosition = InputReader.isUsingController
                ? new Vector3(position.x, 0, position.y)
                : new Vector3(position.x, position.y);
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
            _fsm.TryTransitionTo(_toTrappedID);
            
            foreach (var state in _states)
            {
                if (_fsm.GetCurrentState() == state)
                {
                    if (state is Trapped trapped)
                    {
                        trapped.SetPos(trappedPos);
                        break;
                    }
                }
            }

            inputReader.OnClickStart -= ActiveCleaner;
            SetCleanerIdleMode();
        }

        private void SetTrappedToMoveState()
        {
            _fsm.TryTransitionTo(_toWalkIdleID);

            bool stateFound = false;

            foreach (var state in _states)
            {
                if (_fsm.GetCurrentState() == state)
                {
                    if (state is WalkIdle walkIdle)
                    {
                        _fsm.TryTransitionTo(_toWalkIdleID);
                        walkIdle.SetDir(Vector2.zero);
                        break;
                    }
                }
            }

            inputReader.OnClickStart += ActiveCleaner;
        }

        private void ActiveCleaner()
        {
            OnCleaning?.Invoke(true);
            StartCoroutine(cleanerController.SwitchToTool(_currentCleaner));
        }

        private void SetCleanerIdleMode()
        {
            OnCleaning?.Invoke(false);
            StartCoroutine(cleanerController.SwitchToTool(0));
        }

        private void SwitchTool()
        {
            if (cleanerController.GetCurrentToolID() != 0)
            {
                return;
            }

            _currentCleaner += 1;

            switch (_currentCleaner)
            {
                case 1:
                    CleanerSelectionUIControler.GetInstance().PowerOnVacuum();
                    break;

                case 2:
                    CleanerSelectionUIControler.GetInstance().PowerOnWashFloor();
                    break;

                default:
                    _currentCleaner = 1;
                    CleanerSelectionUIControler.GetInstance().PowerOnVacuum();
                    break;
            }
        }

        private void SetStruggleToWalkIdle()
        {
            OnStruggle?.Invoke(false);
            inputReader.OnClickEnd += SetCleanerIdleMode;
            _fsm.TryTransitionTo(_toWalkIdleID);
            SetIsClickPressed(false);
        }

        private void SetWalkIdleToStruggle()
        {
            OnStruggle?.Invoke(true);
            inputReader.OnClickEnd -= SetCleanerIdleMode;
            _fsm.TryTransitionTo(_toStruggleID);
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