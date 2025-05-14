using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Script.Core;
using Script.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using TMPro;

namespace Script.Characters
{
    public enum PlayerState { None, Idle, Move, Attack, Hit, Dash, Spawn }
    public enum WeaponType { Sword = 0, Gun = 1 }

    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("이동 설정")] public float moveSpeed = 5f;
        public float rotationSpeed = 10f;
        public float jumpForce = 5f;
        public float gravity = -9.81f;

        [Header("상태 체크")] public bool isGrounded;
        public LayerMask groundLayer;

        [Header("공격 설정")]
        public float attackRange = 1.5f;
        public float attackAngle = 90f;
        public LayerMask enemyLayer;

        [Header("참조")] public PlayerManager playerManager;
        public CharacterController characterController;
        public Animator animator;

        private Vector3 _moveDirection;
        private float _verticalVelocity;
        private float _attackMoveSpeed = 15f;
        private float _attackStepDuration = 1f;
        private float _attackStepElapsed = 0f;
        public static readonly int Idle = Animator.StringToHash("Idle");
        public static readonly int IsWalkingAnim = Animator.StringToHash("IsRun");
        public static readonly int SpawnAnim = Animator.StringToHash("Spawn");
        public static readonly int DashAnim = Animator.StringToHash("Dash");
        public static readonly int AttackAnim = Animator.StringToHash("Attack");
        private static readonly int JumpAnim = Animator.StringToHash("Jump");
        private static readonly int DeadAnim = Animator.StringToHash("Dead");

        private PlayerStateIdle _playerStateIdle;
        private PlayerStateMove _playerStateMove;
        private PlayerStateAttack _playerStateAttack;
        private PlayerStateHit _playerStateHit;
        private PlayerStateDash _playerStateDash;
        private PlayerStateSpawn _playerStateSpawn;

        private Dictionary<PlayerState, IPlayerState> _playerStates;
        private Dictionary<WeaponType, IPlayerAttackBehavior> _attackBehaviors;

        private IPlayerAttackBehavior _currentAttackBehavior;
        public IPlayerAttackBehavior GetAttackBehavior() => _currentAttackBehavior;

        public bool IsRun { get; set; } = true;
        public bool IsAttacking { get; set; }
        public bool IsAttackMoving { get; set; }


        public PlayerState CurrentState { get; private set; }
        public WeaponType CurrentWeapon { get; private set; } = WeaponType.Sword;

        [Header("공격 판정 상세 설정")]
        [Tooltip("애니메이션 이벤트 후 공격 판정을 지속할 시간 (초)")]
        public float hitCheckDuration = 0.1f; 
        private bool _isDuringHitCheck = false; // 현재 공격 판정 지속 시간 중인지 여부
        private HashSet<IDamageable> _alreadyHitTargetsInCurrentAttack; // 현재 공격 모션에서 이미 맞은 타겟들

        public float _forwardThrustAmount = 1f;

        [Header("UI References")] // (선택 사항) 인스펙터에서 구분하기 쉽게 Header 추가
        public GameObject damageTextPrefab; // DamageTextPrefab을 Inspector에서 연결
        public Canvas canvas; // 데미지 텍스트를 생성할 Canvas를 Inspector에서 연결

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            if (animator == null)
                animator = GetComponent<Animator>();
            _alreadyHitTargetsInCurrentAttack = new HashSet<IDamageable>();
        }

        private void Start()
        {
            playerManager = PlayerManager.Instance;
            _playerStateIdle = new PlayerStateIdle();
            _playerStateMove = new PlayerStateMove();
            _playerStateAttack = new PlayerStateAttack();
            _playerStateHit = new PlayerStateHit();
            _playerStateDash = new PlayerStateDash();
            _playerStateSpawn = new PlayerStateSpawn();

            _playerStates = new Dictionary<PlayerState, IPlayerState>()
            {
                { PlayerState.Idle, _playerStateIdle },
                { PlayerState.Move, _playerStateMove },
                { PlayerState.Attack, _playerStateAttack },
                { PlayerState.Hit, _playerStateHit },
                { PlayerState.Dash, _playerStateDash },
                { PlayerState.Spawn, _playerStateSpawn },
            };

            _attackBehaviors = new()
            {
                { WeaponType.Sword, new SwordAttack() },
                { WeaponType.Gun, new GunAttack() }
            };
            _currentAttackBehavior = _attackBehaviors[CurrentWeapon];

            Init();
        }

        private void Update()
        {
            // HandleMovement();
            // HandleJump();
            HandleAttack();
            ApplyGravity();

            if (CurrentState != PlayerState.None)
            {
                _playerStates[CurrentState].UpdateState();
            }

            AttackStep();
        }

        private void Init()
        {
            SetState(PlayerState.Spawn);
        }

        public void SetState(PlayerState state)
        {
            if (CurrentState != PlayerState.None)
            {
                _playerStates[CurrentState].ExitState();
            }

            CurrentState = state;
            _playerStates[CurrentState].EnterState(this);
        }

        public void HandleMovement()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized;

            if (movement.magnitude >= 0.1f)
            {
                // 이동 방향으로 회전
                float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSpeed, 0.1f);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                // 이동
                _moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                characterController.Move(moveSpeed * Time.deltaTime * _moveDirection);

                animator?.SetBool(IsWalkingAnim, true);
            }
            else
            {
                animator?.SetBool(IsWalkingAnim, false);
            }
        }

        private void HandleJump()
        {
            isGrounded = characterController.isGrounded;

            if (isGrounded && _verticalVelocity < 0)
            {
                _verticalVelocity = -2f;
            }

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                _verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
                animator?.SetTrigger(JumpAnim);
            }
        }

        private void HandleAttack()
        {

            if (Input.GetMouseButtonDown(0))
            {

            }
        }

        private void ApplyGravity()
        {
            _verticalVelocity += gravity * Time.deltaTime;
            characterController.Move(new Vector3(0, _verticalVelocity, 0) * Time.deltaTime);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            if (this.attackRange > 0)
            {
                Vector3 yOffset = Vector3.up * 1.0f;
                Vector3 sphereCenter = transform.position + yOffset + transform.forward * (this.attackRange * 0.5f);
                float sphereRadius = this.attackRange; 
                Gizmos.DrawWireSphere(sphereCenter, sphereRadius);
            }
        }

        public void SetWeapon(WeaponType type)
        {
            CurrentWeapon = type;
            animator.SetInteger("WeaponType", (int)type);
            _currentAttackBehavior = _attackBehaviors[type];
        }
        private void AttackStep()
        {
            if (IsAttackMoving && CurrentState == PlayerState.Attack)
            {
                _attackStepElapsed += Time.deltaTime;

                if (_attackStepElapsed < _attackStepDuration)
                {
                    characterController.Move(transform.forward * (_attackMoveSpeed * Time.deltaTime));
                }
                else
                {
                    IsAttackMoving = false;
                    _attackStepElapsed = 0f;
                }
            }
        }

        private void LookAtMouse()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, transform.position);

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 point = ray.GetPoint(distance);
                Vector3 direction = point - transform.position;
                direction.y = 0f;

                if (direction.sqrMagnitude > 0.01f)
                {
                    transform.forward = direction.normalized;
                }
            }
        }

        public void TriggerAttack()
        {
            animator.SetBool(IsWalkingAnim, false);
            animator.speed = 1.6f;
            animator.SetTrigger(AttackAnim);
        }

        public void TriggerDash()
        {
            if (CurrentState == PlayerState.Dash) return;

            animator.speed = 1f;
            SetState(PlayerState.Dash);
        }
        
        public void IsPlayerDead()
        {
            if (PlayerManager.Instance.currentHealth <= 0)
                animator.Play(DeadAnim);
        }

        
        #region 애니메이션 이벤트

        public void AttackStart()
        {
            LookAtMouse();
            IsRun = false;
            IsAttacking = true;
            _alreadyHitTargetsInCurrentAttack.Clear();
            _isDuringHitCheck = false;
            StopCoroutine("HitCheckCoroutine");

            // AttackStep() 활성화를 위한 플래그 설정 및 타이머 초기화
            IsAttackMoving = true;
            _attackStepElapsed = 0f;
        }

        public void AttackMoveStep()
        {
            Debug.Log("[PlayerController] AttackMoveStep() Animation Event Fired!");
            if (!_isDuringHitCheck && gameObject.activeInHierarchy && enabled)
            {
                StartCoroutine(HitCheckCoroutine());
            }
        }
        
        private IEnumerator HitCheckCoroutine()
        {
            _isDuringHitCheck = true;
            float timer = 0f;

            while (timer < hitCheckDuration)
            {
                PerformOverlapSphere();
                timer += Time.deltaTime;
                yield return null;
            }
            _isDuringHitCheck = false;
        }

        void PerformOverlapSphere()
        {
            if (playerManager == null)
            {
                Debug.LogError("[PlayerController] PlayerManager 참조가 null입니다!");
                return;
            }

            Vector3 yOffset = Vector3.up * 1.0f;
            Vector3 detectionCenter = transform.position + yOffset + transform.forward * (attackRange * 0.5f);
            float detectionRadius = attackRange; 

            Collider[] hitColliders = Physics.OverlapSphere(detectionCenter, detectionRadius, enemyLayer);
            
            if (hitColliders.Length > 0)
            {
            }

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject == gameObject) continue;

                IDamageable damageable = hitCollider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    if (!_alreadyHitTargetsInCurrentAttack.Contains(damageable))
                    {
                        int damageDealt = playerManager.attackPower; // (선택 사항) 데미지 값을 변수에 저장
                        playerManager.Attack(damageable);
                        _alreadyHitTargetsInCurrentAttack.Add(damageable);

                        Debug.Log($"[PlayerController] {hitCollider.name} 에게 {damageDealt} 데미지로 공격 실행 완료."); // (선택 사항) 로그에 저장된 데미지 사용

                        // --- 데미지 텍스트 생성 및 초기화 시작 ---
                        if (damageTextPrefab != null)
                        {
                            try
                            {
                                // 적의 위치를 기준으로 데미지 텍스트 생성 (적의 머리 위에 표시하기 위해 y 오프셋 추가)
                                Vector3 spawnPosition = hitCollider.gameObject.transform.position + Vector3.up * 1.5f;
                                
                                // 부모 없이 생성 - 프리팹 인스턴스 생성
                                GameObject damageTextInstance = null;
                                
#if UNITY_EDITOR
                                // 프리팹이 에셋인지 확인하여 처리 방식 변경
                                if (PrefabUtility.IsPartOfPrefabAsset(damageTextPrefab))
                                {
                                    // 프리팹 에셋에서 복제하려면 임시 게임오브젝트를 생성하고 TextMeshPro 등 필요한 컴포넌트를 추가
                                    damageTextInstance = new GameObject("DamageText");
                                    
                                    // TextMeshPro 컴포넌트 추가
                                    TextMeshPro textMesh = damageTextInstance.AddComponent<TextMeshPro>();
                                    textMesh.text = damageDealt.ToString();
                                    textMesh.color = Color.red;
                                    textMesh.fontSize = 3;
                                    textMesh.alignment = TextAlignmentOptions.Center;
                                    
                                    // 위치 설정
                                    damageTextInstance.transform.position = spawnPosition;
                                    
                                    // DamageText 컴포넌트 추가
                                    DamageText damageTextComp = damageTextInstance.AddComponent<DamageText>();
                                }
                                else
                                {
                                    // 일반 프리팹인 경우 정상적으로 Instantiate
                                    damageTextInstance = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity);
                                }
#else
                                // 빌드 환경에서는 항상 Instantiate 사용
                                try {
                                    damageTextInstance = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity);
                                }
                                catch (Exception e) {
                                    // 인스턴스화 실패 시 수동으로 생성
                                    damageTextInstance = new GameObject("DamageText");
                                    
                                    // TextMeshPro 컴포넌트 추가
                                    TextMeshPro textMesh = damageTextInstance.AddComponent<TextMeshPro>();
                                    textMesh.text = damageDealt.ToString();
                                    textMesh.color = Color.red;
                                    textMesh.fontSize = 3;
                                    textMesh.alignment = TextAlignmentOptions.Center;
                                    
                                    // 위치 설정
                                    damageTextInstance.transform.position = spawnPosition;
                                    
                                    // DamageText 컴포넌트 추가
                                    DamageText damageTextComp = damageTextInstance.AddComponent<DamageText>();
                                }
#endif
                                
                                // 캔버스에 UI 요소를 추가할 때는 월드 좌표를 스크린 좌표로 변환해야 합니다
                                if (canvas != null && Camera.main != null)
                                {
                                    try
                                    {
                                        // 오브젝트의 RectTransform 컴포넌트 가져오기
                                        RectTransform rectTransform = damageTextInstance.GetComponent<RectTransform>();
                                        if (rectTransform != null)
                                        {
                                            // 월드 좌표를 스크린 좌표로 변환
                                            Vector2 screenPos = Camera.main.WorldToScreenPoint(spawnPosition);
                                            
                                            // Screen Space - Overlay 모드에서는 worldCamera를 null로 전달
                                            Vector2 localPos;
                                            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                                                canvas.GetComponent<RectTransform>(),
                                                screenPos,
                                                null, // Screen Space - Overlay 모드에서는 null 사용
                                                out localPos);
                                            
                                            // UI 요소를 캔버스에 자식으로 추가하는 대신 위치만 직접 설정
                                            // damageTextInstance.transform.SetParent(canvas.transform, false); // 이 부분이 오류를 발생시킴
                                            
                                            // TextMeshPro는 월드 스페이스에서도 작동할 수 있으므로 포지션만 설정
                                            rectTransform.position = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));
                                        }
                                        else
                                        {
                                            Debug.LogError("[PlayerController] DamageText 프리팹에 RectTransform 컴포넌트가 없습니다!");
                                        }
                                    }
                                    catch (System.Exception e)
                                    {
                                        Debug.LogError($"[PlayerController] 데미지 텍스트 위치 설정 중 오류 발생: {e.Message}");
                                    }
                                }
                                else
                                {
                                    Debug.LogWarning("[PlayerController] Canvas 또는 Camera.main이 null입니다!");
                                }
                                
                                // DamageText 컴포넌트 초기화
                                DamageText damageTextComponent = damageTextInstance.GetComponent<DamageText>();
                                if (damageTextComponent != null)
                                {
                                    damageTextComponent.Initialize(damageDealt, Color.red);
                                }
                                else
                                {
                                    Debug.LogError("[PlayerController] 생성된 DamageTextPrefab 인스턴스에 DamageText 컴포넌트가 없습니다!");
                                    // DamageText 컴포넌트 추가 시도
                                    damageTextComponent = damageTextInstance.AddComponent<DamageText>();
                                    if (damageTextComponent != null)
                                    {
                                        Debug.Log("[PlayerController] DamageText 컴포넌트를 동적으로 추가했습니다.");
                                        damageTextComponent.Initialize(damageDealt, Color.red);
                                    }
                                }
                            }
                            catch (System.Exception e)
                            {
                                Debug.LogError($"[PlayerController] 데미지 텍스트 생성 중 오류 발생: {e.Message}");
                            }
                        }
                        else
                        {
                            Debug.LogWarning("[PlayerController] DamageTextPrefab이 Inspector에 연결되지 않았습니다.");
                        }
                        // --- 데미지 텍스트 생성 및 초기화 끝 ---
                    }
                }
            }
        }

        public void AttackMoveStepEnd()
        {
            IsAttackMoving = false;
        }

        public void AttackEnd()
        {
            Debug.Log("[PlayerController] AttackEnd() Animation Event Fired!");
            animator.speed = 1f;
            IsRun = true;
            IsAttacking = false;
            _isDuringHitCheck = false;
            StopCoroutine("HitCheckCoroutine");

            if (CurrentState == PlayerState.Attack)
            {
                SetState(PlayerState.Idle);
            }
            else
            {
                Debug.LogWarning($"[PlayerController] AttackEnd: Current state was {CurrentState}, not Attack. State not changed to Idle by AttackEnd.");
            }
        }

        public void OnSpawnAnimationComplete()
        {
            SetState(PlayerState.Idle);
        }
        
        #endregion
    }
}
