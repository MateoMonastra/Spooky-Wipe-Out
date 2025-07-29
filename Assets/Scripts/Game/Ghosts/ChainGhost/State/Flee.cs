using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Ghosts.ChainGhost
{
    public class Flee : FSM.State
    {
        private readonly Transform _enemy;
        private readonly Transform _player;
        private readonly NavMeshAgent _agent;
        private readonly float _fleeDistance;
        private readonly float _escapeDuration;
        private readonly LayerMask _obstructionMask;
        private readonly System.Action _onEscape;
        private readonly float _headOffset = 1.5f;

        private Vector3 _lastTargetPosition;
        private float _timer = 0f;

        public Flee(
            Transform enemy,
            Transform player,
            NavMeshAgent agent,
            float fleeDistance,
            float escapeDuration,
            LayerMask obstructionMask,
            System.Action onEscape)
        {
            _enemy = enemy;
            _player = player;
            _agent = agent;
            _fleeDistance = fleeDistance;
            _escapeDuration = escapeDuration;
            _obstructionMask = obstructionMask;
            _onEscape = onEscape;
        }

        public override void Enter()
        {
            _timer = 0f;
            FleeFromPlayer();
        }

        public override void Tick(float delta)
        {
            float distance = Vector3.Distance(_enemy.position, _player.position);
            bool hasLineOfSight = !Physics.Raycast(_enemy.position + Vector3.up * _headOffset,
                                                    (_player.position - _enemy.position).normalized,
                                                    distance,
                                                    _obstructionMask);

            if (distance > _fleeDistance && !hasLineOfSight)
            {
                _timer += delta;
                if (_timer >= _escapeDuration)
                {
                    _onEscape?.Invoke();
                    return;
                }
            }
            else
            {
                _timer = 0f;
                FleeFromPlayer();
            }

            if (_agent.remainingDistance < 0.5f && !_agent.pathPending)
            {
                FleeFromPlayer();
            }
        }

        public override void Exit()
        {
            _agent.ResetPath();
        }

        private void FleeFromPlayer()
        {
            Vector3 fleeDirection = (_enemy.position - _player.position).normalized;
            Vector3 targetPosition = _enemy.position + fleeDirection * _fleeDistance;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPosition, out hit, 5f, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
                _lastTargetPosition = hit.position;
            }
        }
        
        
#if UNITY_EDITOR
        public override void DrawStateGizmos()
        {
            if (_enemy == null) return;

            Handles.color = Color.red;
            Handles.DrawWireDisc(_enemy.position, Vector3.up, _fleeDistance);

            if (_lastTargetPosition != Vector3.zero)
            {
                Handles.color = Color.red;
                Handles.DrawLine(_enemy.position, _lastTargetPosition);
            }
        }
#endif
    }
}
