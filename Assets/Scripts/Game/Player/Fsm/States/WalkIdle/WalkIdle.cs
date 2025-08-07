using System;
using FSM;
using Player.FSM;
using UnityEngine;

namespace Game.Player
{
    public class WalkIdle : PlayerState
    {
        private readonly Action<bool> _onWalk;
        private readonly WalkIdleModel _model;
        private readonly LayerMask _layerRaycast;

        private Rigidbody _rigidbody;
        private Vector3 _dir = Vector3.zero;
        private Vector3 _counterMovement = Vector3.zero;
        private Vector3 _mousePosition = Vector3.zero;
        
        private float _stickRotatingSpeed = 5.0f;
        private Camera _camera;

        public WalkIdle(GameObject player, WalkIdleModel model, LayerMask layerRaycast, Action<bool> onWalk)
            : base(player)
        {
            _model = model;
            _layerRaycast = layerRaycast;
            _onWalk = onWalk;
            _camera = Camera.main;
        }

        public override void Enter()
        {
            _rigidbody = player.GetComponent<Rigidbody>();
            
        }

        public override void FixedTick(float delta)
        {
            Move(delta);
        }

        public override void Exit()
        {
            _dir = Vector3.zero;
            _counterMovement = Vector3.zero;
        }

        private void Move(float delta)
        {
            if (_rigidbody == null) return;

            bool isVacuuming = InputReader.isClickPressed;
            bool isUsingStick = InputReader.isUsingController;
            float inputMagnitude = Mathf.Clamp01(_dir.magnitude);
            
            _onWalk?.Invoke(inputMagnitude > 0.01f);
            
            float movementForce = isVacuuming ? _model.MovementForceVacuuming : _model.MovementForce;
            Vector3 moveForce = _dir * movementForce;
            
            CounterMovement(isVacuuming, inputMagnitude);

            _rigidbody.AddForce(moveForce + _counterMovement, ForceMode.Force);
            
            float angle = Vector3.SignedAngle(player.transform.forward, _dir, Vector3.up);
            if (!isVacuuming)
            {
                RotateByMovementInput(angle, delta);
            }
            else
            {
                if (isUsingStick)
                    RotateWhileVacuumingStick(angle, delta);
                else
                    RotateWhileVacuuming();
            }
            
            LimitVelocity();
        }

        private void CounterMovement(bool isVacuuming, float inputMagnitude)
        {
            float counterForce = isVacuuming ? _model.CounterMovementForceVacuuming : _model.CounterMovementForce;

            if (inputMagnitude < 0.01f)
            {
                _counterMovement = new Vector3(
                    -_rigidbody.velocity.x * counterForce * 1.5f,
                    0f,
                    -_rigidbody.velocity.z * counterForce * 1.5f
                );
            }
            else
            {
                _counterMovement = new Vector3(
                    -_rigidbody.velocity.x * counterForce * inputMagnitude,
                    0f,
                    -_rigidbody.velocity.z * counterForce * inputMagnitude
                );
            }
        }

        private void LimitVelocity()
        {
            Vector3 horizontalVelocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
            if (horizontalVelocity.magnitude > _model.MaxSpeed)
            {
                Vector3 limitedVelocity = horizontalVelocity.normalized * _model.MaxSpeed;
                _rigidbody.velocity = new Vector3(limitedVelocity.x, _rigidbody.velocity.y, limitedVelocity.z);
            }
        }


        private void RotateByMovementInput(float angle, float delta)
        {
            player.transform.Rotate(Vector3.up, angle * delta * _model.RotationSpeed);
        }

        private void RotateWhileVacuuming()
        {
            if (!_camera) return;
            
            if (Camera.main != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(_mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerRaycast))
                {
                    Vector3 target = new Vector3(hit.point.x, player.transform.position.y, hit.point.z);
                    Vector3 direction = target - player.transform.position;
                    float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                    player.transform.rotation = Quaternion.Euler(0f, angle, 0f);
                }
            }
        }

        private void RotateWhileVacuumingStick(float angle, float delta)
        {
            if (!Camera.main || _mousePosition == Vector3.zero) return;

            var cameraTransform = Camera.main.transform;
            
            var rawDirection = cameraTransform.TransformDirection(_mousePosition);
            rawDirection.y = 0f;

            if (rawDirection.sqrMagnitude < 0.01f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(rawDirection);
            player.transform.rotation = Quaternion.Slerp(
                player.transform.rotation,
                targetRotation,
                _stickRotatingSpeed * delta
            );
        }


        public void SetDir(Vector3 dir) => _dir = dir;
        public void SetMousePosition(Vector3 pos) => _mousePosition = pos;
    }

    public static class Vector3Extensions
    {
        public static Vector3 IgnoreY(this Vector3 vector) => new Vector3(vector.x, 0f, vector.z);
    }
}
