using Minigames;
using UnityEngine;

namespace Game.Minigames
{
    public class StartMinigameTest : MonoBehaviour
    {
        [SerializeField] private Minigame minigame;

        private void Start()
        {
            minigame.StartGame();
        }
    }
}
