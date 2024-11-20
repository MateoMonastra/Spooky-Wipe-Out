using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] private GameObject finishScreen;
    [SerializeField] private PreselectButton preselectButton;
    [SerializeField] private GameObject button;
    [SerializeField] private TextMeshProUGUI timeText;

    private void Start()
    {
        GameManager.GetInstance().OnFinish += InitFinishScreen;
    }


    private void InitFinishScreen()
    {
        Time.timeScale = 0f;
        finishScreen.SetActive(true);
        preselectButton.SetPreselectedButton(button);
        timeText.text = GameManager.GetInstance().timer.SetTimerText();
    }

    public void GoMenu()
    {
        Time.timeScale = 1f;
    }
}
