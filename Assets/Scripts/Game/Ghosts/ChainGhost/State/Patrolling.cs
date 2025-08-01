using System;
using FSM;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Ghosts.ChainGhost
{
    public class Patrolling : State
    {
        private readonly Transform _enemy;
        private readonly Transform _player;
        private readonly NavMeshAgent _agent;
        private readonly Transform[] _waypoints;
        private readonly float _detectRadius;
        private readonly LayerMask _obstructionMask;
        private readonly Action _onSeePlayer;
        private readonly float _headOffset = 1.5f;

        private int _currentWaypointIndex;

        public Patrolling(
            Transform enemy,
            Transform player,
            NavMeshAgent agent,
            Transform[] waypoints,
            float detectRadius,
            LayerMask obstructionMask,
            Action onSeePlayer)
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

            if (!_agent.pathPending && _agent.remainingDistance < 0.5f && _agent.velocity.magnitude < 0.05f)
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

            int safety = 0;
            float minDistance = 0.5f;
            float sampleRadius = 2f;

            while (safety < _waypoints.Length)
            {
                var target = _waypoints[_currentWaypointIndex];
                _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
                safety++;

                if (target == null)
                    continue;

                if (NavMesh.SamplePosition(target.position, out NavMeshHit hit, sampleRadius, NavMesh.AllAreas))
                {
                    float dist = Vector3.Distance(_enemy.position, hit.position);
                    if (dist > minDistance)
                    {
                        _agent.SetDestination(hit.position);
                        return;
                    }
                }
            }

            Debug.LogWarning($"{_enemy.name}: No valid waypoint found on NavMesh.");
        }

        public override void DrawStateGizmos()
        {
#if UNITY_EDITOR
            if (_waypoints != null)
            {
                Handles.color = Color.yellow;
                foreach (var wp in _waypoints)
                {
                    if (wp != null)
                        Handles.DrawSolidDisc(wp.position, Vector3.up, 0.2f);
                }
            }

            Handles.color = Color.green;
            Handles.DrawLine(_enemy.position, _agent.destination);

            if (_waypoints != null)
                foreach (var wp in _waypoints)
                {
                    if (wp != null)
                    {
                        Handles.color = NavMesh.SamplePosition(wp.position, out _, 1f, NavMesh.AllAreas)
                            ? Color.yellow
                            : Color.red;

                        Handles.DrawSolidDisc(wp.position, Vector3.up, 0.25f);
                    }
                }

#endif
        }
    }
}