using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridgeInteract : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Abre la heladera");
    }
}
