using Player.FSM;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Interactable
{
    public class InteractableObject : MonoBehaviour
    {
        private bool _playerInRange = false;
        private IInteractable _interactable;

        [SerializeField] private GameObject uiInteract;

        [SerializeField] private InputReader inputReader;

        private void Start()
        {
            _interactable = GetComponent<IInteractable>();

            if(_interactable == null )
            {
                Debug.Log($"El objeto {gameObject.name} no tiene IInteractable");
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                _playerInRange = true;
                uiInteract.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = false;
                uiInteract.SetActive(false);
            }
        }

        private void Update()
        {
            if (_playerInRange && inputReader.isInteractPressed == true)
            {
                _interactable?.Interact();
                uiInteract.SetActive(false);
            }
        }
    }
}
