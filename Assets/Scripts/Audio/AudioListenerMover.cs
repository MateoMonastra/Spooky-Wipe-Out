using System.Collections;
using UnityEngine;

namespace Audio
{
    public class AudioListenerMover : MonoBehaviour
    {
        [SerializeField] private Transform placeToMove;
        [SerializeField] private AkAudioListener listenerToMove;
    
        private Transform _placeToReturn;

        private IEnumerator Start()
        {
            yield return null;

            listenerToMove = FindObjectOfType<AkAudioListener>();
        
            _placeToReturn = listenerToMove.transform.parent;

            if (listenerToMove != null && placeToMove != null)
            {
                listenerToMove.transform.SetParent(placeToMove, false);
            }

        }
        private void OnDestroy()
        {
            ReturnListener();
        }

        private void ReturnListener()
        {
            if (_placeToReturn != null)
            {
                listenerToMove.transform.SetParent(_placeToReturn, false);
            }
        }
    }
}