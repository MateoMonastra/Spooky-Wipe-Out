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

    private void Start()
    {
        inputReader.OnSeeTimerStart += ShowTimerUI;
        inputReader.OnSeeTasksStart += ShowTasksUI;
    }

    private void ShowTimerUI()
    {
        if (!_activeTasks)
        {
            _activeTimer = !_activeTimer;
            timerBackground.SetActive(_activeTimer);
            timerText.SetActive(_activeTimer);
        }
    }

    private void ShowTasksUI()
    {
        if (!_activeTimer)
        {
            _activeTasks = !_activeTasks;
            tasksUI.SetActive(_activeTasks);
        }
    }
}
