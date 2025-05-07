using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Script.Core;

namespace Script.Characters
{
    public class Enemy : MonoBehaviour, IDamageable, IAttacker
    {
        [Header("기본 스탯")]
        public int maxHealth = 100;
        public int currentHealth;
        public int attackDamage = 10;
        public float attackRange = 2f;
        public float attackCooldown = 1f;
        public float moveSpeed = 3f;

        [Header("전투 설정")]
        public float detectionRange = 10f;
        public float attackAngle = 45f;

        [Header("보상 설정")]
        public int expReward = 10;
        public ItemDropData[] possibleDrops;
        public float dropChance = 0.3f;

        [Header("애니메이션")]
        public Animator animator;
        private static readonly int IsWalkingAnim = Animator.StringToHash("IsWalking");
        private static readonly int AttackAnim = Animator.StringToHash("Attack");
        private static readonly int HitAnim = Animator.StringToHash("Hit");
        private static readonly int DieAnim = Animator.StringToHash("Die");

        private float lastAttackTime;
        private Transform playerTransform;
        private bool isDead;

        [System.Serializable]
        public class EnemyDeathEvent : UnityEvent<Enemy> { }
        public EnemyDeathEvent OnEnemyDeath = new EnemyDeathEvent();

        private void Start()
        {
            currentHealth = maxHealth;
            playerTransform = PlayerManager.Instance.transform;
        }

        private void Update()
        {
            if (isDead) return;

            // 플레이어 감지 및 추적
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            
            if (distanceToPlayer <= detectionRange)
            {
                // 플레이어를 향해 회전
                Vector3 direction = (playerTransform.position - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(direction);

                if (distanceToPlayer <= attackRange)
                {
                    // 공격 가능한 각도인지 확인
                    float angle = Vector3.Angle(transform.forward, direction);
                    if (angle <= attackAngle && CanAttack())
                    {
                        PerformAttack(PlayerManager.Instance);
                    }
                    animator?.SetBool(IsWalkingAnim, false);
                }
                else
                {
                    // 플레이어에게 이동
                    transform.position += moveSpeed * Time.deltaTime * transform.forward;
                    animator?.SetBool(IsWalkingAnim, true);
                }
            }
            else
            {
                animator?.SetBool(IsWalkingAnim, false);
            }
        }

        public void TakeDamage(int damage)
        {
            if (isDead) return;

            currentHealth -= damage;
            animator?.SetTrigger(HitAnim);
            
            // 데미지 이펙트와 텍스트
            EffectManager.Instance.PlayEffect("Hit", transform.position, Quaternion.identity);
            EffectManager.Instance.ShowDamageText(damage, transform.position + Vector3.up);

            if (currentHealth <= 0)
            {
                HandleDeath();
            }
        }

        private void HandleDeath()
        {
            isDead = true;
            animator?.SetTrigger(DieAnim);
            OnEnemyDeath?.Invoke(this);
            
            // 죽음 이펙트
            EffectManager.Instance.PlayEffect("Death", transform.position, Quaternion.identity);
            
            // 경험치 지급
            PlayerManager.Instance.GainExperience(expReward);
            EffectManager.Instance.ShowExpText(expReward, transform.position + Vector3.up);
            
            // 아이템 드롭
            DropItems();
            
            Destroy(gameObject, 2f);
        }

        private void DropItems()
        {
            if (Random.value > dropChance) return;

            foreach (var dropData in possibleDrops)
            {
                if (Random.value <= dropData.dropChance)
                {
                    Vector3 dropPosition = transform.position + Random.insideUnitSphere * 0.5f;
                    dropPosition.y = transform.position.y;
                    Instantiate(dropData.itemPrefab, dropPosition, Quaternion.identity);
                }
            }
        }

        public bool IsDead()
        {
            return isDead;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public void Attack(IDamageable target)
        {
            PerformAttack(target);
        }

        private void PerformAttack(IDamageable target)
        {
            if (!CanAttack()) return;

            lastAttackTime = Time.time;
            animator?.SetTrigger(AttackAnim);
            
            // 공격 이펙트
            EffectManager.Instance.PlayEffect("Attack", transform.position + transform.forward, transform.rotation);
            
            // 약간의 딜레이 후 데미지 적용
            StartCoroutine(DelayedDamage(target));
        }

        private IEnumerator DelayedDamage(IDamageable target)
        {
            yield return new WaitForSeconds(0.3f); // 애니메이션 타이밍에 맞춰 조정
            target.TakeDamage(attackDamage);
        }

        public bool CanAttack()
        {
            return Time.time >= lastAttackTime + attackCooldown;
        }

        public float GetAttackRange()
        {
            return attackRange;
        }

        public int GetAttackDamage()
        {
            return attackDamage;
        }

        private void OnDrawGizmosSelected()
        {
            // 감지 범위 시각화
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // 공격 범위 시각화
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }

    [System.Serializable]
    public class ItemDropData
    {
        public GameObject itemPrefab;
        public float dropChance = 0.1f;
    }
} 