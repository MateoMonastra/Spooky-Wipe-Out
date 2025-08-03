using UnityEngine;
using VacuumCleaner;

namespace Game.Player
{
    public class Struggle : PlayerState
    {
        private CleanerController _cleanerController;
        private Rigidbody _rigidbody;

        public Struggle(GameObject player, CleanerController cleanerController) : base(player)
        {
            _cleanerController = cleanerController;
        }

        public override void Enter()
        {
            _rigidbody = player.GetComponent<Rigidbody>();

            _rigidbody.constraints = RigidbodyConstraints.FreezePositionX |
                                     RigidbodyConstraints.FreezePositionY |
                                     RigidbodyConstraints.FreezePositionZ |
                                     RigidbodyConstraints.FreezeRotation;
        }

        public override void Tick(float delta)
        {
            _cleanerController.SwitchToTool(1);
        }

        public override void FixedTick(float delta)
        {
        }

        public override void Exit()
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
}