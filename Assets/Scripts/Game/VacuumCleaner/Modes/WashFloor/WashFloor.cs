using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VacuumCleaner.Modes
{
    public class WashFloor : MonoBehaviour, ITool
    {
        private Coroutine _turnOnVFX;
        private static readonly int SmoothMask = Shader.PropertyToID("_SmoothMask");
        public event Action OnPowerOn;
        public event Action OnPowerOff;

        [SerializeField] WashFloorColision washFloorColision;
        [SerializeField] private float maxTimeVFX = 0.5f;
        [SerializeField] private float maxVFXPower = 23.0f;
        [SerializeField] private Material material;

        public void PowerOn()
        {
            if (_turnOnVFX != null)
            {
                StopCoroutine(_turnOnVFX);
            }

            _turnOnVFX = StartCoroutine(TurnOnVFX());

            washFloorColision.gameObject.SetActive(true);
            OnPowerOn?.Invoke();
        }

        private IEnumerator TurnOnVFX()
        {
            float _timerVFX = 0;
            float VFXPower = 0;
            while (_timerVFX <= maxTimeVFX)
            {
                _timerVFX += Time.deltaTime;
                VFXPower = Mathf.Lerp(0, maxVFXPower, _timerVFX / maxTimeVFX);


                material.SetFloat(SmoothMask, VFXPower);


                yield return null;
            }


            material.SetFloat(SmoothMask, maxVFXPower);
        }

        public void PowerOff()
        {
            washFloorColision.gameObject.SetActive(false);
            OnPowerOff?.Invoke();
        }
    }
}