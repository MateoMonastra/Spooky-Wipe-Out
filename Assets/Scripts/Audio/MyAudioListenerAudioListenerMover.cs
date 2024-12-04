using System.Collections;
using UnityEngine;

namespace Audio
{
    public class AudioListenerMover : MonoBehaviour
    {
        [SerializeField] private Transform placeToMove;
        [SerializeField] private MyAudioListener listenerToMove;
        
        private IEnumerator Start()
        {
            yield return null;
            
            if (listenerToMove != null && placeToMove != null)
            {
                listenerToMove.GetListener().transform.SetParent(placeToMove, false);
            }
        }
    }
}