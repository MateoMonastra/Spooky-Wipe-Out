using UnityEngine;
using UnityEngine.Serialization;

namespace Audio
{
    public class AudioPlayerStepSwitch : MonoBehaviour
    {
        [SerializeField] GameObject player;
    
        private void Start()
        {
            SetRotationAsCamera();
        }

        private void Update()
        {
            gameObject.transform.position = player.transform.position;
        }

        private void SetRotationAsCamera()
        {
            gameObject.transform.Rotate(0, -45, 0);
        }
    }
}
