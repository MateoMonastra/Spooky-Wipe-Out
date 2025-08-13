using Audio;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PauseMenuMusicController : MonoBehaviour
{
    
    [SerializeField] private PauseManager pauseManager;
    [SerializeField] private AudioListenerMover audioListenerMover;

    void Start()
    {
        pauseManager.onPauseStarted.AddListener(Pause);
        pauseManager.onPauseResume.AddListener(Resume);
        pauseManager.onPauseRestart.AddListener(PauseRestart);
        pauseManager.onPauseGoMenu.AddListener(GoMenu);

    }

    private void Pause()
    {
        AkSoundEngine.SetState("GameState", "Pause");
    }

    private void Resume()
    {
        AkSoundEngine.SetState("GameState", "Playing");

    }

    private void PauseRestart()
    {
        audioListenerMover.ReturnListener();
        AkSoundEngine.SetState("GameState", "None");
        AkSoundEngine.SetState("GameState", "Playing");
    }

    private void GoMenu()
    {
        audioListenerMover.ReturnListener();
    }

}


