using UnityEngine;

namespace Game.Player
{
    public class PlayerAnimationEvents : MonoBehaviour
    {
        [SerializeField] private PlayerAgent playerAgent;

        public void NotifyGhostAnimationEnd()
        {
            Debug.LogWarning("GhostAnimationEnd");
            playerAgent.FinishStruggle();
        }
    }
}
