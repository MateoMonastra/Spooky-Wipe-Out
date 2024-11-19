using Player.FSM;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class HudController : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    
    [SerializeField] private List<GameObject> pcHUD;
    [SerializeField] private List<GameObject> gamePadHUD;

    private void OnEnable()
    {
        inputReader.OnInputDevice += OnAnyButtonPress;
    }

    private void OnDisable()
    {
        inputReader.OnInputDevice -= OnAnyButtonPress;
    }
    
    private void OnAnyButtonPress(InputDevice control)
    {
        var device = control.device;
        
        if (device is Mouse || device is Keyboard)
        {
            foreach (var sprite in pcHUD)
            {
                sprite.GameObject().gameObject.SetActive(true);
            }
            foreach (var sprite in gamePadHUD)
            {
                sprite.GameObject().gameObject.SetActive(false);
            }
        }
        else if (device is Gamepad)
        {
            foreach (var sprite in gamePadHUD)
            {
                sprite.GameObject().gameObject.SetActive(true);
            }
            foreach (var sprite in pcHUD)
            {
                sprite.GameObject().gameObject.SetActive(false);
            }
        }
        
    }
}