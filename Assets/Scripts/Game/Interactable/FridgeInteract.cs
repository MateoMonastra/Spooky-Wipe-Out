using Game.Player;
using UnityEngine;

namespace Game.Interactable
{
    public class FridgeInteract : MonoBehaviour, IInteractable
    {
        private static readonly int Open = Animator.StringToHash("Open");
        [SerializeField] private Animator animator;
        [SerializeField] private Transform playerPosition;
        [SerializeField] private PlayerAgent player;
        
        private bool _isActive = false;
        private bool _alredyUsed = false;
        public void Interact()
        {
            if (_alredyUsed) return;
            _isActive = true;
            _alredyUsed = true;
            player.SetFridgeInteractionState();
            animator.SetTrigger(Open);
            GetComponent<BoxCollider>().enabled = false;
        }

        public void Update()
        {
            if (!_isActive) return;
            player.gameObject.transform.position = playerPosition.position;
            player.gameObject.transform.rotation = playerPosition.rotation;
        }

        public void Reset()
        {
            _isActive = false;
        }
    }
}
