using Player.FSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    private bool playerInRange = false;
    private IInteractable interactable;

    [SerializeField] private GameObject UIInteract;

    [SerializeField] private InputReader inputReader;

    private void Start()
    {
        interactable = GetComponent<IInteractable>();

        if(interactable == null )
        {
            Debug.Log($"El objeto {gameObject.name} no tiene IInteractable");
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
            UIInteract.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            UIInteract.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && inputReader.isInteractPressed == true)
        {
            interactable?.Interact();
        }
    }
}
