using System.Collections.Generic;
using Player.FSM;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class HudController : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
    
        [SerializeField] private List<GameObject> pcHUD;
        [SerializeField] private List<GameObject> gamePadHUD;
        
        [SerializeField] private List<GameObject> instructionsToShow;
        [SerializeField] private List<GameObject> instructionsToHide;

        private void OnEnable()
        {
            inputReader.OnInputDevice += OnAnyButtonPress;
            inputReader.OnIntructionsStart += ShowInstructions;
            inputReader.OnIntructionsEnd += HideInstructions;
        }

        private void OnDisable()
        {
            inputReader.OnInputDevice -= OnAnyButtonPress;
            inputReader.OnIntructionsStart -= ShowInstructions;
            inputReader.OnIntructionsEnd -= HideInstructions;
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

        private void ShowInstructions()
        {
            foreach (var instruction in instructionsToShow)
            {
                instruction.GameObject().gameObject.SetActive(true);
            }

            foreach (var instructions in instructionsToHide)
            {
                instructions.GameObject().gameObject.SetActive(false);
            }
        }

        private void HideInstructions()
        {
            foreach (var instruction in instructionsToShow)
            {
                instruction.GameObject().gameObject.SetActive(false);
            }

            foreach (var instructions in instructionsToHide)
            {
                instructions.GameObject().gameObject.SetActive(true);
            }
        }
    }
}