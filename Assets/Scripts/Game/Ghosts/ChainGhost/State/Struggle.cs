using FSM;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Ghosts.ChainGhost
{
    public class Struggle : FinishableState
    {
        private readonly Transform _enemy;
        private readonly Transform _player;
        private readonly NavMeshAgent _agent;
        private readonly System.Action _onEnterCaptured;
        private readonly System.Action _onStruggleFail;

        public Struggle(
            Transform enemy,
            Transform player,
            NavMeshAgent agent,
            System.Action onEnterCaptured,
            System.Action onStruggleFail)
        {
            _enemy = enemy;
            _player = player;
            _agent = agent;
            _onEnterCaptured = onEnterCaptured;
            _onStruggleFail = onStruggleFail;
        }

        public override void Enter()
        {
            base.Enter();
            
            _agent.ResetPath();
            _agent.updatePosition = false;
            _agent.updateRotation = false;
            
            _enemy.forward = _player.forward;
        }

        public override void Tick(float delta)
        {
            base.Tick(delta);
        }

        public override void Exit()
        {
            _agent.updatePosition = true;
            _agent.updateRotation = true;
        }

        public void ResolveStruggle(bool playerWon)
        {
            if (playerWon)
                _onEnterCaptured?.Invoke();
            else
                _onStruggleFail?.Invoke();
        }

        public override void DrawStateGizmos()
        {
#if UNITY_EDITOR
            UnityEditor.Handles.color = Color.magenta;
            UnityEditor.Handles.DrawWireDisc(_enemy.position, Vector3.up, 1.2f);
            UnityEditor.Handles.Label(_enemy.position + Vector3.up * 2f, "Struggling...");
#endif
        }
    }
}
