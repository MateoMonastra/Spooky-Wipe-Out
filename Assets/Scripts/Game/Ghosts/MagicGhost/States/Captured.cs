using FSM;
using Game.Minigames;
using Minigames;
using UnityEngine;

namespace Game.Ghosts.MagicGhost
{
    public class Captured : State
    {
        private GameObject _model;
        private MagicGhostAgent _agent;
        private Minigame _minigame;
        private Collider _collider;

        public Captured(GameObject model, MagicGhostAgent agent, Collider collider, Minigame minigame)
        {
            _model = model;
            _agent = agent;
            _minigame = minigame;
            _collider =  collider;
        }

        public override void Enter()
        {
            _agent.enabled = false;
            _collider.enabled = false;
            _agent.OnBeingDestroy?.Invoke(_agent);
            _minigame?.StopGame();
        }

        public override void Tick(float delta)
        {
        }

        public override void FixedTick(float delta)
        {
        }

        public override void Exit()
        {
        }
    }
}
