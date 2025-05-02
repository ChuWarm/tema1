using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public enum PlayerState { None, Idle, Move, Attack, Hit }
public enum LookMode { None, Movement, Mouse }

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private const float gravity = -9.81f;
    
    private Dictionary<PlayerState, IPlayerState> _playerStates;
    private PlayerStateIdle _playerStateIdle;
    private PlayerStateMove _playerStateMove;
    private PlayerStateAttack _playerStateAttack;
    private PlayerStateHit _playerStateHit;
    private float _moveSpeed = 15f;
    private Vector3 _velocity;
    
    public PlayerState CurrentState { get; private set; }

    public LookMode currentLookMode = LookMode.Movement;
    public CharacterController characterController;
    public Animator animator;
    
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
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
    
    public void Move(Vector3 inputDirection)
    {
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
            
            characterController.Move(moveDirection * (_moveSpeed * Time.deltaTime));
        }
    }

    public void LookAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 100f))
        {
            Vector3 lookDirection = hit.point - transform.position;
            lookDirection.y = 0f;

            if (lookDirection.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
    }
}
