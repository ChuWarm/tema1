using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;
using Script.Core;

namespace Script.Characters
{
    public static class EnemyFactory
    {
        // RoomEventHolder 빼고 Vector3 position 매개변수 추가
        public static EnemyBase SpawnEnemy(EnemyData enemyData, Vector3 position, Transform parent = null)
        {
            var basePrefab = Resources.Load<GameObject>("EnemyBase");

            // 변경된 코드
            if (basePrefab == null)
            {
                Debug.LogError("EnemyBase 프리팹이 Resources/EnemyBase 에 없음");
                return null;
            }
            
            var instance = Object.Instantiate(basePrefab, position, Quaternion.identity, parent);

            if (instance.TryGetComponent<EnemyBase>(out var enemy))
            {
                return enemy.Init(enemyData);
            }
            
            Debug.LogError("EnemyBase 컴포넌트를 찾을 수 없음");
            return null;
        }
    }

    public abstract class EnemyBase : MonoBehaviour
    {
        [SerializeField] Transform m_visualHolder;
        [SerializeField] private EnemyData m_enemyData;
        public int health;
        float lastAttack;
        public EnemyData GetEnemyData => m_enemyData;
        public EnemyBase Init(EnemyData data)
        {
            m_enemyData = data;
            gameObject.name = data.enemyName;
            health = data.health;
            lastAttack = 0;
            /*
            var visual = Resources.Load<Transform>($"{data.visualResourceID}");

            visual.SetParent(visualHolder);
            visual.transform.localPosition = Vector3.zero;
            */
            
            return this;    
        }

        private void Update()
        {
            var player = GamePlayManager.Instance.gamePlayLogic.m_Player;
            if(Vector3.Distance(transform.position, player.transform.position) <= m_enemyData.attackRange)
            {
                if (Time.time >= lastAttack + m_enemyData.attackCooldown)
                    Attack();
            }
            else
            {
                transform.Translate(player.transform.position);
            }
        }

        public void TakeDamage(int damage)
        {
            damage -= m_enemyData.resistance;
            
            health -= damage;

            if(health <= 0)
            {
                Die();
            }
        }

        void Attack()
        {
            // 공격
            // 아마 오브젝트 발사
            lastAttack = Time.time;
            // GameEventBus.Publish<HitPlayer>(new HitPlayer { enemyData = m_enemyData });
        }

        void Die()
        {
            GameEventBus.Publish<PlayerEXPAdded>(new PlayerEXPAdded
            {
                amount = m_enemyData.experienceGiven
            });
        }
    }
}




