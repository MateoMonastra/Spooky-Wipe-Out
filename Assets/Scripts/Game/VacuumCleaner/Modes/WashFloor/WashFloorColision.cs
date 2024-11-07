using UnityEngine;

namespace VacuumCleaner.Modes
{
    public class WashFloorColision : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private WashFloorModel model;

        private Ray _ray;

        private Vector3? _collision;
        private Quaternion _left;
        private Quaternion _right;

        private Vector3 _leftBoundary;
        private Vector3 _rightBoundary;


        private void OnTriggerStay(Collider other)
        {
            if (IsWasheable(other))
            {
                if (CanWash(other))
                {
                    other.GetComponentInParent<IWasheable>().IsBeingWashed(model.WashSpeed, model.MinScale);
                }
            }
        }

        private static bool IsWasheable(Collider other)
        {
            IWasheable washeable = other.GetComponentInParent<IWasheable>();

            return washeable != null;
        }

        private bool CanWash(Collider other)
        {
            _ray = new Ray(target.position, other.transform.position - target.position);

            if (Physics.Raycast(_ray, out var hit, model.RenderDistance, model.WallLayer))
            {
                return false;
            }

            return true;
        }
    }
}
