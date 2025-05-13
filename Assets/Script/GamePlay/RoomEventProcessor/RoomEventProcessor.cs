using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEventProcessor : MonoBehaviour
{
    private IRoomState _currentRoomState;
    private RoomType _roomType;
    private Room _room;
    private bool _eventTriggered;
    private bool _isInitialized = false;
    public bool IsInitialized => _isInitialized;
    private EnemySpawnManager _enemySpawnManager;
    
    private void Awake()
    {
        _room = GetComponent<Room>();
        _roomType = _room.RoomType;
        _enemySpawnManager = GetComponent<EnemySpawnManager>();
        if (_enemySpawnManager == null)
        {
            _enemySpawnManager = gameObject.AddComponent<EnemySpawnManager>();
        }
        
        SetState(CreateState(_roomType));

        if (_roomType == RoomType.Spawn)
        {
            _eventTriggered = true;
            _currentRoomState.Enter(this);
        }

        _isInitialized = true;
    }


    private IRoomState CreateState(RoomType roomType)
    {
        return roomType switch
        {
            RoomType.Spawn => new SpawnRoomState(),
            RoomType.Normal => new BattleRoomState(this),
            RoomType.Elite => new BattleRoomState(this),
            RoomType.Rest => new RestRoomState(),
            RoomType.Shop => new ShopRoomState(),
            RoomType.Boss => new BossRoomState(),
            _ => new BattleRoomState(this)
        };
    }

    private void SetState(IRoomState newState)
    {
        _currentRoomState = newState;
    }

    public void OnPlayerEnterRoom()
    {
        if (!_isInitialized)
        {
            Debug.LogWarning($"RoomEventProcessor {gameObject.name}: Cannot process room enter - not initialized!");
            return;
        }

        if (_eventTriggered)
        {
            Debug.Log($"RoomEventProcessor {gameObject.name}: Room event already triggered, skipping.");
            return;
        }

        Debug.Log($"RoomEventProcessor {gameObject.name}: Player entered room of type {_roomType}");
        _eventTriggered = true;

        // 적 생성
        if (_roomType != RoomType.Spawn)
        {
            _enemySpawnManager.SpawnEnemiesForRoom(_roomType, transform);
        }

        _currentRoomState?.OnPlayerEnter(this);
    }
    
    public void OnRoomCleared(RoomClearedEvent roomClearedEvent)
    {
        _room.MarkRoomCleared();
    }
}
