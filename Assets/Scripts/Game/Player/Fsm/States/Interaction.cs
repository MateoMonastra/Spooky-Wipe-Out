using FSM;
using UnityEngine;

namespace Game.Player
{
    public class Interaction : State
    {
        private GameObject _player;
        private Rigidbody _rigidbody;

        public Interaction(GameObject player)
        {
            _player = player;
        }

        public override void Enter()
        {
            _rigidbody = _player.GetComponent<Rigidbody>();
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        public override void Exit()
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        public override void Tick(float delta)
        {
        }
    }
}