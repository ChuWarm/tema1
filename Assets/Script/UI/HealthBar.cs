using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;
    public TextMeshProUGUI valueText;
    public bool showValue = true;
    public bool followTarget = true;
    public float smoothSpeed = 10f;

    private int maxValue;
    private Transform target;
    private Vector3 offset;
    

    public void Initialize(int maxValue, Transform target = null)
    {
        this.maxValue = maxValue;
        this.target = target;
        offset = transform.position - (target != null ? target.position : Vector3.zero);
        UpdateValue(maxValue);
    }

    public void UpdateValue(int currentValue)
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = (float)currentValue / maxValue;
        }

        if (valueText != null && showValue)
        {
            valueText.text = $"{currentValue}/{maxValue}";
        }
    }

    private void LateUpdate()
    {
        if (followTarget && target != null)
        {
            Vector3 targetPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
    }
} 