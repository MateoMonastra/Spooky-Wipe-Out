using System;
using FSM;
using Player.FSM;
using UnityEngine;

namespace Game.Player
{
    public class WalkIdle : PlayerState
    {
        private Action<bool> _OnWalk;
        private Rigidbody _rigidbody;
        private WalkIdleModel _model;
        private LayerMask _layerRaycast;

        private Vector3 _dir = Vector3.zero;
        private bool _isClickPressed;

        private Vector3 mousePosition;

        private Vector3 _counterMovement;

        private float _stickRotatingSpeed = 5.0f;

        public WalkIdle(GameObject player, WalkIdleModel model, LayerMask layerRaycast, Action<bool> OnWalk) : base(player)
        {
            _model = model;
            _layerRaycast = layerRaycast;
            _OnWalk = OnWalk;
        }

        public override void Enter()
        {
            _rigidbody = player.GetComponent<Rigidbody>();
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
                _OnWalk?.Invoke(_dir.normalized != Vector3.zero);

                _rigidbody.AddForce(_dir.normalized * _model.MovementForce + _counterMovement);

                float angle = Vector3.SignedAngle(player.transform.forward, _dir, player.transform.up);

                if (!_isClickPressed)
                {
                    _counterMovement = new Vector3(-_rigidbody.velocity.x * _model.CounterMovementForce, 0,
                        -_rigidbody.velocity.z * _model.CounterMovementForce);

                    RotateByMovementInput(angle);
                }
                else
                {
                    _counterMovement = new Vector3(-_rigidbody.velocity.x * _model.CounterMovementForceVacuuming, 0,
                        -_rigidbody.velocity.z * _model.CounterMovementForceVacuuming);
                    if (InputReader.isUsingController)
                    {
                        RotateWhileVacuumingStick(angle);
                    }
                    else
                    {
                        RotateWhileVacuuming();
                    }
                }
            }
        }

        private void RotateByMovementInput(float angle)
        {
            player.transform.Rotate(player.transform.up, angle * Time.deltaTime * _model.RotationSpeed);
        }

        private void RotateWhileVacuuming()
        {
            if (!Camera.main) return;

            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layerRaycast))
            {
                Vector3 targetPosition = new Vector3(hit.point.x, player.transform.position.y, hit.point.z);

                Vector3 direction = targetPosition - player.transform.position;
                Quaternion actualRotation = player.transform.rotation;
                float rotationAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f, rotationAngle, 0f);

                player.transform.rotation = targetRotation;
            }
        }

        private void RotateWhileVacuumingStick(float angle)
        {
            if (!Camera.main) return;
            if (mousePosition == Vector3.zero) return;

            var cameraTransform = Camera.main.transform;
            var cameraBasedRotateDirection = cameraTransform.TransformDirection(mousePosition);

            Quaternion targetRotation = Quaternion.LookRotation(cameraBasedRotateDirection.IgnoreY());
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation,
                _stickRotatingSpeed * Time.deltaTime);
        }

        public void SetDir(Vector3 newDir)
        {
            _dir = newDir;
        }

        public void SetIsClickPressedState(bool isClickPresed)
        {
            _isClickPressed = isClickPresed;
        }

        public void SetMousePosition(Vector3 mousePosition)
        {
            this.mousePosition = mousePosition;
        }
    }

    public static class Vector3Extensions
    {
        public static Vector3 IgnoreY(this Vector3 self)
        {
            var result = new Vector3(self.x, 0, self.z);
            return result;
        }
    }
}