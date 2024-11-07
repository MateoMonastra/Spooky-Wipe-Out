using System;
using System.Collections;
using System.Collections.Generic;
using Minigames;
using Player.FSM;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

enum SkillCheckState
{
    CanLose,
    CantLose
}

public class SkillCheckController : Minigame
{
    [SerializeField] private InputReader inputReader;

    [SerializeField] private float needleSpeed = 100f;
    [SerializeField] private float minNeedleSpeed = 60f;
    [SerializeField] private float maxNeedleSpeed = 600f;

    [SerializeField] private float decreaseRate = 0.1f;
    [SerializeField] private float increaseAmount = 0.15f;
    [SerializeField] private float decreaseAmount = -0.15f;

    [SerializeField] private float maxProgress = 1f;
    [SerializeField] private float minProgress = 0f;

    [SerializeField] private float maxWidthSafeZone;
    [SerializeField] private float minWidthSafeZone;

    [SerializeField] private SkillCheck skillCheck;
    [SerializeField] private SkillCheckState skillCheckState;

    [SerializeField] private AnimationCurve decreaseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private float progress { get; set; } = 0.0f;

    private bool HasPlayerWon => progress >= maxProgress;
    private bool HasPlayerLost;

    private float _needleDir = 1;

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

    private void UpdateProgress(float value)
    {
        progress = value;
        if (HasPlayerWon)
            WinGame();
        else if (HasPlayerLost)
            LoseGame();

        skillCheck.SetProgressBarFill(progress);
    }

    protected override void ResetGame()
    {
        progress = minProgress;
        _isActive = false;
        StopCoroutine(DecreaseProgressOverTime());
        StopCoroutine(MoveNeedleOverTime());
        skillCheck.gameObject.SetActive(false);
    }

    public override void StartGame()
    {
        OnStart?.Invoke();
        HasPlayerLost = false;
        inputReader.OnSpaceInputStart += HandleInput;
        skillCheck.gameObject.SetActive(true);
        StartCoroutine(DecreaseProgressOverTime());
        StartCoroutine(MoveNeedleOverTime());
        RandomizeSafeZoneWidth();
        _isActive = true;
    }

    private void MoveNeedle()
    {
        var transformLocalPosition = skillCheck.needle.transform.localPosition;

        if (transformLocalPosition.x <= skillCheck.bar.offsetMin.x)
        {
            _needleDir = 1;
        }
        else if (transformLocalPosition.x >= skillCheck.bar.offsetMax.x)
        {
            _needleDir = -1;
        }

        transformLocalPosition.x += needleSpeed * _needleDir * Time.deltaTime;

        skillCheck.needle.transform.localPosition = transformLocalPosition;
    }

    private void HandleInput()
    {
        RectTransform needleRect = skillCheck.needle;
        RectTransform safeZoneRect = skillCheck.safeZone;

        if (IsColliding(needleRect, safeZoneRect))
        {
            UpdateProgress(progress + increaseAmount);
            
            RandomizeSafeZoneWidth();
            RandomizeSafeZonePosition();
            RandomizeNeedleSpeed();
            
        }
        else if( progress <= minProgress)
        {
            HasPlayerLost = true;
        }
        else
        {
            UpdateProgress(progress + decreaseAmount);
        }
    }

    private void RandomizeSafeZoneWidth()
    {
        float randomWidth = Random.Range(minWidthSafeZone, maxWidthSafeZone);
        skillCheck.safeZone.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, randomWidth);
    }

    private void RandomizeSafeZonePosition()
    {
        float newX = Random.Range(skillCheck.bar.offsetMin.x, skillCheck.bar.offsetMax.x);
        skillCheck.safeZone.transform.localPosition = new Vector2(newX, skillCheck.safeZone.transform.localPosition.y);
    }

    private void RandomizeNeedleSpeed()
    {
        float newSpeed = Random.Range(minNeedleSpeed, maxNeedleSpeed);
        needleSpeed = newSpeed;
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

    public bool IsColliding(RectTransform rectA, RectTransform rectB)
    {
        if (rectA == null || rectB == null)
        {
            Debug.LogWarning("RectTransform no asignado");
            return false;
        }

        Rect rect1 = GetScreenRect(rectA);
        Rect rect2 = GetScreenRect(rectB);

        return rect1.Overlaps(rect2);
    }

    public override void StopGame()
    {
        if (_isActive)
        {
            OnStop?.Invoke();
            ResetGame();
        }
    }

    private IEnumerator DecreaseProgressOverTime()
    {
        while (skillCheck.gameObject.activeInHierarchy)
        {
            DecreaseProgress();
            yield return null;
        }
    }

    private IEnumerator MoveNeedleOverTime()
    {
        while (skillCheck.gameObject.activeInHierarchy)
        {
            MoveNeedle();
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