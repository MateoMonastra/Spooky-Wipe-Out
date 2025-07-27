using FSM;
using Debug = UnityEngine.Debug;

namespace Ghosts.WalkingGhost
{
    public class Rest : State
    {
        private GhostRest _ghostRest;

        public Rest(GhostRest rest)
        {
            this._ghostRest = rest;
        }

        public void ChangeRest(bool obj)
        {
            Debug.Log($"se cambia el rest a: {obj}");
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
