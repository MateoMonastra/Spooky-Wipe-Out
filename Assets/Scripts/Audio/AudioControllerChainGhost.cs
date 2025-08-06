using UnityEngine;
using Event = AK.Wwise.Event;

namespace Audio
{
    public class AudioControllerChainGhost : MonoBehaviour
    {
        [SerializeField] private Event chainGhostOnFlee;
        [SerializeField] private Event destroyedGhost;

        public void Flee()
        {
            if (AkSoundEngine.IsInitialized() && gameObject.activeInHierarchy)
            {
                chainGhostOnFlee.Post(this.gameObject);
            }
        }

        public void DestroyGhost()
        {
            if (AkSoundEngine.IsInitialized() && gameObject.activeInHierarchy)
            {
                destroyedGhost.Post(this.gameObject);
            }
        }
    }
}