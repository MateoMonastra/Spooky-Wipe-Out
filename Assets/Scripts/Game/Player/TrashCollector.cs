using System.Collections;
using System.Collections.Generic;
using Fsm_Mk2;
using Ghosts;
using Ghosts.WalkingGhost;
using UnityEngine;


public class TrashCollector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (IsVacuumable(other))
        {
            Trash trash = other.gameObject.transform.parent.GetComponent<Trash>();
            trash?.OnBeingDestroy.Invoke(trash);
            
            ChainGhostAgent ghost = other.gameObject.transform.parent.GetComponent<ChainGhostAgent>();
            if (trash)
            {
                other.gameObject.transform.parent.gameObject.SetActive(false);
            }
            else if (ghost)
            {
                if (ghost.GetCurrentState().ToString() == "Capture")
                {
                    other.gameObject.transform.parent.gameObject.SetActive(false);
                }
            }

            //GameManager.GetInstance().garbage.Remove();
            //other.gameObject.SetActive(false);
            Debug.Log("Trash was Collected");
        }
    }

    private bool IsVacuumable(Collider other)
    {
        IVacuumable vacuumable = other.GetComponentInParent<IVacuumable>();

        return vacuumable != null;
    }
}