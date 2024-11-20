using System;
using System.Collections.Generic;
using AI.DecisionTree.Helpers;
using Fsm_Mk2;
using Gameplay.GhostMechanics;
using Ghosts.WalkingGhost;
using Minigames;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;
using State = Fsm_Mk2.State;

namespace Ghosts
{
    public class ChainGhostAgent : Ghost, IVacuumable
    {
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

        private List<State> _states = new List<State>();

        private Fsm _fsm;

        private Transition _fleeToStruggle;
        private Transition _struggleToCapture;
        private Transition _struggleToWalk;
        private Transition _struggleToFlee;
        private Transition _fleeToWalk;
        private Transition _walkToFlee;
        private Transition _startWalk;
        private Transition _fleeToRest;
        private Transition _walkToRest;
        private Transition _restToWalk;
        private Transition _restToFlee;
        private Transition _restToStruggle;

        [SerializeField] private bool logFsmStateChanges = false;

        AI.DecisionTree.Tree tree;

        private Dictionary<Type, Action> actionsByType = new();

        private NavMeshAgent _agent;

        private void Awake()
        {
            _patrollingGhost = GetComponent<GhostPatrolling>();
            _fleeGhost = GetComponent<GhostFlee>();
            _restGhost = GetComponent<GhostRest>();

            actionsByType.Add(typeof(Action_Patrolling), SetFleeWalkingState);
            actionsByType.Add(typeof(Action_Flee), SetWalkingFleeState);
            actionsByType.Add(typeof(Action_Rest), SetFleeRestState);

            isRested = true;
            currentHunterDistance = viewHunterDistance;
        }

        public void Start()
        {
            _agent = GetComponent<NavMeshAgent>();

            minigame.OnWin += SetCaptureState;
            minigame.OnLose += SetWalkingFleeState;
            minigame.OnStop += SetWalkingFleeState;

            GameManager.GetInstance().ghosts.Add(this);

            State _walk = new Walk(_patrollingGhost);
            _states.Add(_walk);

            State _struggle = new WalkingGhost.Struggle(_agent);
            _states.Add(_struggle);

            State _capture = new Capture(model, this, minigame);

            _states.Add(_capture);

            State _flee = new Flee(_fleeGhost);
            _states.Add(_flee);

            State _rest = new Rest(_restGhost, OnRested);
            _states.Add(_rest);

            _fleeToStruggle = new Transition() { From = _flee, To = _struggle };
            _flee.transitions.Add(_fleeToStruggle);

            _struggleToCapture = new Transition() { From = _struggle, To = _capture };
            _struggle.transitions.Add(_struggleToCapture);

            _struggleToFlee = new Transition() { From = _struggle, To = _flee };
            _struggleToFlee.TransitionAction += () =>
            {
                isRested = true;
                OnRested(isRested = true);
            };
            _struggle.transitions.Add(_struggleToFlee);

            _struggleToWalk = new Transition() { From = _struggle, To = _walk };
            _struggle.transitions.Add(_struggleToWalk);

            _fleeToWalk = new Transition() { From = _flee, To = _walk };
            _flee.transitions.Add(_fleeToWalk);

            _walkToFlee = new Transition() { From = _walk, To = _flee };
            _walkToFlee.TransitionAction += surprisedVfx.Play;
            _walk.transitions.Add(_walkToFlee);

            _startWalk = new Transition() { From = _walk, To = _walk };
            _walk.transitions.Add(_startWalk);

            _fleeToRest = new Transition() { From = _flee, To = _rest };
            _flee.transitions.Add(_fleeToRest);

            _walkToRest = new Transition() { From = _walk, To = _rest };
            _walk.transitions.Add(_walkToRest);

            _restToWalk = new Transition() { From = _rest, To = _walk };
            _rest.transitions.Add(_restToWalk);

            _restToFlee = new Transition() { From = _rest, To = _flee };
            _restToFlee.TransitionAction += surprisedVfx.Play;
            _rest.transitions.Add(_restToFlee);

            _restToStruggle = new Transition() { From = _rest, To = _struggle };
            _rest.transitions.Add(_restToStruggle);

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
                Debug.Log($"decision found: {type.Name}");
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
            _fsm.ApplyTransition(_fleeToStruggle);
            _fsm.ApplyTransition(_restToStruggle);


            minigame.StartGame();
        }

        private void SetCaptureState()
        {
            _fsm.ApplyTransition(_struggleToCapture);
        }

        private void SetFleeState()
        {
            OnVacuumed?.Invoke(false);

            _fsm.ApplyTransition(_struggleToFlee);
        }

        private void SetWalkState()
        {
            OnVacuumed?.Invoke(false);


            _fsm.ApplyTransition(_struggleToWalk);
        }

        private void SetFleeWalkingState()
        {
            currentHunterDistance = viewHunterDistance;
            _fsm.ApplyTransition(_fleeToWalk);
            _fsm.ApplyTransition(_restToWalk);
        }

        private void SetStartWalk()
        {
            _fsm.ApplyTransition(_startWalk);
        }

        private void SetWalkingFleeState()
        {
            OnVacuumed?.Invoke(false);
            currentHunterDistance = awareHunterDistance;
            _fsm.ApplyTransition(_walkToFlee);
            _fsm.ApplyTransition(_struggleToFlee);
            OnRested?.Invoke(true);
        }

        private void SetFleeRestState()
        {
            currentHunterDistance = viewHunterDistance;
            _fsm.ApplyTransition(_fleeToRest);
            _fsm.ApplyTransition(_walkToRest);
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