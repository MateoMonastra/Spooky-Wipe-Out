using Fsm_Mk2;
using UnityEngine.AI;

namespace Ghosts.WalkingGhost
{
    public class Struggle : State
    {
        private NavMeshAgent _agent;

        public Struggle(NavMeshAgent _agent)
        {
            this._agent = _agent;
        }
        public override void Enter()
        {
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
            _agent.enabled = true;
        }
    }
}
