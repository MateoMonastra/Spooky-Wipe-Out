using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Minigames
{
    public class SkillCheck : MonoBehaviour
    {
        public RectTransform needle;
        public RectTransform bar;
        public RectTransform safeZone;
        public Image progressPointsBar;
        
        public void SetProgressBarFill( float value )
        {
            progressPointsBar.fillAmount = value;
        }
    }
}