using System;
using UnityEngine;

namespace Audio
{
    public class SetAudioListener : MonoBehaviour
    {
        [SerializeField] private MyAudioListener myListener;
        [SerializeField] private AkAudioListener listener;

        private void Start()
        {
            myListener.SetListener(listener);
        }
    }
}
