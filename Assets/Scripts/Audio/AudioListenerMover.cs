using System.Collections;
using UnityEngine;

namespace Audio
{
    public class AudioListenerMover : MonoBehaviour
    {
        [SerializeField] private Transform placeToMove;
        [SerializeField] private AkAudioListener listenerToMove;
        [SerializeField] private PauseManager pauseManager;

        private Transform _placeToReturn;

        private void OnEnable()
        {
            pauseManager.onPauseGoMenu.AddListener(ReturnListener);
            pauseManager.onPauseRestart.AddListener(ReturnListener);
        }

        private void OnDisable()
        {
            pauseManager.onPauseGoMenu.RemoveListener(ReturnListener);
            pauseManager.onPauseRestart.RemoveListener(ReturnListener);
        }

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

        private void ReturnListener()
        {
            if (_placeToReturn != null)
            {
                listenerToMove.transform.SetParent(_placeToReturn, false);
            }
        }
    }
}