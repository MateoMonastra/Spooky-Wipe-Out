using UnityEngine;

namespace Game.Ghosts.PaintGhost
{
    public class PaintGhostAnimation : MonoBehaviour
    {
        private static readonly int IsDeath = Animator.StringToHash("IsDeath");
        private static readonly int Catched = Animator.StringToHash("Catched");
    
        [SerializeField] private Animator animator;
    
        public void SetIsDeathState(bool state)
        {
            animator.SetBool(IsDeath, state);
        }
    
        public void SetGrabTrigger()
        {
            animator.SetTrigger(Catched);
        }
    }
}
