using System;
using FSM;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Ghosts.ChainGhost
{
    public class Flee : State
    {
        private Transform _enemy;
        private Transform _player;
        private NavMeshAgent _agent;
        private Transform[] _waypoints;
        private LayerMask _obstructionMask;
        private float _fleeDistance = 6f;
        private float _fleeSpeed;
        private float _originalSpeed;
        private bool _cameFromPanic;
        private Vector3 _lastTargetPosition;
        private float _stuckTimer;
        private readonly float _maxStuckTime = 1f;
        private Action _onEscape;
        private Action _onPanic;
        
        private float _timeSinceLastSeen;
        private float _lostSightThreshold = 3f;
        private float _headOffset = 1.5f;
        private float _detectRadius = 12f;


        public Flee(
            Transform enemy,
            Transform player,
            NavMeshAgent agent,
            Transform[] waypoints,
            LayerMask obstructionMask,
            float fleeSpeed,
            Action onEscape,
            Action onPanic)
        {
            _enemy = enemy;
            _player = player;
            _agent = agent;
            _waypoints = waypoints;
            _obstructionMask = obstructionMask;
            _fleeSpeed = fleeSpeed;
            _onEscape = onEscape;
            _onPanic = onPanic;
        }

        public void SetCameFromPanic(bool value) => _cameFromPanic = value;
        public override void Enter()
        {
            _originalSpeed = _agent.speed;
            _agent.speed = _fleeSpeed;
            _stuckTimer = 0f;

            if (_cameFromPanic)
            {
                FleeToDistantWaypoint();
                _cameFromPanic = false;
            }
            else
            {
                FleeFromPlayer();
            }
        }
        public override void Tick(float delta)
        {
            if (_cameFromPanic)
            {
                HandleWaypointEscape();
                return;
            }

            if (PanicCheck(delta))
                return;

            HandleSightCheck(delta);
            HandleFleeMovement();
        }
        public override void Exit()
        {
            _agent.ResetPath();
            _agent.speed = _originalSpeed;
        }
        private void HandleSightCheck(float delta)
        {
            if (CanSeePlayer())
            {
                _timeSinceLastSeen = 0f;
            }
            else
            {
                _timeSinceLastSeen += delta;

                if (_timeSinceLastSeen >= _lostSightThreshold)
                {
                    _onEscape?.Invoke();
                }
            }
        }
        private void HandleFleeMovement()
        {
            if (_agent.remainingDistance < 0.5f && !_agent.pathPending)
            {
                FleeFromPlayer();
            }
        }
        private bool CanSeePlayer()
        {
            float distanceToPlayer = Vector3.Distance(_enemy.position, _player.position);

            if (distanceToPlayer < _detectRadius)
            {
                Vector3 directionToPlayer = (_player.position - _enemy.position).normalized;
                Vector3 rayOrigin = _enemy.position + Vector3.up * _headOffset;

                return !Physics.Raycast(rayOrigin, directionToPlayer, distanceToPlayer, _obstructionMask);
            }

            return false;
        }
        private void HandleWaypointEscape()
        {
            if (_agent.remainingDistance < 0.5f && !_agent.pathPending)
            {
                _cameFromPanic = false;
                FleeFromPlayer();
            }
        }
        private bool PanicCheck(float delta)
        {
            if (_agent.velocity.sqrMagnitude < 0.01f)
                _stuckTimer += delta;
            else
                _stuckTimer = 0f;

            if (_stuckTimer >= _maxStuckTime)
            {
                _onPanic?.Invoke();
                return true;
            }

            return false;
        }
        private void FleeFromPlayer()
        {
            Vector3 fleeDirection = (_enemy.position - _player.position).normalized;
            Vector3 targetPosition = _enemy.position + fleeDirection * _fleeDistance;

            if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
                _lastTargetPosition = hit.position;
            }
            else
            {
                FleeToDistantWaypoint();
            }
        }
        private void FleeToDistantWaypoint()
        {
            Transform bestWaypoint = null;
            float maxDistance = -1f;

            foreach (var wp in _waypoints)
            {
                if (wp == null) continue;

                float distanceToPlayer = Vector3.Distance(wp.position, _player.position);

                if (distanceToPlayer > maxDistance &&
                    NavMesh.SamplePosition(wp.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                {
                    bestWaypoint = wp;
                    maxDistance = distanceToPlayer;
                }
            }

            if (bestWaypoint != null)
            {
                _agent.SetDestination(bestWaypoint.position);
                _lastTargetPosition = bestWaypoint.position;
            }
            else
            {
                Debug.LogWarning($"{_enemy.name}: No hay waypoint lejano disponible.");
            }
        }
        public override void DrawStateGizmos()
        {
#if UNITY_EDITOR
            Handles.color = Color.red;
            Handles.DrawWireDisc(_enemy.position, Vector3.up, 5f);
            Handles.color = Color.magenta;
            Handles.DrawDottedLine(_enemy.position, _lastTargetPosition, 3f);

            if (_waypoints != null)
            {
                foreach (var wp in _waypoints)
                {
                    if (wp != null)
                        Handles.DrawSolidDisc(wp.position, Vector3.up, 0.2f);
                }
            }
#endif
        }
    }
}
