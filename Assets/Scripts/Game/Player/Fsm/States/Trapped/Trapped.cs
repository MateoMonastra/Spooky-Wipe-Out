using UnityEngine;

namespace Game.Player
{
    public class Trapped : PlayerState
    {
        private Transform _trappedPos;

        public Trapped(GameObject player) : base(player)
        {
        }

        public override void Enter()
        {
        }

        public override void Tick(float delta)
        {
            player.transform.position = _trappedPos.position;
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