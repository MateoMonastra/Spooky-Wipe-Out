using System.Collections;
using UnityEngine;

namespace Audio
{
    public class AudioListenerMover : MonoBehaviour
    {
        [SerializeField] private Transform placeToMove;
        [SerializeField] private MyAudioListener listenerToMove;
        
        private Transform _placeToReturn;
        
        private IEnumerator Start()
        {
            yield return null;

            _placeToReturn = listenerToMove.GetListener().transform.parent;

            if (listenerToMove != null && placeToMove != null)
            {
                listenerToMove.GetListener().transform.SetParent(placeToMove, false);
            }
        }
        public void ReturnListener()
        {
            if (_placeToReturn != null)
            {
                listenerToMove.GetListener().transform.SetParent(_placeToReturn, false);
            }
        }
    }
}