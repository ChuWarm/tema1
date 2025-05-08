using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public enum PlayerState { None, Idle, Move, Attack, Hit }
public enum LookMode { None, Movement, Mouse }
public enum WeaponType { Sword = 0, Gun = 1 }

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private const float gravity = -9.81f;
    private static readonly int Attack = Animator.StringToHash("Attack");

    private Dictionary<PlayerState, IPlayerState> _playerStates;
    private Dictionary<WeaponType, IPlayerAttackBehavior> _attackBehaviors;
    private PlayerStateIdle _playerStateIdle;
    private PlayerStateMove _playerStateMove;
    private PlayerStateAttack _playerStateAttack;
    private PlayerStateHit _playerStateHit;
    private Vector3 _velocity;
    private IPlayerAttackBehavior _currentAttackBehavior;
    public IPlayerAttackBehavior GetAttackBehavior() => _currentAttackBehavior;
    
    public PlayerState CurrentState { get; private set; }
    public Animator Animator { get; private set; }
    public WeaponType CurrentWeapon { get; private set; } = WeaponType.Sword;

    public float moveSpeed = 15f;
    public LookMode currentLookMode = LookMode.Movement;
    public CharacterController characterController;
    
    private void Awake()
    {
        Animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        _playerStateIdle = new PlayerStateIdle();
        _playerStateMove = new PlayerStateMove();
        _playerStateAttack = new PlayerStateAttack();
        _playerStateHit = new PlayerStateHit();
        
        _playerStates = new Dictionary<PlayerState, IPlayerState>()
        {
            { PlayerState.Idle, _playerStateIdle },
            { PlayerState.Move, _playerStateMove },
            { PlayerState.Attack, _playerStateAttack},
            { PlayerState.Hit, _playerStateHit}
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
        if (CurrentState != PlayerState.None)
        {
            _playerStates[CurrentState].UpdateState();
        }
    }

    private void Init()
    {
        SetState(PlayerState.Idle);
        _velocity = Vector3.zero;
    }

    public void SetState(PlayerState state)
    {
        if (state == PlayerState.Attack && CurrentWeapon == WeaponType.Sword)
            _velocity = Vector3.zero;
        
        if (CurrentState != PlayerState.None)
        {
            _playerStates[CurrentState].ExitState();
        }
        CurrentState = state;
        _playerStates[CurrentState].EnterState(this);
    }
    
    public void SetWeapon(WeaponType type)
    {
        CurrentWeapon = type;
        Animator.SetInteger("WeaponType", (int)type);
        _currentAttackBehavior = _attackBehaviors[type];
    }
    
    public void Move(Vector3 inputDirection, float speed)
    {
        if (CurrentState == PlayerState.Attack && CurrentWeapon == WeaponType.Sword)
            return;
        
        inputDirection.Normalize();
        
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        
        cameraForward.y = 0;
        cameraRight.y = 0;
        
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        Vector3 moveDirection = cameraForward * inputDirection.z 
                                + cameraRight * inputDirection.x;
        
        if (moveDirection.magnitude > 0.1f)
        {
            if (currentLookMode == LookMode.Movement)
                transform.rotation = Quaternion.LookRotation(moveDirection);
            
            _velocity.y += gravity * Time.deltaTime;
            moveDirection.y = _velocity.y;
            
            characterController.Move(moveDirection * (moveSpeed * Time.deltaTime));
        }
    }

    public void LookAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.Log($"좌표 : {ray}");
        if (Physics.Raycast(ray, out var hit, 300f))
        {
            Vector3 lookDirection = hit.point - transform.position;
            lookDirection.y = 0f;

            if (lookDirection.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
    }

    public void TirggerAttack()
    {
        LookAtMouse();
        Animator.SetTrigger(Attack);
    }
}
