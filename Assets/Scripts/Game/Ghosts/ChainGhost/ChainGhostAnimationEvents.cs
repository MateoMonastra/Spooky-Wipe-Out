using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Ghosts.ChainGhost
{
    public class ChainGhostAnimationEvents : MonoBehaviour
    {
        [SerializeField] private ChainGhostAgent chainGhostAgent;
        
        public void NotifyDeathAnimationScaling()
        {
            if (chainGhostAgent != null)
            {
                chainGhostAgent.OnDeathAnimationScaling();
            }
        }
    }
}
