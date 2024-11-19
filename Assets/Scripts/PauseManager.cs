using System.Collections;
using System.Collections.Generic;
using EventSystems.EventSceneManager;
using Player.FSM;
using UnityEngine;
using UnityEngine.Events;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private string menuSceneName;
    
    [SerializeField] private EventChannelSceneManager eventChannelSceneManager;
    [SerializeField] private UnityEvent OnPauseStarted;

    private void Start()
    {
        inputReader.OnPauseStart += InitPauseMenu;
    }

    private void InitPauseMenu()
    {
        GameManager.GetInstance().SetPlayerUIState(false);
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        AkSoundEngine.SetState("GameState", "Pause");
        OnPauseStarted?.Invoke();
    }
    
    public void Resume()
    {
        GameManager.GetInstance().SetPlayerUIState(true);
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        AkSoundEngine.SetState("GameState", "Playing");
    }
    
    public void Restart()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        AkSoundEngine.SetState("GameState", "None");
        AkSoundEngine.SetState("GameState", "Playing");
        eventChannelSceneManager.OnRemoveScene(gameObject.scene.name);
        eventChannelSceneManager.OnAddScene(gameObject.scene.name);
    }

    public void GoMenu()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        eventChannelSceneManager.OnAddScene(menuSceneName);
        eventChannelSceneManager.OnRemoveScene(gameObject.scene.name);
    }
}
