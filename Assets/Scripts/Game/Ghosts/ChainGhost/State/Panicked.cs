using UnityEngine;

namespace Game.Ghosts.ChainGhost
{
    public class Panicked : FSM.State
    {
        private Transform _enemy;
        private float _panicDuration;
        private float _timer;
        private System.Action _onPanicEnd;

        public Panicked(Transform enemy, float panicDuration, System.Action onPanicEnd)
        {
            _enemy = enemy;
            _panicDuration = panicDuration;
            _onPanicEnd = onPanicEnd;
        }

        public override void Enter()
        {
            _timer = 0f;
            Debug.Log($"{_enemy.name} entró en estado de PÁNICO!");
        }

        public override void Tick(float delta)
        {
            _timer += delta;
            
            _enemy.Rotate(Vector3.up * (180f * delta));

            if (_timer >= _panicDuration)
            {
                _onPanicEnd?.Invoke();
            }
        }

        public override void Exit()
        {
            Debug.Log($"{_enemy.name} salió del estado de pánico.");
        }

        public override void DrawStateGizmos()
        {
#if UNITY_EDITOR
            UnityEditor.Handles.color = Color.cyan;
            UnityEditor.Handles.DrawWireDisc(_enemy.position, Vector3.up, 2f);
#endif
        }
    }
}