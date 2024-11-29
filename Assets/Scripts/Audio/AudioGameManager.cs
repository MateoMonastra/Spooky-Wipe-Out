using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ghosts;
using Unity.VisualScripting;
using UnityEngine.Serialization;


public class AudioGameManager : MonoBehaviour
{
    [SerializeField] private int destroyedGhostCount = 0;
    [SerializeField] private AK.Wwise.RTPC destroyedGhostRtpc;
    [SerializeField] private AK.Wwise.Event destroyedGhost;
    [SerializeField] private GameObject gameMusic;
    [SerializeField] private AK.Wwise.Event startLevel;
    [SerializeField] private AK.Wwise.Event endLevel;

    private List<Ghost> _ghosts;


    // Start is called before the first frame update
    private IEnumerator Start()
    {
        yield return null;

        GameManager gameManager = GameManager.GetInstance();

        gameManager.OnFinish += Finish;

        _ghosts = gameManager.ghosts;

        foreach (Ghost ghost in _ghosts)
        {
            ghost.OnBeingDestroy += GhostDestroyed;
        }
        
        destroyedGhostRtpc.SetValue(gameMusic, destroyedGhostCount);
        startLevel.Post(gameMusic);
    }

    private void OnDestroy()
    {
        AkSoundEngine.StopAll(gameMusic);
    }

    void GhostDestroyed(Ghost ghost)
    {
        destroyedGhostCount++;
        destroyedGhostRtpc.SetValue(gameMusic, destroyedGhostCount);
        destroyedGhost.Post(gameObject);
    }

    void Finish()
    {
        endLevel.Post(gameMusic);
    }
}
