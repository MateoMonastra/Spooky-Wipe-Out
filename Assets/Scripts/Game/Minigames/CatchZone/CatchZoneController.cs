using System;
using System.Collections;
using Minigames;
using Player.FSM;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Minigames.CatchZone
{
    public class CatchZoneController : Minigame
    {
        public Action OnCatchProgress;
        public Action OnGhostEscapes;

        [Header("Input")] [SerializeField] private InputReader inputReader;

        [Header("UI References")] [SerializeField]
        private RectTransform catchZone;

        [SerializeField] private RectTransform ghost;
        [SerializeField] private RectTransform barArea;
        [SerializeField] private CatchingUI catchingUI;

        [Header("Catch Zone Physics")] [SerializeField]
        private float initialProgress = 0.2f;

        [SerializeField] private float riseSpeed;
        [SerializeField] private float gravity;
        [SerializeField] private float maxRiseSpeed = 300f;
        [SerializeField] private float maxFallSpeed = 300f;
        [SerializeField] private float gravityResetValue = 50f;

        [Header("Progress")] [SerializeField] private float updateRate;

        [Header("Difficulties")] [SerializeField]
        private CatchingDifficulty[] difficulties;

        private CatchingDifficulty _currentDifficulty;

        private float _catchZoneVelocity;
        private float _ghostTimer;
        private bool _isInputHeld;

        private Coroutine _updateLoop;

        private bool HasPlayerWon => Progress >= 1f;
        private bool HasPlayerLost => Progress <= 0f;

        public override void StartGame()
        {
            if (IsActive|| IsBloqued)  return;
            
            OnStart?.Invoke();
            IsActive = true;
            Progress = initialProgress;

            _currentDifficulty = difficulties[UnityEngine.Random.Range(0, difficulties.Length)];

            catchingUI.gameObject.SetActive(true);
            catchingUI.SetProgress(Progress);

            inputReader.OnSpaceInputStart += OnInputPressed;
            inputReader.OnSpaceInputCancel += OnInputReleased;

            _updateLoop = StartCoroutine(UpdateLoop());
        }

        public override void StopGame()
        {
            if (IsActive)
            {
                OnStop?.Invoke();
                ResetGame();
            }
        }

        protected override void ResetGame()
        {
            IsActive = false;
            Progress = 0f;
            _catchZoneVelocity = 0f;
            _ghostTimer = 0f;

            inputReader.OnSpaceInputStart -= OnInputPressed;
            inputReader.OnSpaceInputCancel -= OnInputReleased;

            if (_updateLoop != null)
                StopCoroutine(_updateLoop);

            catchingUI.SetProgress(0f);
            catchingUI.gameObject.SetActive(false);
        }

        protected override void WinGame()
        {
            ResetGame();
            OnWin?.Invoke();
        }

        protected override void LoseGame()
        {
            ResetGame();
            OnLose?.Invoke();
        }

        private void OnInputPressed() => _isInputHeld = true;
        private void OnInputReleased() => _isInputHeld = false;

        private IEnumerator UpdateLoop()
        {
            while (IsActive)
            {
                UpdateCatchZonePosition();
                UpdateGhostPosition();
                UpdateProgress();
                yield return new WaitForSeconds(updateRate);
            }
        }

        private void UpdateCatchZonePosition()
        {
            if (_isInputHeld)
            {
                if (_catchZoneVelocity <= 0)
                {
                    _catchZoneVelocity = 0;
                }
                _catchZoneVelocity += riseSpeed * updateRate;
            }
            else
            {
                if (_catchZoneVelocity > gravityResetValue)
                {
                    _catchZoneVelocity = gravityResetValue;
                }
                _catchZoneVelocity -= gravity * updateRate;
            }
            
            _catchZoneVelocity = Mathf.Clamp(_catchZoneVelocity, -maxFallSpeed, maxRiseSpeed);

            float newY = catchZone.anchoredPosition.y + _catchZoneVelocity * updateRate;
            float maxY = barArea.rect.height - catchZone.rect.height;
            newY = Mathf.Clamp(newY, 0, maxY);

            catchZone.anchoredPosition = new Vector2(catchZone.anchoredPosition.x, newY);
        }

        private void UpdateGhostPosition()
        {
            _ghostTimer += _currentDifficulty.moveSpeed * updateRate;
            float t = Mathf.PingPong(_ghostTimer, 1f);
            float ghostY = _currentDifficulty.movementCurve.Evaluate(t) * (barArea.rect.height - ghost.rect.height);

            ghost.anchoredPosition = new Vector2(ghost.anchoredPosition.x, ghostY);
        }

        private void UpdateProgress()
        {
            bool isInside = IsGhostInsideCatchZone();

            Progress += (isInside ? _currentDifficulty.increaseRate : -_currentDifficulty.decreaseRate) * updateRate;
            Progress = Mathf.Clamp01(Progress);

            if (isInside)
                OnCatchProgress?.Invoke();
            else
                OnGhostEscapes?.Invoke();

            catchingUI.SetProgress(Progress);

            if (HasPlayerWon)
                WinGame();
            else if (HasPlayerLost)
                LoseGame();
        }

        private bool IsGhostInsideCatchZone()
        {
            float ghostTop = ghost.anchoredPosition.y + ghost.rect.height;
            float ghostBottom = ghost.anchoredPosition.y;

            float zoneTop = catchZone.anchoredPosition.y + catchZone.rect.height;
            float zoneBottom = catchZone.anchoredPosition.y;

            return ghostBottom >= zoneBottom && ghostTop <= zoneTop;
        }
    }
}