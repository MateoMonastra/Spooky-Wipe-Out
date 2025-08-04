using System;
using System.Collections.Generic;
using FSM;
using Minigames;
using Player.FSM;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using VacuumCleaner;

namespace Game.Player
{
    public class PlayerAgent : MonoBehaviour
    {
        public Action<Transform> OnHunted;

        public UnityEvent<bool> OnWalk;
        public UnityEvent<bool> OnStruggle;
        public UnityEvent<bool> OnCleaning;

        [SerializeField] private InputReader inputReader;
        [SerializeField] private WalkIdleModel walkIdleModel;
        [SerializeField] private ADController adController;
        [SerializeField] private SkillCheckController skillCheckController;
        [SerializeField] private CleanerController cleanerController;
        [SerializeField] private LayerMask layerRaycast;

        private readonly List<State> _states = new();
        private Fsm _fsm;

        private WalkIdle _walkIdle;
        private Struggle _struggle;
        private Trapped _trapped;

        private int _currentCleaner = 1;
        private Vector3 _lastMousePos;

        private readonly string _toTrappedID = "ToTrapped";
        private readonly string _toWalkIdleID = "ToWalkIdle";
        private readonly string _toStruggleID = "ToStruggle";

        private void Start()
        {
            SetupInput();
            SetupFsm();
        }

        private void SetupInput()
        {
            inputReader.OnMove += SetMoveStateDirection;
            inputReader.OnAimingVacuum += SetAimingVacuumDirection;
            inputReader.OnClickStart += ActiveCleaner;
            inputReader.OnClickEnd += SetCleanerIdleMode;
            inputReader.OnSwitchTool += SwitchTool;

            skillCheckController.OnStart += SetWalkIdleToStruggle;
            OnHunted += SetTrappedState;
        }

        private void SetupFsm()
        {
            _walkIdle = new WalkIdle(gameObject, walkIdleModel, layerRaycast, OnWalkAction);
            _trapped = new Trapped(gameObject, adController, SetTrappedToMoveState);
            _struggle = new Struggle(gameObject, cleanerController, skillCheckController, SetStruggleToWalkIdle);


            _walkIdle.AddTransition(new Transition { From = _walkIdle, To = _trapped, ID = _toTrappedID });
            _walkIdle.AddTransition(new Transition { From = _walkIdle, To = _walkIdle, ID = _toWalkIdleID });
            _walkIdle.AddTransition(new Transition { From = _walkIdle, To = _struggle, ID = _toStruggleID });

            _trapped.AddTransition(new Transition { From = _trapped, To = _walkIdle, ID = _toWalkIdleID });
            _struggle.AddTransition(new Transition { From = _struggle, To = _walkIdle, ID = _toWalkIdleID });

            _states.Add(_walkIdle);
            _states.Add(_trapped);
            _states.Add(_struggle);

            _fsm = new Fsm(_walkIdle);
        }


        private void Update()
        {
            _fsm.Update();

            if (!InputReader.isClickPressed || InputReader.isUsingController) return;
            
            SetAimingVacuumDirection(_lastMousePos);
        }


        private void FixedUpdate() => _fsm.FixedUpdate();

        private void OnWalkAction(bool isWalking) => OnWalk?.Invoke(isWalking);

        private void SetMoveStateDirection(Vector2 direction)
        {
            _fsm.TryTransitionTo(_toWalkIdleID);
            var moveDir = CalculateMoveDirection(direction);
            _walkIdle?.SetDir(moveDir);
        }

        private static Vector3 CalculateMoveDirection(Vector2 direction)
        {
            Vector3 moveDirection = new(direction.x, 0, direction.y);
            var cameraTransform = Camera.main.transform;
            var worldDirection = cameraTransform.TransformDirection(moveDirection);
            worldDirection.y = 0;
            return worldDirection;
        }

        public void SetAimingVacuumDirection(Vector2 position)
        {
            if (InputReader.isUsingController)
            {
                Vector3 worldPos = new Vector3(position.x, 0f, position.y);
                _lastMousePos = worldPos;
            }
            else
            {
                _lastMousePos = position;
            }
        
            foreach (var state in _states)
            {
                if (_fsm.GetCurrentState() == state && state is WalkIdle walkIdle)
                {
                    walkIdle.SetMousePosition(_lastMousePos);
                    break;
                }
            }
        }

        private void SetTrappedState(Transform trappedPos)
        {
            _fsm.TryTransitionTo(_toTrappedID);
            _trapped.SetPos(trappedPos);

            inputReader.OnClickStart -= ActiveCleaner;
            inputReader.OnMove -= SetMoveStateDirection;
            SetCleanerIdleMode();
        }

        private void SetTrappedToMoveState()
        {
            _fsm.TryTransitionTo(_toWalkIdleID);
            _walkIdle.SetDir(Vector2.zero);
            inputReader.OnClickStart += ActiveCleaner;
            inputReader.OnMove += SetMoveStateDirection;
        }

        private void SetWalkIdleToStruggle()
        {
            OnStruggle?.Invoke(true);
            inputReader.OnClickEnd -= SetCleanerIdleMode;
            inputReader.OnMove -= SetMoveStateDirection;
            _fsm.TryTransitionTo(_toStruggleID);
        }

        private void SetStruggleToWalkIdle()
        {
            inputReader.OnClickEnd += SetCleanerIdleMode;
            inputReader.OnMove += SetMoveStateDirection;
            SetCleanerIdleMode();
            OnStruggle?.Invoke(false);
            _fsm.TryTransitionTo(_toWalkIdleID);
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
                return;

            _currentCleaner++;
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

        public CleanerController GetCleanerController() => cleanerController;
    }
}