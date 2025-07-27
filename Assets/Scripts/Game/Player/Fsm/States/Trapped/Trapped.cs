using FSM;
using UnityEngine;

namespace Game.Player
{
    public class Trapped : State
    {
        private Transform _trappedPos;
        private GameObject _player;

        public Trapped(GameObject player)
        {
            _player = player;
        }
        public override void Enter()
        {
        }

        public override void Tick(float delta)
        {
            _player.transform.position = _trappedPos.position;
        }

        public override void FixedTick(float delta)
        {
            
        }

        public override void Exit()
        {
            
        }

        public void SetPos(Transform trappedPos)
        {
            _trappedPos = trappedPos;
        }
    }
}
