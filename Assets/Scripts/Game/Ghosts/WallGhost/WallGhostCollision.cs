using System;
using Fsm_Mk2;
using UnityEngine;

namespace Game.Ghosts.WallGhost
{
    public class WallGhostCollision : MonoBehaviour
    {
        [SerializeField] private Transform trappingPos;

        public Action OnPlayerCollision;
        private GameObject _player;
        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsPlayerCollision(other))
            {
                OnPlayerCollision?.Invoke();
                SetPlayerAction(other);
            }
        }

        private static bool IsPlayerCollision(Collider other)
        {
            return other.gameObject.layer == LayerMask.NameToLayer($"Player");
        }

        private void SetPlayerAction(Collider other)
        {
            _player = other.gameObject;
            _player.GetComponentInParent<PlayerAgent>().OnHunted.Invoke(trappingPos);
        }
        
        public void SetActiveCollision(bool active)
        {
            if (_collider != null)
                _collider.enabled = active;
        }
    }
}