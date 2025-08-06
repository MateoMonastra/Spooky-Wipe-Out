using UnityEngine;

namespace Game.Ghosts.MagicGhost
{
    public class MagicGhostAnimation : MonoBehaviour
    {
        private static readonly int Struggled = Animator.StringToHash("isStruggled");
        private static readonly int Walk = Animator.StringToHash("isWalk");
        private static readonly int IsDropping = Animator.StringToHash("isDropping");
        private static readonly int IsDead = Animator.StringToHash("isDead");

        [SerializeField] private Animator animator;

        public void SetStruggledAnimation(bool isStruggled)
        {
            animator.SetBool(Struggled,isStruggled);
        }

        public void SetDropTrashAnimation(bool isDropping)
        {
            animator.SetBool(IsDropping, isDropping);
        }

        public void SetWalkAnimation(bool isWalking)
        {
            animator.SetBool(Walk, isWalking);
        }

        public void SetDeadAnimation()
        {
            animator.SetBool(IsDead, true);
        }
    }
}
