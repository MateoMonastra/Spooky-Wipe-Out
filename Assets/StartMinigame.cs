using Game.Minigames.CatchZone;
using UnityEngine;

public class StartMinigame : MonoBehaviour
{
    [SerializeField] private CatchZoneController catchController;

    private void Start()
    {
        catchController.StartGame();
    }
}
