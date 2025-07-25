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

        [Header("Input")]
        [SerializeField] private InputReader inputReader;

        [Header("UI References")]
        [SerializeField] private RectTransform catchZone;
        [SerializeField] private RectTransform ghost;
        [SerializeField] private RectTransform barArea;
        [SerializeField] private CatchingUI catchingUI;

        [Header("Catch Zone Physics")]
        [SerializeField] private float riseSpeed;
        [SerializeField] private float gravity;

        [Header("Progress")]
        [SerializeField] private float updateRate;

        [Header("Difficulties")]
        [SerializeField] private CatchingDifficulty[] difficulties;

        private CatchingDifficulty _currentDifficulty;

        private float _catchZoneVelocity;
        private float _ghostTimer;
        private float _progress;

        private Coroutine _updateLoop;

        private bool HasPlayerWon => _progress >= 1f;
        private bool HasPlayerLost => _progress <= 0f;

        public override void StartGame()
        {
            OnStart?.Invoke();
            _isActive = true;
            _progress = 0.2f;

            _currentDifficulty = difficulties[UnityEngine.Random.Range(0, difficulties.Length)];

            catchingUI.gameObject.SetActive(true);
            catchingUI.SetProgress(_progress);

            inputReader.OnSpaceInputStart += OnInputPressed;

            _updateLoop = StartCoroutine(UpdateLoop());
        }

        public override void StopGame()
        {
            if (_isActive)
            {
                OnStop?.Invoke();
                ResetGame();
            }
        }

        protected override void ResetGame()
        {
            _isActive = false;
            _progress = 0f;
            _catchZoneVelocity = 0f;
            _ghostTimer = 0f;

            inputReader.OnSpaceInputStart -= OnInputPressed;

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

        private void OnInputPressed() => _catchZoneVelocity = riseSpeed;

        private IEnumerator UpdateLoop()
        {
            while (_isActive)
            {
                UpdateCatchZonePosition();
                UpdateGhostPosition();
                UpdateProgress();
                yield return new WaitForSeconds(updateRate);
            }
        }

        private void UpdateCatchZonePosition()
        {
            _catchZoneVelocity -= gravity * updateRate;

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

            _progress += (isInside ? _currentDifficulty.increaseRate : -_currentDifficulty.decreaseRate) * updateRate;
            _progress = Mathf.Clamp01(_progress);

            if (isInside)
                OnCatchProgress?.Invoke();
            else
                OnGhostEscapes?.Invoke();

            catchingUI.SetProgress(_progress);

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
