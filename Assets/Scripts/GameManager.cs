using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Timer;
using UnityEngine;
using Ghosts;
using UnityEngine.SceneManagement;
using System.Collections;
using EventSystems.EventSceneManager;
using Minigames;
using Fsm_Mk2;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public Action OnFinish;
    //[SerializeField] private PlayerAgent playerAgent;

    //[SerializeField] private SkillCheckController SKMinigame;
    //[SerializeField] private ADController ADMinigame;

    [SerializeField] private Timer timer;
    [SerializeField] private ObjectivesUI objectivesUI;
    [SerializeField] private GameObject playerUI;
    public List<Ghost> ghosts;
    public List<Trash> garbage;
    public List<Ectoplasm> ectoplasms;
    [SerializeField] private string nextScene;
    [SerializeField] private EventChannelSceneManager eventChannelSceneManager;

    private static GameManager _instance;

    IEnumerator Start()
    {
        yield return null;

        foreach (Ghost ghost in ghosts)
        {
            ghost.OnBeingDestroy += RemoveGhost;
        }

        foreach (Trash trash in garbage)
        {
            trash.OnBeingDestroy += RemoveTrash;
        }
        
        foreach (Ectoplasm ectoplasm in ectoplasms)
        {
            ectoplasm.OnBeingDestroy += RemoveEctoplasm;
        }

        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            // Si ya existe una instancia y no es esta, se destruye el objeto duplicado
            Destroy(gameObject);
        }

        objectivesUI.SetTrashQnty(garbage.Count);
        objectivesUI.SetGhostQnty(ghosts.Count);
        objectivesUI.SetEctoplasmQnty(ectoplasms.Count);
        
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    private void RemoveTrash(Trash trash)
    {
        trash.OnBeingDestroy -= RemoveTrash;
        garbage.Remove(trash);
        objectivesUI.SetTrashQnty(garbage.Count);
        Debug.Log("The trash has been destroyed");

        GameIsOver();
    }

    private void RemoveGhost(Ghost ghost)
    {
        ghost.OnBeingDestroy -= RemoveGhost;
        ghosts.Remove(ghost);
        objectivesUI.SetGhostQnty(ghosts.Count);
        Debug.Log("The ghost has been destroyed");

        GameIsOver();
    }
    
    private void RemoveEctoplasm(Ectoplasm ectoplasm)
    {
        ectoplasm.OnBeingDestroy -= RemoveEctoplasm;
        ectoplasms.Remove(ectoplasm);
        objectivesUI.SetEctoplasmQnty(ectoplasms.Count);
        Debug.Log("The ectoplasm has been destroyed");

        GameIsOver();
    }

    public static GameManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<GameManager>();
        }

        return _instance;
    }

    public bool IsAnyGhost()
    {
        return ghosts.Any(ghost => ghost.isActiveAndEnabled);
    }

    public bool IsAnyGarbage()
    {
        return garbage.Any(trash => trash.isActiveAndEnabled);
    }
    
    public bool IsAnyEctoplasm()
    {
        return ectoplasms.Any(ectoplasm => ectoplasm.isActiveAndEnabled);
    }

    public float GetTimeLeft()
    {
        return timer.GetTime();
    }

    public void SetPlayerUIState(bool state)
    {
        playerUI.SetActive(state);
    }

    private void GameIsOver()
    {
        if (!IsAnyGhost() && !IsAnyGarbage() && !IsAnyEctoplasm())
        {
            FinishGame();
        }
    }

    private void FinishGame()
    {
        SetPlayerUIState(false);
        OnFinish?.Invoke();
    }
}