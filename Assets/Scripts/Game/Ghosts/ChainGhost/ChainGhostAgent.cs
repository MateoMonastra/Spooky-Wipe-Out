using System;
using System.Collections.Generic;
using AI.DecisionTree.Helpers;
using FSM;
using Game.Ghosts.ChainGhost.State;
using Ghosts;
using Minigames;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Game.Ghosts.ChainGhost
{
    public class ChainGhostAgent : Ghost, IVacuumable
    {
        public Action OnFlee;
        public UnityEvent<bool> OnVacuumed;
        private Action<bool> OnRested = delegate { };

        [SerializeField] private Minigame minigame;
        [SerializeField] private GameObject model;
        [SerializeField] private TextAsset treeAsset;

        private GhostPatrolling _patrollingGhost;
        private GhostFlee _fleeGhost;
        private GhostRest _restGhost;

        [field: SerializeField] public float viewHunterDistance { get; private set; } = 8.0f;
        [field: SerializeField] public float awareHunterDistance { get; private set; } = 16.0f;
        [field: SerializeField] public float currentHunterDistance { get; private set; }
        [field: SerializeField] public bool isRested { get; set; }

        [SerializeField] private Transform hunter;
        [SerializeField] private ParticleSystem surprisedVfx;
        [SerializeField] private bool logFsmStateChanges = false;

        private List<State> _states = new List<State>();

        private string _toFleeID = "ToFlee";
        private string _toPatrollingID = "ToPatrolling";
        private string _toRestID = "ToRest";
        private string _toStruggleID = "ToStruggle";
        private string _toCaptureID = "ToCapture";

        private Fsm _fsm;

        private Rest _restState;
        AI.DecisionTree.Tree tree;

        private Dictionary<Type, Action> actionsByType = new();

        private NavMeshAgent _agent;

        private void Awake()
        {
            _patrollingGhost = GetComponent<GhostPatrolling>();
            _fleeGhost = GetComponent<GhostFlee>();
            _restGhost = GetComponent<GhostRest>();

            actionsByType.Add(typeof(Action_Patrolling), SetFleeWalkingState);
            actionsByType.Add(typeof(Action_Flee), SetWalkToFleeState);
            actionsByType.Add(typeof(Action_Rest), SetFleeRestState);

            isRested = true;
            currentHunterDistance = viewHunterDistance;
        }

        public void Start()
        {
            _agent = GetComponent<NavMeshAgent>();

            minigame.OnWin += SetCaptureState;
            minigame.OnLose += SetFleeMinigameState;
            minigame.OnStop += SetFleeMinigameState;

            GameManager.GetInstance().ghosts.Add(this);

            State _walk = new Patrolling(_patrollingGhost);
            _states.Add(_walk);

            State _struggle = new Struggle(_agent);
            _states.Add(_struggle);

            State _capture = new Captured(model, this, minigame);

            _states.Add(_capture);

            State _flee = new Flee(_fleeGhost);
            _states.Add(_flee);

            State _rest = new Rest(_restGhost);
            _states.Add(_rest);

            _restState = (_rest as Rest);
            OnRested += _restState.ChangeRest;

            Transition _fleeToStruggle = new Transition() { From = _flee, To = _struggle, ID = _toStruggleID };
            _flee.AddTransition(_fleeToStruggle);

            Transition _struggleToCapture = new Transition() { From = _struggle, To = _capture, ID = _toCaptureID };
            _struggle.AddTransition(_struggleToCapture);

            Transition _struggleToFlee = new Transition() { From = _struggle, To = _flee, ID = _toFleeID };

            _struggleToFlee.OnTransition += () =>
            {
                isRested = true;
                OnRested(isRested);
            };

            _struggle.AddTransition(_struggleToFlee);

            Transition struggleToWalk = new Transition() { From = _struggle, To = _walk, ID = _toPatrollingID };
            _struggle.AddTransition(struggleToWalk);

            Transition fleeToWalk = new Transition() { From = _flee, To = _walk, ID = _toPatrollingID };
            _flee.AddTransition(fleeToWalk);

            Transition walkToFlee = new Transition() { From = _walk, To = _flee, ID = _toFleeID };
            walkToFlee.OnTransition += surprisedVfx.Play;
            walkToFlee.OnTransition += () => OnFlee?.Invoke();
            _walk.AddTransition(walkToFlee);

            Transition startWalk = new Transition() { From = _walk, To = _walk, ID = _toPatrollingID };
            _walk.AddTransition(startWalk);

            Transition fleeToRest = new Transition() { From = _flee, To = _rest, ID = _toRestID };
            _flee.AddTransition(fleeToRest);

            Transition walkToRest = new Transition() { From = _walk, To = _rest, ID = _toRestID };
            _walk.AddTransition(walkToRest);

            Transition restToWalk = new Transition() { From = _rest, To = _walk, ID = _toPatrollingID };
            _rest.AddTransition(restToWalk);

            Transition restToFlee = new Transition() { From = _rest, To = _flee, ID = _toFleeID };
            restToFlee.OnTransition += surprisedVfx.Play;
            restToFlee.OnTransition += () => OnFlee?.Invoke();
            _rest.AddTransition(restToFlee);

            Transition restToStruggle = new Transition() { From = _rest, To = _struggle, ID = _toStruggleID };
            _rest.AddTransition(restToStruggle);

            _fsm = new Fsm(_walk);

            if (treeAsset != null)
            {
                tree = TreeHelper.LoadTree(treeAsset, GetGhostData);
                tree.Callback = HandleDecision;
            }
            else
            {
                Debug.Log("The path is empty");
            }

            _fleeGhost.SetRestState.AddListener(SetRestedState);
            _restGhost.SetRestState.AddListener(SetRestedState);
        }

        private void OnDisable()
        {
            _fleeGhost.SetRestState.RemoveListener(SetRestedState);
            _restGhost.SetRestState.RemoveListener(SetRestedState);
            OnRested -= _restState.ChangeRest;
        }

        private void SetRestedState(bool value)
        {
            isRested = value;
        }

        private void HandleDecision(object[] args)
        {
            if (args.Length == 0) return;

            if (args[0] is Type type && actionsByType.TryGetValue(type, out Action action))
            {
                action?.Invoke();
            }
        }

        private object GetGhostData()
        {
            return new GhostBehaviourData(this, hunter);
        }

        private void SetStruggleState()
        {
            if (minigame.GetActive()) return;

            OnVacuumed?.Invoke(true);
            gameObject.transform.forward = hunter.forward;
            _fsm.TryTransitionTo(_toStruggleID);

            minigame.StartGame();
        }

        private void SetCaptureState()
        {
            _fsm.TryTransitionTo(_toCaptureID);
        }

        private void SetFleeState()
        {
            OnVacuumed?.Invoke(false);

            _fsm.TryTransitionTo(_toFleeID);
        }

        private void SetFleeWalkingState()
        {
            currentHunterDistance = viewHunterDistance;
            _fsm.TryTransitionTo(_toPatrollingID);
        }

        private void SetWalkToFleeState()
        {
            OnVacuumed?.Invoke(false);
            currentHunterDistance = awareHunterDistance;
            _fsm.TryTransitionTo(_toFleeID);
        }

        private void SetFleeMinigameState()
        {
            OnVacuumed?.Invoke(false);
            currentHunterDistance = awareHunterDistance;
            _fsm.TryTransitionTo(_toFleeID);
            OnRested?.Invoke(true);
        }

        private void SetFleeRestState()
        {
            currentHunterDistance = viewHunterDistance;
            isRested = false;
            OnRested?.Invoke(false);
            _fsm.TryTransitionTo(_toRestID);
        }

        private void Update()
        {
            tree.RunTree();

            _fsm.Update();
            if (logFsmStateChanges)
                Debug.Log(_fsm.GetCurrentState());
        }

        private void FixedUpdate()
        {
            _fsm.FixedUpdate();
        }

        public void IsBeingVacuumed(params object[] args)
        {
            SetStruggleState();
        }

        public State GetCurrentState()
        {
            return _fsm.GetCurrentState();
        }
    }
}