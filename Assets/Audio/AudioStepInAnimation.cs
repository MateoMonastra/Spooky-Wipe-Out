using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioStepInAnimation : MonoBehaviour
{
    [SerializeField] GameObject AudioPlayer;
    [SerializeField] AK.Wwise.Event StepEvent;
    private void StepSound()
    {
        StepEvent.Post(AudioPlayer);
    }

}
