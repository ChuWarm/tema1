using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Script.Core;

namespace Script.Characters
{
    public static class EnemyFactory
    {
        public static Enemy SpawnEnemy(EnemyData enemyData, Vector3 position, Transform parent = null, RoomEventProcessor roomProcessor = null)
        {
            var basePrefab = Resources.Load<GameObject>("EnemyBase");
            if (basePrefab == null)
            {
                Debug.LogError("[적] EnemyBase 프리팹이 Resources/EnemyBase에 없음");
                return null;
            }
            
            var instance = Object.Instantiate(basePrefab, position, Quaternion.identity, parent);
            if (instance.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.SetRoomProcessor(roomProcessor);
                return enemy.Init(enemyData);
            }
            
            Debug.LogError("[적] Enemy 컴포넌트를 찾을 수 없음");
            return null;
        }
    }

    public class Enemy : MonoBehaviour, IDamageable, IAttacker
    {
        [Header("시각적 요소")]
        [SerializeField] private Transform visualHolder;
        [SerializeField] private Animator animator;

        [Header("적 데이터")]
        [SerializeField] private string enemyID;
        private EnemyData enemyData;
        private bool isInitialized;
        private bool isDead;

        [Header("스탯")]
        private int currentHealth;
        private float lastAttackTime;

        [Header("감지 및 추적")]
        [SerializeField] private float detectionRange = 10f;
        [SerializeField] private float attackAngle = 45f;
        [SerializeField] private float rotationSpeed = 5f;
        private Vector3 lastKnownPosition;
        private bool isPlayerDetected;
        private float searchTimer;
        private float maxSearchTime = 5f;

        [Header("애니메이션")]
        private static readonly int IsWalkingAnim = Animator.StringToHash("IsWalking");
        private static readonly int AttackAnim = Animator.StringToHash("Attack");
        private static readonly int HitAnim = Animator.StringToHash("Hit");
        private static readonly int DieAnim = Animator.StringToHash("Dead");
        private static readonly int IdleAnim = Animator.StringToHash("Idle");

        [System.Serializable]
        public class EnemyDeathEvent : UnityEvent<Enemy> { }
        public EnemyDeathEvent OnEnemyDeath = new EnemyDeathEvent();

        private RoomEventProcessor _roomProcessor;

        public EnemyData GetEnemyData => enemyData;

        private void Start()
        {
            StartCoroutine(InitializeEnemy());
        }

        private IEnumerator InitializeEnemy()
        {
            while (!DataManager.IsReady || PlayerManager.Instance == null)
            {
                yield return null;
            }

            if (!string.IsNullOrEmpty(enemyID))
            {
                var data = DataManager.GetData<EnemyData>(enemyID);
                if (data != null)
                {
                    Init(data);
                }
                else
                {
                    Debug.LogError($"[적] {gameObject.name}: ID {enemyID}에 대한 데이터를 찾을 수 없음");
                }
            }
            else
            {
                Debug.LogError($"[적] {gameObject.name}: enemyID가 설정되지 않음");
            }
        }

        public Enemy Init(EnemyData data)
        {
            enemyData = data;
            gameObject.name = data.enemyName;
            currentHealth = data.health;
            lastAttackTime = 0;
            isInitialized = true;

            // 시각적 요소 설정
            if (visualHolder != null && !string.IsNullOrEmpty(data.visualResourceID))
            {
                var visual = Resources.Load<Transform>(data.visualResourceID);
                if (visual != null)
                {
                    Instantiate(visual, visualHolder).localPosition = Vector3.zero;
                }
            }

            return this;
        }

        public void SetRoomProcessor(RoomEventProcessor processor)
        {
            _roomProcessor = processor;
        }

        private void Update()
        {
            if (!isInitialized || isDead || PlayerManager.Instance == null) return;
            
            var playerTransform = PlayerManager.Instance.transform;
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            // 플레이어 감지
            if (distanceToPlayer <= detectionRange)
            {
                if (angleToPlayer <= attackAngle * 2)
                {
                    if (Physics.Raycast(transform.position, directionToPlayer, out var hit, detectionRange) 
                        && hit.transform == playerTransform)
                    {
                        isPlayerDetected = true;
                        lastKnownPosition = playerTransform.position;
                        searchTimer = 0f;
                    }
                }
            }

            // 플레이어 추적 및 공격
            if (isPlayerDetected)
            {
                if (distanceToPlayer <= detectionRange)
                {
                    // 플레이어를 향해 회전
                    Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                    if (distanceToPlayer <= 15f) // enemyData.attackRange)
                    {
                        if (angleToPlayer <= attackAngle && CanAttack())
                        {
                            Attack(PlayerManager.Instance);
                        }
                        SetWalkingAnimation(false);
                    }
                    else
                    {
                        // 플레이어에게 이동
                        transform.position += enemyData.moveSpeed * Time.deltaTime * transform.forward;
                        SetWalkingAnimation(true);
                    }
                }
                else
                {
                    // 마지막 위치 추적
                    searchTimer += Time.deltaTime;
                    if (searchTimer < maxSearchTime)
                    {
                        Vector3 directionToLastKnown = (lastKnownPosition - transform.position).normalized;
                        transform.rotation = Quaternion.Slerp(transform.rotation, 
                            Quaternion.LookRotation(directionToLastKnown), rotationSpeed * Time.deltaTime);
                        transform.position += enemyData.moveSpeed * Time.deltaTime * transform.forward;
                        SetWalkingAnimation(true);
                    }
                    else
                    {
                        isPlayerDetected = false;
                        SetWalkingAnimation(false);
                    }
                }
            }
            else
            {
                SetWalkingAnimation(false);
            }
        }

        private void SetWalkingAnimation(bool isWalking)
        {
            if (animator != null)
            {
                bool currentState = animator.GetBool(IsWalkingAnim);
                if (currentState != isWalking)
                {
                    Debug.Log($"[Enemy: {gameObject.name}] SetWalkingAnimation: current IsWalking = {currentState}, new IsWalking = {isWalking}");
                }
                animator.SetBool(IsWalkingAnim, isWalking);
            }
        }

        public void TakeDamage(int damage)
        {
            if (isDead) return;

            damage -= enemyData.resistance;
            currentHealth -= damage;
            
            if (animator != null)
            {
                animator.SetTrigger(HitAnim);
            }

            // 데미지 이펙트
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
            if (animator != null)
            {
                Debug.Log("죽음 애니메이션 실행");
                animator.ResetTrigger(HitAnim);
                animator.SetTrigger(DieAnim);
            }

            OnEnemyDeath?.Invoke(this);
            
            // 죽음 이펙트
            EffectManager.Instance.PlayEffect("Death", transform.position, Quaternion.identity);
            
            // 경험치 지급
            PlayerManager.Instance.GainExperience(enemyData.experienceGiven);
            EffectManager.Instance.ShowExpText(enemyData.experienceGiven, transform.position + Vector3.up);
            
            // 이벤트 발행
            if (_roomProcessor != null)
            {
                GameEventBus.Publish(new RoomEnemyDeadEvent { sender = _roomProcessor, enemy = this });
                Debug.Log($"[{_roomProcessor.GetRoom().gameObject.name}] Enemy '{gameObject.name}' died. Published RoomEnemyDeadEvent with sender: {_roomProcessor.GetInstanceID()}");
            }
            else
            {
                Debug.LogError($"[{gameObject.name}] Enemy.HandleDeath - _roomProcessor is null! Publishing RoomEnemyDeadEvent without sender.");
                GameEventBus.Publish(new RoomEnemyDeadEvent { enemy = this }); 
            }
            
            Destroy(gameObject, 3.5f);
        }

        public void Attack(IDamageable target)
        {
            if (!CanAttack()) return;

            lastAttackTime = Time.time;
            if (animator != null)
            {
                animator.SetTrigger(AttackAnim);
            }
            
            // 공격 이펙트
            EffectManager.Instance.PlayEffect("Attack", transform.position + transform.forward, transform.rotation);
            
            // 딜레이 후 데미지 적용 
            // StartCoroutine(DelayedDamage(target));
        }

        private IEnumerator DelayedDamage(IDamageable target)
        {
            yield return new WaitForSeconds(1f);
            target.TakeDamage(enemyData.attackPower);
        }

        public bool CanAttack()
        {
            return Time.time >= lastAttackTime + enemyData.attackCooldown;
        }

        public float GetAttackRange() => enemyData.attackRange;
        public int GetAttackDamage() => enemyData.attackPower;
        public bool IsDead() => isDead;
        public Transform GetTransform() => transform;

        private void OnDrawGizmosSelected()
        {
            // 감지 범위 시각화
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // 공격 범위 시각화
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyData?.attackRange ?? 0f);
        }

        public void TriggerHit()
        {
            float hitRadius = 2f;
            Vector3 hitOrigin = transform.position + transform.forward * 1.5f;
            Collider[] hits = Physics.OverlapSphere(hitOrigin, hitRadius, LayerMask.GetMask("Player"));

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<PlayerManager>(out var player))
                {
                    player.TakeDamage(enemyData.attackPower);
                    Debug.Log($"[Enemy] 플레이어 적중: {player.name}");
                }
            }
            
            EffectManager.Instance.PlayEffect("Hit", hitOrigin, Quaternion.identity);
        }
    }
} 