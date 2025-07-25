using UnityEngine;
using UnityEngine.UI;

namespace Game.Minigames.CatchZone
{
    public class CatchingUI : MonoBehaviour
    {
        [SerializeField] private Image progressBar;

        public void SetProgress(float value)
        {
            progressBar.fillAmount = Mathf.Clamp01(value);
        }
    }
}