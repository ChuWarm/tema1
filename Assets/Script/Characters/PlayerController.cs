using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Script.Core;

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

        [Header("참조")] public PlayerManager playerManager;
        public CharacterController characterController;
        public Animator animator;

        private Vector3 _moveDirection;
        private float _verticalVelocity;
        private float _attackMoveSpeed = 15f;
        private float _attackStepDuration = 0.15f;
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

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            if (animator == null)
                animator = GetComponent<Animator>();
        }

        private void Start()
        {
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
                // animator?.SetTrigger(AttackAnim);
                // 공격 로직은 PlayerManager에서 처리
            }
        }

        private void ApplyGravity()
        {
            _verticalVelocity += gravity * Time.deltaTime;
            characterController.Move(new Vector3(0, _verticalVelocity, 0) * Time.deltaTime);
        }

        private void OnDrawGizmosSelected()
        {
            // 지면 체크 범위 시각화
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.2f);
        }

        public void SetWeapon(WeaponType type)
        {
            CurrentWeapon = type;
            animator.SetInteger("WeaponType", (int)type);
            _currentAttackBehavior = _attackBehaviors[type];
        }

        // public void Move(Vector3 inputDirection, float speed)
        // {
        //     if (CurrentState == PlayerState.Attack && CurrentWeapon == WeaponType.Sword)
        //         return;
        //
        //     inputDirection.Normalize();
        //
        //     Vector3 cameraForward = Camera.main.transform.forward;
        //     Vector3 cameraRight = Camera.main.transform.right;
        //
        //     cameraForward.y = 0;
        //     cameraRight.y = 0;
        //
        //     cameraForward.Normalize();
        //     cameraRight.Normalize();
        //
        //     Vector3 moveDirection = cameraForward * inputDirection.z
        //                             + cameraRight * inputDirection.x;
        //
        //     if (moveDirection.magnitude > 0.1f)
        //     {
        //         if (currentLookMode == LookMode.Movement)
        //             transform.rotation = Quaternion.LookRotation(moveDirection);
        //
        //         characterController.Move(moveDirection * (moveSpeed * Time.deltaTime));
        //     }
        // }

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
            if (Physics.Raycast(ray, out var hit, 300f))
            {
                Vector3 lookDirection = hit.point - transform.position;
                lookDirection.y = 0f;

                if (lookDirection.sqrMagnitude > 0.01f)
                {
                    transform.forward = lookDirection.normalized;
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
        }

        public void AttackMoveStep()
        {
            IsAttackMoving = true;
            _attackStepElapsed = 0f;
        }

        public void AttackMoveStepEnd()
        {
            IsAttackMoving = false;
        }

        public void AttackEnd()
        {
            animator.speed = 1f;
            IsRun = true;
            IsAttacking = false;
        }

        public void OnSpawnAnimationComplete()
        {
            SetState(PlayerState.Idle);
        }
        
        #endregion
    }
}
