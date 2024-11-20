using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerMover : MonoBehaviour
{

    private AkAudioListener listenerToMove;
    public string placeToMoveTag;
    private GameObject placeToMove;
    private Transform placeToReturn;

    // Start is called before the first frame update
    void Start()
    {
        
        StartCoroutine(DelayInitialization());
    }

    private void OnDestroy()
    {
        ReturnListener();
    }

    IEnumerator DelayInitialization()
    {
        // Wait until the end of the frame
        yield return new WaitForEndOfFrame();

        placeToMove = GameObject.FindGameObjectWithTag(placeToMoveTag);

        listenerToMove = FindObjectOfType<AkAudioListener>();
        placeToReturn = listenerToMove.transform.parent;

        if (listenerToMove != null && placeToMove != null)
        {
            listenerToMove.transform.SetParent(placeToMove.transform, false);
            
        }


    }

    public void ReturnListener()
    {
        if (placeToReturn != null)
        {
            listenerToMove.transform.SetParent(placeToReturn, false);
        }
    }
}