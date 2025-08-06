using FSM;
using Minigames;
using UnityEngine;

namespace Game.Ghosts.MagicGhost
{
    public class Captured : State
    {
        private GameObject _model;
        private MagicGhostAgent _agent;
        private Minigame _minigame;

        public Captured(GameObject model, MagicGhostAgent agent, Minigame minigame)
        {
            _model = model;
            _agent = agent;
            _minigame = minigame;
        }

        public override void Enter()
        {
            _agent.OnBeingDestroy?.Invoke(_agent);
            _model.SetActive(false);
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
