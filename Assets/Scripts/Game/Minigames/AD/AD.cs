using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Minigames
{
    public class AD : MonoBehaviour
    {
        [SerializeField] private Image rightProgressBar;
        [SerializeField] private Image leftProgressBar;

        [SerializeField] private Animator animator;

        public void SetProgressBarFill( float value )
        {
            rightProgressBar.fillAmount = value;
            leftProgressBar.fillAmount = value;

            animator?.SetBool("isScared", value < 0.5f);
        }
    }
}
