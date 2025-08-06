using UnityEngine;

namespace Game.Ghosts.ChainGhost
{
    public class ChainGhostAnimation : MonoBehaviour
    {
        private static readonly int IsStruggled = Animator.StringToHash("isStruggled");
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int IsTired = Animator.StringToHash("isTired");
        
        [SerializeField] private Animator animator;

        public void SetStruggledAnimation(bool isStruggled)
        {
            animator.SetBool(IsStruggled,isStruggled);
        }

        public void SetTiredAnimation(bool isTired)
        {
            animator.SetBool(IsTired, isTired);
        }

        public void SetWalkAnimation()
        {
            animator.SetTrigger(Walk);
        }
    }
}