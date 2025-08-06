using UnityEngine;

namespace Game.Ghosts.PaintGhost
{
    public class PaintGhostAnimationEvents : MonoBehaviour
    {
        public PaintGhostAgent paintGhostAgent;
        
        public void NotifyDeathAnimationEnd()
        {
            if (paintGhostAgent != null)
            {
                paintGhostAgent.OnDeathAnimationEnd();
            }
        }
    }
}