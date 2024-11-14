using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerMover : MonoBehaviour
{

    [SerializeField] private string placeToMoveName = "Manolo";
    private AkAudioListener listenerToMove;
    private GameObject placeToMove;
    private Transform placeToReturn;

    private void Start()
    {
        StartCoroutine(DelayInitialization());
    }

    private IEnumerator DelayInitialization()
    {
        yield return new WaitForEndOfFrame();

        placeToMove = GameObject.Find(placeToMoveName);

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