using Fsm_Mk2;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.AI;

namespace Ghosts.WalkingGhost
{
    public class Rest : State
    {
        private GhostRest _ghostRest;

        public Rest(GhostRest rest)
        {
            this._ghostRest = rest;
        }

        public override void Enter()
        {
            Debug.Log("Entro al estado rest");
            _ghostRest.enabled = true;
        }

        public override void Tick(float delta)
        {

        }

        public override void FixedTick(float delta)
        {

        }

        public override void Exit()
        {
            _ghostRest.enabled = false;
        }
    }
}
