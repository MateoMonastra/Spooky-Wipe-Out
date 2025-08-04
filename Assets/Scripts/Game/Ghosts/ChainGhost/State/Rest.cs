using UnityEngine;
using UnityEngine.AI;

namespace Game.Ghosts.ChainGhost
{
    public class Rest : FSM.State
    {
        private readonly Transform _enemy;
        private readonly float _restDuration;
        private readonly System.Action _onRestComplete;
        private readonly Rigidbody _rigidbody;
        private readonly NavMeshAgent _agent;

        private float _timer;

        public Rest(
            Transform enemy, NavMeshAgent agent,
            float restDuration,
            System.Action onRestComplete)
        {
            _enemy = enemy;
            _restDuration = restDuration;
            _onRestComplete = onRestComplete;
            _agent = agent;
        }

        public override void Enter()
        {
            _timer = 0f;
            
            _agent.isStopped = true;
            _agent.ResetPath();
        }

        public override void Tick(float delta)
        {
            _timer += delta;

            if (_timer >= _restDuration)
            {
                _onRestComplete?.Invoke();
            }
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void DrawStateGizmos()
        {
#if UNITY_EDITOR
            UnityEditor.Handles.color = Color.cyan;
            UnityEditor.Handles.DrawWireDisc(_enemy.position, Vector3.up, 0.8f);
            UnityEditor.Handles.Label(_enemy.position + Vector3.up * 2f, "Resting...");
#endif
        }
    }
}