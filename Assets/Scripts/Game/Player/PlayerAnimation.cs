using UnityEngine;

namespace Game.Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private static readonly int IsCleaning = Animator.StringToHash("IsCleaning");
        private static readonly int IsStumbled = Animator.StringToHash("IsStumbled");
        private static readonly int IsOpeningDoor = Animator.StringToHash("IsOpeningDoor");

        [SerializeField] private Animator animator;
    
        public void SetWalkState(bool state)
        {
            animator.SetBool(IsWalking, state);
        }
    
        public void SetCleaning(bool state)
        {
            animator.SetBool(IsCleaning, state);
        }

        public void SetStumble(bool state)
        {
            animator.SetBool(IsStumbled, state);
        }

        public void SetOpenDoor(bool state)
        {
            animator.SetBool(IsOpeningDoor, state);
        }
    }
}