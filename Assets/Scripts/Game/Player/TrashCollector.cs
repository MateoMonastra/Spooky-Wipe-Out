using Game.Garbage;
using Game.Ghosts.ChainGhost;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Player
{
    public class TrashCollector : MonoBehaviour
    {
        public UnityEvent OnTrashCollected;
        public UnityEvent OnGhostCollected;
    
        private void OnTriggerEnter(Collider other)
        {
            if (IsVacuumable(other))
            {
                Trash trash = other.transform.parent?.GetComponent<Trash>();
                trash?.OnBeingDestroy.Invoke(trash);
            
                ChainGhostAgent ghost = other.gameObject.transform.parent.GetComponent<ChainGhostAgent>();
                if (trash)
                {
                    other.gameObject.transform.parent.gameObject.SetActive(false);
                    OnTrashCollected?.Invoke();
                }
                else if (ghost)
                {
                    if (ghost.GetCurrentState().ToString() == "Capture")
                    {
                        other.gameObject.transform.parent.gameObject.SetActive(false);
                        OnGhostCollected?.Invoke();
                    }
                }
            }
        }

        private bool IsVacuumable(Collider other)
        {
            IVacuumable vacuumable = other.GetComponentInParent<IVacuumable>();

            return vacuumable != null;
        }
    }
}