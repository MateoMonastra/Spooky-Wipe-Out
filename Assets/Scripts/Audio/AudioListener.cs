using UnityEngine;

namespace Audio
{
    [CreateAssetMenu (fileName = "AudioListener", menuName = "Audio Listener")]
    public class MyAudioListener : ScriptableObject
    {
        [SerializeField] private AkAudioListener listener;

        public AkAudioListener GetListener()
        {
            return listener;
        }

        public void SetListener(AkAudioListener listener)
        {
            this.listener = listener;
        }
    }
}
