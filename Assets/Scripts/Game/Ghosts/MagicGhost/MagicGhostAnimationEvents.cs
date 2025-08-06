using UnityEngine;

namespace Game.Ghosts.MagicGhost
{
    public class MagicGhostAnimationEvents : MonoBehaviour
    {
        [SerializeField] private MagicGhostAgent magicGhostAgent;
        public void NotifyDeathAnimationEnd()
        {
            if (magicGhostAgent != null)
            {
                magicGhostAgent.OnDeathAnimationEnd();
            }
        }
    }
}
