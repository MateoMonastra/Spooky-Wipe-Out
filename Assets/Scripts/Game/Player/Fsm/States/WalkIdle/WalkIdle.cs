using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace Fsm_Mk2
{
    public class WalkIdle : State
    {
        private GameObject _gameObject;
        private Rigidbody _rigidbody;
        private WalkIdleModel _model;
        private LayerMask _layerRaycast;

        private Vector3 _dir = Vector3.zero;
        private bool _isClickPressed;

        private Vector3 mousePosition;

        private Vector3 _counterMovement;

        public WalkIdle(GameObject gameObject, WalkIdleModel model, LayerMask layerRaycast)
        {
            _gameObject = gameObject;
            _model = model;
            _layerRaycast = layerRaycast;
        }

        public override void Enter()
        {
            _rigidbody = _gameObject.GetComponent<Rigidbody>();
        }

        public override void Tick(float delta)
        {
        }

        public override void FixedTick(float delta)
        {
            Move();
        }

        public override void Exit()
        {
            _dir = Vector3.zero;
        }

        private void Move()
        {
            if (_rigidbody)
            {
                _rigidbody.AddForce(_dir.normalized * _model.MovementForce + _counterMovement);

                float angle = Vector3.SignedAngle(_gameObject.transform.forward, _dir, _gameObject.transform.up);

                if (!_isClickPressed)
                {
                    _counterMovement = new Vector3(-_rigidbody.velocity.x * _model.CounterMovementForce, 0, -_rigidbody.velocity.z * _model.CounterMovementForce);

                    RotateByMovementInput(angle);
                }
                else
                {
                    _counterMovement = new Vector3(-_rigidbody.velocity.x * _model.CounterMovementForceVacuuming, 0, -_rigidbody.velocity.z * _model.CounterMovementForceVacuuming);

                    RotateWhileVacuuming();
                }
            }
        }

        private void RotateByMovementInput(float angle)
        {
            _gameObject.transform.Rotate(_gameObject.transform.up, angle * Time.deltaTime * _model.RotationSpeed);
        }

        private void RotateWhileVacuuming()
        {
            if (!Camera.main) return;

            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layerRaycast))
            {
                Vector3 targetPosition = new Vector3(hit.point.x, _gameObject.transform.position.y, hit.point.z);

                Vector3 direction = targetPosition - _gameObject.transform.position;
                Quaternion actualRotation = _gameObject.transform.rotation;
                float rotationAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f, rotationAngle, 0f);

                _gameObject.transform.rotation = targetRotation;
            }
        }

        public void SetDir(Vector3 newDir)
        {
            _dir = newDir;
        }

        public void SetIsClickPressedState(bool isClickPresed)
        {
            _isClickPressed = isClickPresed;
            Debug.Log($"The button is : {isClickPresed}");
        }

        public void SetMousePosition(Vector3 mousePosition)
        {
            this.mousePosition = mousePosition;
        }
    }
}