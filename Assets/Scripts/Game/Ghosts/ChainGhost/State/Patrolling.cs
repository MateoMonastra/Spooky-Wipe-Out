using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Ghosts.ChainGhost
{
    public class Patrolling : FSM.State
    {
        private readonly Transform _enemy;
        private readonly Transform _player;
        private readonly NavMeshAgent _agent;
        private readonly Transform[] _waypoints;
        private readonly float _detectRadius;
        private readonly LayerMask _obstructionMask;
        private readonly System.Action _onSeePlayer;
        private readonly float _headOffset = 1.5f;

        private int _currentWaypointIndex;

        public Patrolling(
            Transform enemy,
            Transform player,
            NavMeshAgent agent,
            Transform[] waypoints,
            float detectRadius,
            LayerMask obstructionMask,
            System.Action onSeePlayer)
        {
            _enemy = enemy;
            _player = player;
            _agent = agent;
            _waypoints = waypoints;
            _detectRadius = detectRadius;
            _obstructionMask = obstructionMask;
            _onSeePlayer = onSeePlayer;
        }

        public override void Enter()
        {
            GoToNextWaypoint();
        }

        public override void Tick(float delta)
        {
            float distanceToPlayer = Vector3.Distance(_enemy.position, _player.position);

            if (distanceToPlayer < _detectRadius)
            {
                Vector3 directionToPlayer = (_player.position - _enemy.position).normalized;
                Vector3 rayOrigin = _enemy.position + Vector3.up * _headOffset;

                if (!Physics.Raycast(rayOrigin, directionToPlayer, out RaycastHit hit, _detectRadius, _obstructionMask))
                {
                    _onSeePlayer?.Invoke();
                    return;
                }
            }

            if (!_agent.pathPending && _agent.remainingDistance < 0.2f)
            {
                GoToNextWaypoint();
            }
        }

        public override void Exit()
        {
            _agent.ResetPath();
        }

        private void GoToNextWaypoint()
        {
            if (_waypoints == null || _waypoints.Length == 0)
                return;

            _agent.SetDestination(_waypoints[_currentWaypointIndex].position);
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
        }
        
#if UNITY_EDITOR
        public override void DrawStateGizmos()
        {
            if (_enemy == null) return;
            
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(_enemy.position, Vector3.up, _detectRadius);
            
            if (_player != null)
            {
                Vector3 rayOrigin = _enemy.position + Vector3.up * _headOffset;
                Vector3 direction = (_player.position - _enemy.position).normalized;
                if (!Physics.Raycast(rayOrigin, direction, out RaycastHit hit, _detectRadius, _obstructionMask) &&
                    Vector3.Distance(_enemy.position, _player.position) < _detectRadius)
                {
                    Handles.color = Color.green;
                    Handles.DrawLine(rayOrigin, _player.position);
                }
            }
        }
#endif
    }
}