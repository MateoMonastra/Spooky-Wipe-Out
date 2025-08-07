using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandlesInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject[] flames;

    private bool isCandleLit = false;

    private void Start()
    {
        foreach (var flame in flames)
        {
            flame.SetActive(false);
        }
    }

    public void Interact()
    {
        isCandleLit = !isCandleLit;

        foreach (var flame in flames)
        {
            flame.SetActive(isCandleLit);
        }
    }
}
