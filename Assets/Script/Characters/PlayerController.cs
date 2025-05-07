using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public enum PlayerState { None, Idle, Move, Attack, Hit }

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public PlayerStats playerStats;

    private PlayerStateIdle _playerStateIdle;
    private PlayerStateMove _playerStateMove;
    private PlayerStateAttack _playerStateAttack;
    private PlayerStateHit _playerStateHit;
    
    public PlayerState CurrentState { get; private set; }
    
    private Dictionary<PlayerState, IPlayerState> _playerStates;
    
    public CharacterController characterController;
    
    private void Awake()
    {
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
}
