using UnityEngine;

namespace Game.Player
{
    [CreateAssetMenu(fileName = "WalkIdleModel", menuName = "Player/FSM/States/WalkIdleModel")]
    public class WalkIdleModel : ScriptableObject
    {
        [SerializeField] private float movementForce = 30;
        [SerializeField] private float counterMovementForce = 5;
        [SerializeField] private float counterMovementForceVacuuming = 10;
        [SerializeField] private float rotationSpeed = 5;
        [SerializeField] private  float maxSpeed = 6f;
        [SerializeField] private float movementForceVacuuming = 15f;

        [SerializeField] private LayerMask layerRaycast;
        
        public float MovementForce
        {
            get => movementForce;
            set => movementForce = value;
        }

        public float CounterMovementForce
        {
            get => counterMovementForce;
            set => counterMovementForce = value;
        }

        public float CounterMovementForceVacuuming
        {
            get => counterMovementForceVacuuming;
            set => counterMovementForceVacuuming = value;
        }

        public float RotationSpeed
        {
            get => rotationSpeed;
            set => rotationSpeed = value;
        }

        public LayerMask LayerRaycast
        {
            get => layerRaycast;
            set => layerRaycast = value;
        }
        public float MaxSpeed
        {
            get => maxSpeed;
            set => maxSpeed = value;
        }
        public float MovementForceVacuuming
        {
            get => movementForceVacuuming;
            set => movementForceVacuuming = value;
        }
    }
}
