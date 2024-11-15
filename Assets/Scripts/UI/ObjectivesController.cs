using System.Collections;
using System.Collections.Generic;
using Player.FSM;
using UnityEngine;

public class ObjectivesController : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    [SerializeField] private GameObject timerBackground;
    [SerializeField] private GameObject timerText;
    [SerializeField] private GameObject tasksUI;

    private bool _activeTimer = false;
    private bool _activeTasks = false;

    private void OnEnable()
    {
        inputReader.OnShowTimer += ShowTimerUI;
        inputReader.OnHideTimer += HideTimerUI;
        inputReader.OnShowTasks += ShowTasksUI;
        inputReader.OnHideTasks += HideTasksUI;
    }

    private void OnDisable()
    {
        inputReader.OnShowTimer -= ShowTimerUI;
        inputReader.OnShowTasks -= ShowTasksUI;
        inputReader.OnShowTasks -= ShowTasksUI;
        inputReader.OnHideTasks -= HideTasksUI;
    }

    private void ShowTimerUI()
    {
        if (!_activeTasks)
        {
            ToggleTimer(true);
        }
    }

    private void HideTimerUI()
    {
        ToggleTimer(false);
    }

    private void ToggleTimer(bool value)
    {
        timerBackground.SetActive(value);
        timerText.SetActive(value);
        _activeTimer = value;
    }
    private void ShowTasksUI()
    {
        if (!_activeTimer)
        {
            ToggleTasks(true);
        }
    }

    private void HideTasksUI()
    {
        ToggleTasks(false);
    }

    private void ToggleTasks(bool value)
    {
        tasksUI.SetActive(value);
        _activeTasks = value;
    }
}
