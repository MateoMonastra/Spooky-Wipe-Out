using Fsm_Mk2;
using System;
using UnityEngine;

namespace Ghosts.WallGhost
{
    public class Dead : State
    {
        private Action _OnRested;
        private float time;
        private float restTime;

        public Dead(float restTime, Action OnRested)
        {
            this.restTime = restTime;
            _OnRested = OnRested;
        }
        public override void Enter()
        {
            time = 0;
        }

        public override void Tick(float delta)
        {
            time += Time.deltaTime;

            if (time>= restTime)
            {
                _OnRested.Invoke();
            }
        }

        public override void FixedTick(float delta)
        {
            
        }

        public override void Exit()
        {
        }
    }
}
