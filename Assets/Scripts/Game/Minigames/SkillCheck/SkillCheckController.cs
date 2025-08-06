using System;
using System.Collections;
using Minigames;
using Player.FSM;
using UnityEngine;
using Random = UnityEngine.Random;

enum SkillCheckState
{
    CanLose,
    CantLose
}

public class SkillCheckController : Minigame
{
    private static readonly int IsScared = Animator.StringToHash("isScared");
    public Action OnCheckPass;
    public Action OnCheckFail;

    [Header("Input")] [SerializeField] private InputReader inputReader;

    [Header("Needle Settings")] 
    [SerializeField] private float minNeedleSpeed = 400f;
    [SerializeField] private float maxNeedleSpeed = 600f;
    [SerializeField] private float needleAcceleration = 5f;
    [SerializeField] private float needlePenaltyOnFail = 50f;

    [Header("Progress Settings")] [SerializeField]
    private float decreaseRate = 0.1f;

    [SerializeField] private float increaseAmount = 0.15f;
    [SerializeField] private float decreaseAmount = -0.15f;
    [SerializeField] private float maxProgress = 1f;
    [SerializeField] private float minProgress = 0f;
    [SerializeField] private float scaredProgress = 0.7f;

    [Header("Safe Zone Settings")] [SerializeField]
    private float maxWidthSafeZone = 150f;

    [SerializeField] private float minWidthSafeZone = 50f;

    [Header("Skill Check UI")] [SerializeField]
    private SkillCheck skillCheck;

    [SerializeField] private SkillCheckState skillCheckState;

    [SerializeField] private AnimationCurve decreaseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private float progress { get; set; } = 0f;
    private bool HasPlayerWon => progress >= maxProgress;
    private bool HasPlayerLost;

    private float _needleSpeed;
    private float _needleDir = 1f;

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

    public override void StartGame()
    {
        if (_isActive) return;
        
        OnStart?.Invoke();
        HasPlayerLost = false;
        _isActive = true;

        _needleSpeed = minNeedleSpeed;
        progress = minProgress;

        skillCheck.gameObject.SetActive(true);
        inputReader.OnSpaceInputStart += HandleInput;

        RandomizeSafeZone();

        StartCoroutine(DecreaseProgressOverTime());
        StartCoroutine(MoveNeedleOverTime());
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
        inputReader.OnSpaceInputStart -= HandleInput;
        _needleSpeed = minNeedleSpeed; 
        StopAllCoroutines();

        skillCheck.gameObject.SetActive(false);
    }

    private void HandleInput()
    {
        if (IsColliding(skillCheck.needle, skillCheck.safeZone))
        {
            OnCheckPass?.Invoke();
            UpdateProgress(progress + increaseAmount);

            //_needleSpeed = Mathf.Clamp(_needleSpeed + needleAcceleration, minNeedleSpeed, maxNeedleSpeed);
            _needleSpeed = Random.Range(minNeedleSpeed, maxNeedleSpeed);

            RandomizeSafeZone();
        }
        else
        {
            OnCheckFail?.Invoke();

            if (progress <= minProgress)
            {
                HasPlayerLost = true;
            }
            else
            {
                UpdateProgress(progress + decreaseAmount);
                //_needleSpeed = Mathf.Clamp(_needleSpeed - needlePenaltyOnFail, minNeedleSpeed, maxNeedleSpeed);
                _needleSpeed = Random.Range(minNeedleSpeed, maxNeedleSpeed);
            }
        }
    }

    private void UpdateProgress(float value)
    {
        progress = Mathf.Clamp(value, minProgress, maxProgress);
        skillCheck.SetProgressBarFill(progress);
        ChangeScaredAnimation(progress >= scaredProgress);
        
        if (HasPlayerWon)
            WinGame();
        else if (HasPlayerLost)
            LoseGame();
    }

    private void MoveNeedle()
    {
        RectTransform needle = skillCheck.needle;
        RectTransform bar = skillCheck.bar;

        Vector2 localPos = needle.localPosition;

        if (localPos.x <= bar.offsetMin.x)
            _needleDir = 1f;
        else if (localPos.x >= bar.offsetMax.x)
            _needleDir = -1f;

        localPos.x += _needleSpeed * _needleDir * Time.deltaTime;
        needle.localPosition = localPos;
    }

    private IEnumerator MoveNeedleOverTime()
    {
        while (skillCheck.gameObject.activeInHierarchy)
        {
            MoveNeedle();
            yield return null;
        }
    }

    private IEnumerator DecreaseProgressOverTime()
    {
        while (skillCheck.gameObject.activeInHierarchy)
        {
            float curveValue = decreaseCurve.Evaluate(progress);
            float nextProgress = progress - decreaseRate * Time.deltaTime * curveValue;
            UpdateProgress(nextProgress);
            yield return null;
        }
    }

    private void RandomizeSafeZone()
    {
        float barWidth = skillCheck.bar.rect.width;
        float safeZoneWidth = Random.Range(minWidthSafeZone, maxWidthSafeZone);
        safeZoneWidth = Mathf.Min(safeZoneWidth, barWidth); // no más grande que la barra

        skillCheck.safeZone.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, safeZoneWidth);

        float halfSafeWidth = safeZoneWidth * 0.5f;
        float minX = skillCheck.bar.offsetMin.x + halfSafeWidth;
        float maxX = skillCheck.bar.offsetMax.x - halfSafeWidth;
        float randomX = Random.Range(minX, maxX);

        Vector2 newPos = new Vector2(randomX, skillCheck.safeZone.localPosition.y);
        skillCheck.safeZone.localPosition = newPos;
    }

    private bool IsColliding(RectTransform rectA, RectTransform rectB)
    {
        if (rectA == null || rectB == null)
        {
            Debug.LogWarning("RectTransform no asignado.");
            return false;
        }

        Rect rect1 = GetScreenRect(rectA);
        Rect rect2 = GetScreenRect(rectB);
        return rect1.Overlaps(rect2);
    }

    private Rect GetScreenRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        float xMin = corners[0].x;
        float xMax = corners[2].x;
        float yMin = corners[0].y;
        float yMax = corners[2].y;

        return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }

    private void ChangeScaredAnimation(bool value)
    {
        skillCheck.safeZoneAnimator.SetBool(IsScared, value);
    }
}