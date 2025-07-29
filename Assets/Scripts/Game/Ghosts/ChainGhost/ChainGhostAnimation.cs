using UnityEngine;

namespace Game.Ghosts.ChainGhost
{
    public class ChainGhostAnimation : MonoBehaviour
    {
        private static readonly int IsBeingCaptured = Animator.StringToHash("IsBeingCaptured");
        private static readonly int IsTired = Animator.StringToHash("IsTired");
        [SerializeField] private Animator animator;

        public void SetVacuumedState(bool state)
        {
            animator.SetBool(IsBeingCaptured, state);
        }
    
        public void SetIsTiredState(bool state)
        {
            animator.SetBool(IsTired, state);
        }
    }
}
