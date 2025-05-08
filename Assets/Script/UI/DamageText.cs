using UnityEngine;
using TMPro;

namespace Script.UI
{
    public class DamageText : MonoBehaviour
    {
        private TextMeshProUGUI damageText;
        private float moveSpeed = 1f;
        private float fadeSpeed = 1f;
        private float lifeTime = 1f;
        private float currentLifeTime;
        private Vector3 moveDirection;

        private void Awake()
        {
            damageText = GetComponent<TextMeshProUGUI>();
            moveDirection = Vector3.up;
            currentLifeTime = lifeTime;
        }

        private void Update()
        {
            // 수직으로 이동
            transform.position += moveSpeed * Time.deltaTime * moveDirection;

            // 페이드 아웃
            float alpha = currentLifeTime / lifeTime;
            Color color = damageText.color;
            color.a = alpha;
            damageText.color = color;

            // 수명 감소
            currentLifeTime -= Time.deltaTime;

            // 수명이 다하면 제거
            if (currentLifeTime <= 0)
            {
                Destroy(gameObject);
            }
        }

        public void Initialize(int damage, Color color)
        {
            damageText.text = damage.ToString();
            damageText.color = color;
        }
    }
} 