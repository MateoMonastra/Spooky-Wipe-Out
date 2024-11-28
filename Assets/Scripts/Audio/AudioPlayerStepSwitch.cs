using UnityEngine;
using UnityEngine.Serialization;

namespace Audio
{
    public class AudioPlayerStepSwitch : MonoBehaviour
    {
        [SerializeField] GameObject player;
        [SerializeField] AK.Wwise.Switch stepTile;
        [SerializeField] AK.Wwise.Switch stepWood;


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

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("StepsTile");
            stepTile.SetValue(this.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            stepWood.SetValue(this.gameObject);
        }
    }
}
