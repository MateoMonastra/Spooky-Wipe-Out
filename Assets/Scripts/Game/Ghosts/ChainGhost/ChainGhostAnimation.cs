using UnityEngine;

namespace Game.Ghosts.ChainGhost
{
    public class ChainGhostAnimation : MonoBehaviour
    {
        private static readonly int IsStruggled = Animator.StringToHash("isStruggled");
        private static readonly int IsTired = Animator.StringToHash("isTired");
        private static readonly int IsDead = Animator.StringToHash("isDead");

        [SerializeField] private Animator animator;

        public void SetStruggledAnimation(bool isStruggled)
        {
            animator.SetBool(IsStruggled, isStruggled);
        }

        public void SetTiredAnimation(bool isTired)
        {
            animator.SetBool(IsTired, isTired);
        }

        public void SetDeadAnimation(bool isDead)
        {
            animator.SetBool(IsDead, isDead);
        }
    }
}