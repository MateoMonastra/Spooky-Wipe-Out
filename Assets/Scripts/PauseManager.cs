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
        GameManager.GetInstance().OnFinish += () => inputReader.OnPauseStart -= InitPauseMenu;
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
        Time.timeScale = 1f;
        GameManager.GetInstance().SetPlayerUIState(true);
        pauseMenu.SetActive(false);
        AkSoundEngine.SetState("GameState", "Playing");
    }
    
    public void Restart()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        AkSoundEngine.SetState("GameState", "None");
        AkSoundEngine.SetState("GameState", "Playing");
        eventChannelSceneManager.OnRemoveScene(gameObject.scene.name);
        eventChannelSceneManager.OnAddScene(gameObject.scene.name);
    }

    public void GoMenu()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        eventChannelSceneManager.OnRemoveScene(gameObject.scene.name);
        eventChannelSceneManager.OnAddScene(menuSceneName);
    }
}
