using FSM;
using UnityEngine;
using VacuumCleaner;

namespace Game.Player
{
    public class Struggle : State
    {
        private GameObject _model;
        private CleanerController _cleanerController;
        private Rigidbody _rigidbody;

        public Struggle(GameObject model, CleanerController cleanerController)
        {
            _model = model;
            _cleanerController = cleanerController;
        }

        public override void Enter()
        {
            _rigidbody = _model.GetComponent<Rigidbody>();
        
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