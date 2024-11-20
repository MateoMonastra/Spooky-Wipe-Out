using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ghosts;
using Unity.VisualScripting;


public class AudioGameManager : MonoBehaviour
{
    private List<Ghost> _ghosts;
    [SerializeField] private int destroyedGhostCount = 0;
    [SerializeField] private AK.Wwise.RTPC destroyedGhostRTPC;
    [SerializeField] private AK.Wwise.Event destroyedGhost;
    [SerializeField] GameObject GameMusic;
    [SerializeField] AK.Wwise.Event StartLevel;


    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return null;

        _ghosts = GameManager.GetInstance().ghosts;

        foreach (Ghost ghost in _ghosts)
        {
            ghost.OnBeingDestroy += GhostDestroyed;
        }
        destroyedGhostRTPC.SetValue(GameMusic, destroyedGhostCount);
        StartLevel.Post(GameMusic);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        AkSoundEngine.StopAll(GameMusic);
    }

    void GhostDestroyed(Ghost ghost)
    {
        destroyedGhostCount++;
        destroyedGhostRTPC.SetValue(GameMusic, destroyedGhostCount);
        destroyedGhost.Post(gameObject);
    }
}
