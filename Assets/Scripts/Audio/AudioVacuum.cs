using UnityEngine;
using VacuumCleaner.Modes;

namespace Audio
{
    public class AudioVacuum : MonoBehaviour
    {
        [SerializeField] private Vacuum vacuum;
        [SerializeField] private WashFloor washFloor;

        [SerializeField] private AK.Wwise.Event audioVacuumEvent;
        
        private void OnEnable()
        {
            vacuum.OnPowerOn += SetSwitchVacWorking;
            vacuum.OnPowerOff += SetSwitchVacuumStop;
            washFloor.OnPowerOn += SetSwitchHydroWorking;
            washFloor.OnPowerOff += SetSwitchVacuumStop;
        }

        private void OnDisable()
        {
            vacuum.OnPowerOn -= SetSwitchVacWorking;
            vacuum.OnPowerOff -= SetSwitchVacuumStop;
            washFloor.OnPowerOn -= SetSwitchHydroWorking;
            washFloor.OnPowerOff -= SetSwitchVacuumStop;
        }
        
        private void SetSwitchVacWorking()
        {
            AkSoundEngine.SetSwitch("Vacuum", "VacWorking", gameObject);
        }
        private void SetSwitchVacuumStop()
        {
            AkSoundEngine.SetSwitch("Vacuum", "Stop", gameObject);
        }
        private void SetSwitchHydroWorking()
        {
            AkSoundEngine.SetSwitch("Vacuum", "HydroWorking", gameObject);
        }

        public void PlayOnTrashCollected()
        {
            audioVacuumEvent.Post(gameObject);
        }
    }
}
