using Game.Ghosts.WalkingGhost;
using Minigames;
using UnityEngine;

namespace Game.Ghosts.ChainGhost
{
    public class Captured : FSM.State
    {
        private GameObject _model;
        private ChainGhostAgent _agent;
        private Minigame _minigame;

        public Captured(GameObject model, ChainGhostAgent agent, Minigame minigame)
        {
            _model = model;
            _agent = agent;
            _minigame = minigame;
        }

        public override void Enter()
        {
            _agent.OnBeingDestroy?.Invoke(_agent);
            _agent.gameObject.SetActive(false);
            _agent.enabled = false;
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