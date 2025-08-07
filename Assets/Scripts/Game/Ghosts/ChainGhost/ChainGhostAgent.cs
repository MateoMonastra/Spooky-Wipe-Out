using System.Collections;
using FSM;
using Game.Minigames;
using Ghosts;
using Minigames;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Game.Ghosts.ChainGhost
{
    public class ChainGhostAgent : Ghost, IVacuumable
    {
        public UnityEvent onVacuum;
        public UnityEvent onFlee;
        public UnityEvent onRest;
        public UnityEvent onRestEnd;
        public UnityEvent onCaptured;
        public UnityEvent onPatroll;
        public UnityEvent onPanicked;


        [Header("References")] [SerializeField]
        private Transform player;

        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private Transform[] patrolWaypoints;
        [SerializeField] private Minigame struggleMinigame;
        [SerializeField] private Collider ghostCollider;

        [Header("Settings")] [SerializeField] private float fleeSpeed = 6f;
        [SerializeField] private float detectRadius = 8f;
        [SerializeField] private float restDuration = 3f;
        [SerializeField] private float panicDuration = 3f;
        [SerializeField] private LayerMask obstructionMask;
        [SerializeField] private float minScale = 0.1f;
        [SerializeField] private float reductionSpeed = 3f;

        private Fsm _fsm;

        private Patrolling _patrolling;
        private Flee _flee;
        private Rest _rest;
        private Struggle _struggle;
        private Captured _captured;
        private Panicked _panicked;
        
        private Coroutine _scaleCoroutine;

        private const string ToFleeID = "ToFlee";
        private const string ToRestID = "ToRest";
        private const string ToPatrollingID = "ToPatrolling";
        private const string ToCapturedID = "ToCaptured";
        private const string ToPanickedID = "ToPanicked";

        private void Start()
        {
            GameManager.GetInstance().ghosts.Add(this);

            //Panicked
            _panicked = new Panicked(
                transform,
                panicDuration,
                ToFleePanicked
            );


            // Captured
            _captured = new Captured(
                model: gameObject,
                agent: this,
                ghostCollider,
                minigame: struggleMinigame
            );

            // Struggle
            _struggle = new Struggle(
                transform,
                player,
                navMeshAgent,
                struggleMinigame,
                onEnterCaptured: ToCaptured,
                onStruggleFail: ToFlee
            );

            // Patrolling
            _patrolling = new Patrolling(
                transform,
                player,
                navMeshAgent,
                patrolWaypoints,
                detectRadius,
                obstructionMask,
                onSeePlayer: ToFlee
            );

            // Flee
            _flee = new Flee(
                transform,
                player,
                navMeshAgent,
                patrolWaypoints,
                obstructionMask,
                fleeSpeed,
                ToRest,
                ToPanicked
            );


            // Rest
            _rest = new Rest(
                transform,
                navMeshAgent,
                restDuration,
                onRestComplete: OnRestCompleted
            );

            // Transitions
            _patrolling.AddTransition(new Transition { From = _patrolling, To = _flee, ID = ToFleeID });
            _flee.AddTransition(new Transition { From = _flee, To = _rest, ID = ToRestID });
            _flee.AddTransition(new Transition { From = _flee, To = _panicked, ID = ToPanickedID });
            _panicked.AddTransition(new Transition { From = _panicked, To = _flee, ID = ToFleeID });
            _rest.AddTransition(new Transition { From = _rest, To = _patrolling, ID = ToPatrollingID });
            _struggle.AddTransition(new Transition { From = _struggle, To = _captured, ID = ToCapturedID });
            _struggle.AddTransition(new Transition { From = _struggle, To = _flee, ID = ToFleeID });

            _fsm = new Fsm(_patrolling);
        }

        private void ToCaptured()
        {
            _fsm.TryTransitionTo(ToCapturedID);
            onCaptured?.Invoke();
        }

        private void ToFleePanicked()
        {
            _flee.SetCameFromPanic(true);
            _fsm.TryTransitionTo(ToFleeID);
            onFlee?.Invoke();
        }

        private void ToFlee()
        {
            _flee.SetCameFromPanic(false);
            _fsm.TryTransitionTo(ToFleeID);
            onFlee?.Invoke();
        }

        private void ToRest()
        {
            _fsm.TryTransitionTo(ToRestID);
            onRest?.Invoke();
        }

        private void ToPatrolling()
        {
            _fsm.TryTransitionTo(ToPatrollingID);
            onPatroll?.Invoke();
        }

        private void ToPanicked()
        {
            _fsm.TryTransitionTo(ToPanickedID);
            onPanicked?.Invoke();
        }

        private void OnRestCompleted()
        {
            onRestEnd?.Invoke();
            ToPatrolling();
        }

        private void Update()
        {
            _fsm.Update();
        }

        private void FixedUpdate()
        {
            _fsm.FixedUpdate();
        }

        private void OnDrawGizmos()
        {
            _fsm?.GetCurrentState()?.DrawStateGizmos();
        }

        private void EnterStruggle()
        {
            if (_fsm.GetCurrentState() == _struggle)
                return;

            if (GameManager.GetInstance().IsAnyMinigameActive()) return;

            struggleMinigame.OnWin += HandleStruggleWin;
            struggleMinigame.OnLose += HandleStruggleLose;

            struggleMinigame.StartGame();

            _fsm.ForceSetCurrentState(_struggle);
            onVacuum?.Invoke();
        }

        private void HandleStruggleWin()
        {
            CleanupMinigameCallbacks();
            _struggle.ResolveStruggle(true);
        }

        private void HandleStruggleLose()
        {
            CleanupMinigameCallbacks();
            _struggle.ResolveStruggle(false);
        }

        private void CleanupMinigameCallbacks()
        {
            struggleMinigame.OnWin -= HandleStruggleWin;
            struggleMinigame.OnLose -= HandleStruggleLose;
        }

        public void IsBeingVacuumed(params object[] args)
        {
            EnterStruggle();
        }

        public void OnDeathAnimationScaling()
        {
            _scaleCoroutine = StartCoroutine(DeathAnimationCoroutine());
        }

        private IEnumerator DeathAnimationCoroutine()
        {
            GameManager.GetInstance().SetMinigamesBloquedStatus(true);
            while (transform.localScale.x > minScale ||
                   transform.localScale.y > minScale ||
                   transform.localScale.z > minScale)
            {
                Vector3 newScale = transform.localScale - Vector3.one * (reductionSpeed * Time.deltaTime);

                newScale = new Vector3(
                    Mathf.Max(newScale.x, minScale),
                    Mathf.Max(newScale.y, minScale),
                    Mathf.Max(newScale.z, minScale)
                );

                transform.localScale = newScale;

                yield return null;
            }

            GameManager.GetInstance().SetMinigamesBloquedStatus(false);
            gameObject.SetActive(false);
        }

        public State GetCurrentState()
        {
            return _fsm.GetCurrentState();
        }
    }
}