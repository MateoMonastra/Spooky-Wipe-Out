using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject water;

    private bool isWaterOpen = false;

    private void Start()
    {
        water.SetActive(false);
    }

    public void Interact()
    {
        isWaterOpen = !isWaterOpen;

        water.SetActive(isWaterOpen);
    }
}
