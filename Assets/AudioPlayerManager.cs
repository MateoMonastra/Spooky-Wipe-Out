using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VacuumCleaner.Modes;

public class AudioPlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private Vacuum _vacuum;
    [SerializeField] private WashFloor _washFloor;


    // Start is called before the first frame update
    void Start()
    {
        _vacuum.OnPowerOn += setSwitchVacWorking;
        _vacuum.OnPowerOff += setSwitchVacuumStop;
        _washFloor.OnPowerOn += setSwitchHydroWorking;
        _washFloor.OnPowerOff += setSwitchVacuumStop;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void setSwitchVacWorking()
    {
        AkSoundEngine.SetSwitch("Vacuum", "VacWorking", gameObject);
    }
    private void setSwitchVacuumStop()
    {
        AkSoundEngine.SetSwitch("Vacuum", "Stop", gameObject);
    }
    private void setSwitchHydroWorking()
    {
        AkSoundEngine.SetSwitch("Vacuum", "HydroWorking", gameObject);
    }
}
