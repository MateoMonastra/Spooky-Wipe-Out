using Player.FSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PreselectButton : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem; 
    [SerializeField] private InputReader inputReader;

    private GameObject selectedButton;

    private void OnEnable()
    {
        inputReader.OnNavigate += RestartNavigation;
    }

    private void OnDisable()
    {
        inputReader.OnNavigate -= RestartNavigation;
    }

    public void SetPreselectedButton(GameObject button)
    {
        if (eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(button);

            selectedButton = button;
        }
    }

    public void RestartNavigation()
    {
        if (eventSystem != null && eventSystem.currentSelectedGameObject == null)
        {
            eventSystem.SetSelectedGameObject(selectedButton);
        }
    }
}
