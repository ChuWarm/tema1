using UnityEngine;
using System.Collections.Generic;
using Script.UI;

namespace Script.Core
{
    public class EffectManager : MonoBehaviour
    {
        public static EffectManager Instance { get; private set; }

        [System.Serializable]
        public class EffectData
        {
            public string effectName;
            public GameObject effectPrefab;
            public AudioClip soundEffect;
            public float duration = 1f;
        }

        [Header("전투 이펙트")]
        public EffectData attackEffect;
        public EffectData hitEffect;
        public EffectData deathEffect;
        public EffectData healEffect;

        [Header("UI 이펙트")]
        public GameObject damageTextPrefab;
        public GameObject healTextPrefab;
        public GameObject expTextPrefab;

        private Dictionary<string, Queue<GameObject>> effectPools = new Dictionary<string, Queue<GameObject>>();
        private const int POOL_SIZE = 10;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializePools();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializePools()
        {
            // 전투 이펙트 풀 초기화
            InitializeEffectPool(attackEffect);
            InitializeEffectPool(hitEffect);
            InitializeEffectPool(deathEffect);
            InitializeEffectPool(healEffect);
        }

        private void InitializeEffectPool(EffectData effectData)
        {
            if (effectData.effectPrefab == null) return;

            Queue<GameObject> pool = new Queue<GameObject>();
            for (int i = 0; i < POOL_SIZE; i++)
            {
                GameObject obj = Instantiate(effectData.effectPrefab);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
            effectPools[effectData.effectName] = pool;
        }

        public void PlayEffect(string effectName, Vector3 position, Quaternion rotation)
        {
            if (!effectPools.ContainsKey(effectName)) return;

            Queue<GameObject> pool = effectPools[effectName];
            if (pool.Count == 0) return;

            GameObject effect = pool.Dequeue();
            effect.transform.position = position;
            effect.transform.rotation = rotation;
            effect.SetActive(true);

            // 사운드 재생
            EffectData effectData = GetEffectData(effectName);
            if (effectData.soundEffect != null)
            {
                AudioSource.PlayClipAtPoint(effectData.soundEffect, position);
            }

            StartCoroutine(ReturnToPool(effect, effectName, effectData.duration));
        }

        private EffectData GetEffectData(string effectName)
        {
            switch (effectName)
            {
                case "Attack": return attackEffect;
                case "Hit": return hitEffect;
                case "Death": return deathEffect;
                case "Heal": return healEffect;
                default: return null;
            }
        }

        private System.Collections.IEnumerator ReturnToPool(GameObject effect, string effectName, float duration)
        {
            yield return new WaitForSeconds(duration);
            effect.SetActive(false);
            effectPools[effectName].Enqueue(effect);
        }

        public void ShowDamageText(int damage, Vector3 position)
        {
            if (damageTextPrefab == null) return;

            GameObject textObj = Instantiate(damageTextPrefab, position, Quaternion.identity);
            DamageText damageText = textObj.GetComponent<DamageText>();
            if (damageText != null)
            {
                damageText.Initialize(damage, Color.red);
            }
        }

        public void ShowHealText(int amount, Vector3 position)
        {
            if (healTextPrefab == null) return;

            GameObject textObj = Instantiate(healTextPrefab, position, Quaternion.identity);
            DamageText healText = textObj.GetComponent<DamageText>();
            if (healText != null)
            {
                healText.Initialize(amount, Color.green);
            }
        }

        public void ShowExpText(int amount, Vector3 position)
        {
            if (expTextPrefab == null) return;

            GameObject textObj = Instantiate(expTextPrefab, position, Quaternion.identity);
            DamageText expText = textObj.GetComponent<DamageText>();
            if (expText != null)
            {
                expText.Initialize(amount, Color.yellow);
            }
        }
    }
} 