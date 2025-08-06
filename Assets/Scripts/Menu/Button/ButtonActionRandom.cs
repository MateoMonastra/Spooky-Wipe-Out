using Menu.MenuButton;
using UnityEngine;

namespace Menu.Button
{
    public class ButtonActionRandom : MonoBehaviour
    {
        [SerializeField] private ButtonHandler[] randomHandlers;

        public void HandleClickRandom()
        {
            if (randomHandlers == null || randomHandlers.Length == 0)
            {
                Debug.LogWarning("No handlers assigned for ButtonActionRandom");
                return;
            }

            int index = Random.Range(0, randomHandlers.Length);
            randomHandlers[index].Handle();
        }
    }
}