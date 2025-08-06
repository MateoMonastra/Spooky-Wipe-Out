using EventSystems.EventSceneManager;
using Game;
using Player.FSM;
using UnityEngine;
using UnityEngine.Events;

public class PauseManager : MonoBehaviour
{
    public UnityEvent onPauseStarted;
    public UnityEvent onPauseResume;
    public UnityEvent onPauseRestart;
    public UnityEvent onPauseGoMenu;
    
    [SerializeField] private InputReader inputReader;
    
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private string menuSceneName;
    
    [SerializeField] private EventChannelSceneManager eventChannelSceneManager;

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
        onPauseStarted?.Invoke();
    }
    
    public void Resume()
    {
        Time.timeScale = 1f;
        onPauseResume?.Invoke();
        GameManager.GetInstance().SetPlayerUIState(true);
        pauseMenu.SetActive(false);
    }
    
    public void Restart()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        onPauseRestart?.Invoke();
        eventChannelSceneManager.OnRemoveScene(gameObject.scene.name);
        eventChannelSceneManager.OnAddScene(gameObject.scene.name);
    }

    public void GoMenu()
    {
        Time.timeScale = 1f;
        onPauseGoMenu?.Invoke();
        pauseMenu.SetActive(false);
        eventChannelSceneManager.OnRemoveScene(gameObject.scene.name);
        eventChannelSceneManager.OnAddScene(menuSceneName);
    }
}
