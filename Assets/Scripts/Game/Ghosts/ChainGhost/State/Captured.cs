using FSM;
using Game.Minigames;
using Minigames;
using UnityEngine;

namespace Game.Ghosts.ChainGhost
{
    public class Captured : State
    {
        private GameObject _model;
        private ChainGhostAgent _agent;
        private Minigame _minigame;
        private Collider _collider;


        public Captured(GameObject model, ChainGhostAgent agent, Collider collider, Minigame minigame)
        {
            _model = model;
            _agent = agent;
            _minigame = minigame;
            _collider = collider;
        }

        public override void Enter()
        {
            _collider.enabled = false;
            _minigame?.StopGame();
            _agent.OnBeingDestroy?.Invoke(_agent);
            _agent.enabled = false;
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