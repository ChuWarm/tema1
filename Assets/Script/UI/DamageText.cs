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
            if (damageText == null)
            {
                // 자식 오브젝트에서 찾아보기
                damageText = GetComponentInChildren<TextMeshProUGUI>();
                if (damageText == null)
                {
                    Debug.LogError("DamageText 컴포넌트가 TextMeshProUGUI를 찾을 수 없습니다!");
                }
            }
            
            moveDirection = Vector3.up;
            currentLifeTime = lifeTime;
        }

        private void Update()
        {
            // damageText가 null이면 업데이트하지 않음
            if (damageText == null) return;
            
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
            // damageText가 null이면 초기화하지 않음
            if (damageText == null)
            {
                Debug.LogError("DamageText Initialize: damageText is null!");
                return;
            }
            
            damageText.text = damage.ToString();
            damageText.color = color;
        }
    }
} 