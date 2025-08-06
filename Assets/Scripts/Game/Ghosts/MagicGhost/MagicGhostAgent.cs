using FSM;
using Game.Ghosts.ChainGhost;
using Ghosts;
using Minigames;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Game.Ghosts.MagicGhost
{
    public class MagicGhostAgent : Ghost, IVacuumable 
    {
        public UnityEvent onVacuum;
        public UnityEvent onFlee;
        public UnityEvent onDropTrash;
        public UnityEvent onDropTrashEnd;
        public UnityEvent onCaptured;
        public UnityEvent onPatroll;
        public UnityEvent onPanicked;


        [Header("References")]
        [SerializeField] private Transform player;
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private Transform[] patrolWaypoints;
        [SerializeField] private Minigame struggleMinigame;
        [SerializeField] private Collider ghostCollider;
        [SerializeField] private GameObject[] trashPrefabs;
        [SerializeField] private Transform trashSpawnPoint;

        [Header("Settings")] 
        [SerializeField] private float fleeSpeed = 6f;
        [SerializeField] private float detectRadius = 8f;
        [SerializeField] private float panicDuration = 3f;
        [SerializeField] private LayerMask obstructionMask;

        private Fsm _fsm;

        private Patrolling _patrolling;
        private Flee _flee;
        private DropTrash _dropTrash;
        private Struggle _struggle;
        private Captured _captured;
        private Panicked _panicked;

        private const string ToFleeID = "ToFlee";
        private const string ToPatrollingID = "ToPatrolling";
        private const string ToCapturedID = "ToCaptured";
        private const string ToPanickedID = "ToPanicked";
        private const string ToDropTrashID = "ToDropTrash";

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
                ToDropTrash,
                ToPanicked
            );

            //DropTrash
            _dropTrash = new DropTrash(
                navMeshAgent,
                trashPrefabs,
                trashSpawnPoint,
                this,
                OnDropTrashCompleted);

            

            // Transitions
            _patrolling.AddTransition(new Transition { From = _patrolling, To = _flee, ID = ToFleeID });
            _flee.AddTransition(new Transition { From = _flee, To = _dropTrash, ID = ToDropTrashID });
            _flee.AddTransition(new Transition { From = _flee, To = _panicked, ID = ToPanickedID });
            _panicked.AddTransition(new Transition { From = _panicked, To = _flee, ID = ToFleeID });
            _dropTrash.AddTransition(new Transition { From = _dropTrash, To = _patrolling, ID = ToPatrollingID });
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

        private void ToDropTrash()
        {
            _fsm.TryTransitionTo(ToDropTrashID);
            onDropTrash?.Invoke();
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

        private void OnDropTrashCompleted()
        {
            onDropTrashEnd?.Invoke();
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
            
            if(GameManager.GetInstance().IsAnyMinigameActive()) return;
            
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

        public void OnDeathAnimationEnd()
        {
            gameObject.SetActive(false);
        }

        public State GetCurrentState()
        {
            return _fsm.GetCurrentState();
        }
    }
}
