using System;
using System.Collections;
using Player.FSM;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minigames
{
    public class ADController : Minigame
    {
        [SerializeField] InputReader inputReader;

        [SerializeField] private AD ad;

        [SerializeField] private float decreaseRate = 0.1f;
        [SerializeField] private float increaseAmount = 0.15f;
        [SerializeField] private float maxProgress = 1f;
        [SerializeField] private float minProgress = 0f;
        [SerializeField] private float threshold = 0.8f;
        
        [SerializeField] private AnimationCurve decreaseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        private float progress { get; set; } = 0.0f;
        private float _expectedSign = 1.0f;

        private bool HasPlayerWon => progress >= maxProgress;

        private bool HasPlayerLost => progress <= minProgress;

        public override void StartGame()
        {
            if (_isActive) return;
            
            OnStart?.Invoke();
            
            ad.gameObject.SetActive(true);
            inputReader.OnMove += HandleInput;
            progress = minProgress;
            StartCoroutine(DecreaseProgressOverTime());
        }

        public override void StopGame()
        {
            OnStop?.Invoke();
            ResetGame();
        }

        protected override void WinGame()
        {
            OnWin?.Invoke();
            ResetGame();
        }

        protected override void LoseGame()
        {
            OnLose?.Invoke();
            ResetGame();
        }

        protected override void ResetGame()
        {
            inputReader.OnMove -= HandleInput;
            StopCoroutine(DecreaseProgressOverTime());

            progress = minProgress;
            ad.gameObject.SetActive(false);
        }

        private void HandleInput(Vector2 inputDirection)
        {
            float absDirection = Mathf.Abs(inputDirection.x);
            float directionSign = Mathf.Sign(inputDirection.x);

            if (Mathf.Approximately(directionSign, _expectedSign) && absDirection >= threshold)
            {
                UpdateProgress(progress + increaseAmount);
                _expectedSign *= -1;
            }
        }

        private void UpdateProgress(float value)
        {
            progress = value;
            if (HasPlayerWon)
                WinGame();

            ad.SetProgressBarFill(progress);
        }

        private IEnumerator DecreaseProgressOverTime()
        {
            while (ad.gameObject.activeInHierarchy)
            {
                DecreaseProgress();
                yield return null;
            }
        }

        private void DecreaseProgress()
        {
            float curveValue = decreaseCurve.Evaluate(progress);
            
            var decreaseAmount = Mathf.Clamp(
                progress - decreaseRate * Time.deltaTime * curveValue,
                minProgress, maxProgress);

            UpdateProgress(decreaseAmount);
        }
    }
}