using UnityEngine;

namespace Game.Ghosts.ChainGhost
{
    public class ChainGhostAnimation : MonoBehaviour
    {
        private static readonly int Struggled = Animator.StringToHash("Struggled");
        private static readonly int Tired = Animator.StringToHash("Tired");
        private static readonly int Walk = Animator.StringToHash("Walk");
        [SerializeField] private Animator animator;

        public void SetStruggledAnimation()
        {
            animator.SetTrigger(Struggled);
        }
    
        public void SetTiredAnimation()
        {
            animator.SetTrigger(Tired);
        }

        public void SetWalkAnimation()
        {
            animator.SetTrigger(Walk);
        }
    }
}
