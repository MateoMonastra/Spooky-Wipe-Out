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

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return null;

        _ghosts = GameManager.GetInstance().ghosts;

        foreach (Ghost ghost in _ghosts)
        {
            ghost.OnBeingDestroy += GhostDestroyed;
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GhostDestroyed(Ghost ghost)
    {
        destroyedGhostCount++;
        destroyedGhostRTPC.SetValue(gameObject, destroyedGhostCount);
        destroyedGhost.Post(gameObject);
    }
}
