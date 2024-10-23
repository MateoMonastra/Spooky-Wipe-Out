using Fsm_Mk2;
using Minigames;
using UnityEngine;

namespace Ghosts.WalkingGhost
{
    public class Capture : State
    {
        private GameObject _model;
        private WalkingGhostAgent _agent;
        private Minigame _minigame;

        public Capture(GameObject model, WalkingGhostAgent agent, Minigame minigame)
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
            _minigame.StopGame();
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