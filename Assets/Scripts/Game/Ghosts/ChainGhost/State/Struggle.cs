using FSM;
using Game.Minigames;
using UnityEngine;
using UnityEngine.AI;
using Minigames;

namespace Game.Ghosts.ChainGhost
{
    public class Struggle : FinishableState
    {
        private readonly Transform _enemy;
        private readonly Transform _player;
        private readonly NavMeshAgent _agent;
        private readonly Minigame _minigame;
        private readonly System.Action _onEnterCaptured;
        private readonly System.Action _onStruggleFail;

        private float _struggleLerpDistance = 3f; 
        private float _finalOffsetDistance = 1f; 
        private float _lerpSpeed = 1.5f;

        public Struggle(
            Transform enemy,
            Transform player,
            NavMeshAgent agent,
            Minigame minigame,
            System.Action onEnterCaptured,
            System.Action onStruggleFail)
        {
            _enemy = enemy;
            _player = player;
            _agent = agent;
            _minigame = minigame;
            _onEnterCaptured = onEnterCaptured;
            _onStruggleFail = onStruggleFail;
        }

        public override void Enter()
        {
            base.Enter();

            _agent.ResetPath();
            _agent.updatePosition = false;
            _agent.updateRotation = false;
            _agent.enabled = false;

            _enemy.forward = _player.forward;

            InitialGhostPlacement();
        }

        private void InitialGhostPlacement()
        {
            Vector3 inFront = _player.position + _player.forward * _struggleLerpDistance;
            inFront.y = _enemy.position.y;
            _enemy.position = inFront;
        }


        public override void Tick(float delta)
        {
            base.Tick(delta);

            UpdatePlacementWithProgress(delta);
        }

        private void UpdatePlacementWithProgress(float delta)
        {
            float progress = Mathf.Clamp01(_minigame.GetProgress());

            float dynamicOffset = Mathf.Lerp(_struggleLerpDistance, _finalOffsetDistance, progress);

            Vector3 targetPos = _player.position + _player.forward * dynamicOffset;
            targetPos.y = _enemy.position.y;

            _enemy.position = Vector3.Lerp(_enemy.position, targetPos, delta * _lerpSpeed);
        }


        public override void Exit()
        {
            _agent.updatePosition = true;
            _agent.updateRotation = true;
        }

        public void ResolveStruggle(bool playerWon)
        {
            if (playerWon)
            {
                _onEnterCaptured?.Invoke();
            }
            else
            {
                _onStruggleFail?.Invoke();
                _agent.updatePosition = true;
                _agent.updateRotation = true;
                _agent.enabled = true;
            }
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