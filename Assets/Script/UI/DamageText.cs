using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private float moveSpeed = 1f;
    private float fadeSpeed = 1f;
    private float lifeTime = 1f;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    public void Initialize(int amount, Color color)
    {
        textMesh.text = amount.ToString();
        textMesh.color = color;
        StartCoroutine(FadeOut());
    }

    private System.Collections.IEnumerator FadeOut()
    {
        float currentTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + Vector3.up * 2f;

        while (currentTime < lifeTime)
        {
            currentTime += Time.deltaTime;
            float alpha = 1f - (currentTime / lifeTime);
            textMesh.alpha = alpha;
            transform.position = Vector3.Lerp(startPosition, targetPosition, currentTime / lifeTime);
            yield return null;
        }

        Destroy(gameObject);
    }
} 