using UnityEngine;

namespace Game.Interactable
{
    public class FridgeAnimationEvents : MonoBehaviour
    {
        [SerializeField] private FridgeInteract fridgeInteract;

        public void NotifyAnimationEnd()
        {
            fridgeInteract.Reset();
        }
        
        
    }
}
