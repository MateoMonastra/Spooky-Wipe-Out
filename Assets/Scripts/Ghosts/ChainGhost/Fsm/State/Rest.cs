using System;
using Fsm_Mk2;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.AI;

namespace Ghosts.WalkingGhost
{
    public class Rest : State
    {
        private GhostRest _ghostRest;

        public Rest(GhostRest rest, Action<bool> changeRestState)
        {
            this._ghostRest = rest;
            changeRestState = ChangeRest;
            }

        private void ChangeRest(bool obj)
        {
            _ghostRest.isRested = obj;
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
