using TMPro;
using UnityEngine;

namespace AnimaticIntro
{
    public class NextButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        
        public void SetNextButtonText()
        {
            text.text = "NEXT";
        }
        
    }
}
