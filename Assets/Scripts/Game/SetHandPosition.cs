using UnityEngine;
using UnityEngine.UI;

public class SetHandPosition : MonoBehaviour
{
    [SerializeField] private Image fillBar;
    
    public void UpdateTipPosition()
    {
        if (fillBar == null || fillBar.type != Image.Type.Filled || fillBar.fillMethod != Image.FillMethod.Horizontal)
        {
            Debug.LogWarning("The Image is either null, not in Filled mode, or not set to Horizontal fill.");
            return;
        }

        RectTransform rectTransform = fillBar.rectTransform;
       
        float xPosition = rectTransform.rect.width * (fillBar.fillAmount - 0.5f);
        
        Vector3 worldTipPosition = rectTransform.TransformPoint(new Vector3(xPosition, 0, 0));
        gameObject.transform.position = worldTipPosition;
    }

    private void Update()
    {
        UpdateTipPosition();
    }
}
