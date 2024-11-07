using Fsm_Mk2;
using Gameplay.GhostMechanics;
using System;
using UnityEngine;

namespace Ghosts.WalkingGhost
{
    public class Walk : State
    {
        private GhostPatrolling _ghostPatrolling;

        public Walk(GhostPatrolling patrolling)
        {
            this._ghostPatrolling = patrolling;
        }

        public override void Enter()
        {
            //Debug.Log ("enter walk state");
            _ghostPatrolling.enabled = true;
        }

        public override void Tick(float delta)
        {
            //Debug.Log("im walking");
        }

        public override void FixedTick(float delta)
        {

        }

        public override void Exit()
        {
            _ghostPatrolling.enabled = false;
        }
    }
}
