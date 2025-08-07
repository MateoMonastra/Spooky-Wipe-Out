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
    
        public void Interact()
        {
            player.gameObject.transform.position = playerPosition.position;
            player.SetFridgeInteractionState();
            animator.SetTrigger(Open);
        }
    }
}
