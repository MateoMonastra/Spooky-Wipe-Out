using Minigames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VacuumCleaner.Modes
{
    public class Vacuum : MonoBehaviour, ITool
    {
        private Coroutine _turnOnVFX;
        private static readonly int CuttHeight = Shader.PropertyToID("_CuttHeight");
        public event Action OnPowerOn;
        public event Action OnPowerOff;

        [SerializeField] private VacuumColision vacuumColision;
        [SerializeField] private TrashCollector trashCollector;
        [SerializeField] private float maxTimeVFX = 0.5f;
        [SerializeField] private float maxVFXPower = 2.5f;

        [SerializeField] private SkillCheckController SKMinigame;
        [SerializeField] private ADController ADMinigame;

        [SerializeField] private List<Material> materials = new List<Material>();

        private void Start()
        {
            SKMinigame.OnLose += PowerOff;
            ADMinigame.OnLose += PowerOff;
        }

        public void PowerOn()
        {
            if (_turnOnVFX != null)
            {
                StopCoroutine(_turnOnVFX);
            }

            _turnOnVFX = StartCoroutine(TurnOnVFX());
            
            vacuumColision.gameObject.SetActive(true);
            trashCollector.gameObject.SetActive(true);
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

                foreach (var material in materials)
                {
                    material.SetFloat(CuttHeight, VFXPower);
                }

                yield return null;
            }

            foreach (var material in materials)
            {
                material.SetFloat(CuttHeight, maxVFXPower);
            }

            yield break;
        }

        public void PowerOff()
        {
            StopCoroutine(_turnOnVFX);
            vacuumColision.gameObject.SetActive(false);
            trashCollector.gameObject.SetActive(false);
            OnPowerOff?.Invoke();
        }
    }
}