using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] private GameObject finishScreen;
    [SerializeField] private PreselectButton preselectButton;
    [SerializeField] private GameObject button;

    private void Start()
    {
        GameManager.GetInstance().OnFinish += InitFinishScreen;
    }


    private void InitFinishScreen()
    {
        Time.timeScale = 0f;
        finishScreen.SetActive(true);
        preselectButton.SetPreselectedButton(button);
    }
}
