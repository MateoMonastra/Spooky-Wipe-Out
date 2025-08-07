using UnityEngine;

namespace Game.Player
{
    public class PlayerAnimationEvents : MonoBehaviour
    {
        [SerializeField] private PlayerAgent playerAgent;

        public void NotifyGhostAnimationEnd()
        {
            playerAgent.FinishStruggleAnimation();
        }
        
        public void NotifyPlayerStumbleAnimationEnd()
        {
            playerAgent.FinishStumble();
        }
    }
}
