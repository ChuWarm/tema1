using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Script.Core;

namespace Script.Characters
{
    public class Enemy : MonoBehaviour, IDamageable, IAttacker
    {
        [Header("Enemy ID")]
        public string enemyID; // Inspector에서 설정할 적 ID

        [Header("기본 스탯")]
        private int health;
        private int currentHealth;
        private int attackPower;
        private float attackRange;
        private float attackCooldown;
        [SerializeField] private float moveSpeed;

        [Header("감지 및 추적")]
        [SerializeField]private float detectionRange = 10f;  // 플레이어 감지 범위
        [SerializeField]private float attackAngle = 45f;     // 공격 가능한 각도
        [SerializeField]private float rotationSpeed = 5f;    // 회전 속도
        private Vector3 lastKnownPosition;   // 마지막으로 발견한 플레이어 위치
        private bool isPlayerDetected;       // 플레이어 감지 상태
        private float searchTimer;           // 플레이어 추적 타이머
        private float maxSearchTime = 5f;    // 최대 추적 시간

        [Header("보상 설정")]
        private int experienceGiven;

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
            LoadEnemyData();
            currentHealth = health;
            playerTransform = PlayerManager.Instance.transform;
        }

        private void LoadEnemyData()
        {
            if (string.IsNullOrEmpty(enemyID))
            {
                Debug.LogError($"Enemy {gameObject.name} has no enemyID set!");
                return;
            }

            var enemyData = DataManager.GetData<EnemyData>(enemyID);
            if (enemyData == null)
            {
                Debug.LogError($"Failed to load enemy data for ID: {enemyID}");
                return;
            }

            // 스탯 로드
            health = enemyData.health;
            attackPower = enemyData.attackPower;
            attackRange = enemyData.attackRange;
            attackCooldown = enemyData.attackCooldown;
            moveSpeed = enemyData.moveSpeed;
            experienceGiven = enemyData.experienceGiven;

            Debug.Log($"Loaded enemy data for {enemyData.enemyName} (ID: {enemyID})");
        }

        private void Update()
        {
            if (isDead) return;

            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            // 플레이어 감지 로직
            if (distanceToPlayer <= detectionRange)
            {
                // 시야각 체크
                if (angleToPlayer <= attackAngle * 2)
                {
                    // 레이캐스트로 장애물 체크
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRange))
                    {
                        if (hit.transform == playerTransform)
                        {
                            isPlayerDetected = true;
                            lastKnownPosition = playerTransform.position;
                            searchTimer = 0f;
                        }
                    }
                }
            }

            // 플레이어 추적 및 공격 로직
            if (isPlayerDetected)
            {
                if (distanceToPlayer <= detectionRange)
                {
                    // 플레이어를 향해 부드럽게 회전
                    Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                    if (distanceToPlayer <= attackRange)
                    {
                        // 공격 가능한 각도인지 확인
                        if (angleToPlayer <= attackAngle && CanAttack())
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
                    // 플레이어가 감지 범위를 벗어나면 마지막 위치로 이동
                    searchTimer += Time.deltaTime;
                    if (searchTimer < maxSearchTime)
                    {
                        Vector3 directionToLastKnown = (lastKnownPosition - transform.position).normalized;
                        transform.rotation = Quaternion.Slerp(transform.rotation, 
                            Quaternion.LookRotation(directionToLastKnown), rotationSpeed * Time.deltaTime);
                        transform.position += moveSpeed * Time.deltaTime * transform.forward;
                        animator?.SetBool(IsWalkingAnim, true);
                    }
                    else
                    {
                        isPlayerDetected = false;
                        animator?.SetBool(IsWalkingAnim, false);
                    }
                }
            }
            else
            {
                animator?.SetBool(IsWalkingAnim, false);
            }
        }
        
        //ToDo 에너미 타입 클릭으로 설정

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
            PlayerManager.Instance.GainExperience(experienceGiven);
            EffectManager.Instance.ShowExpText(experienceGiven, transform.position + Vector3.up);
            
            
            Destroy(gameObject, 2f);
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
            target.TakeDamage(attackPower);
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
            return attackPower;
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
} 