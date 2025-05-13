using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEventProcessor : MonoBehaviour
{
    private IRoomState _currentRoomState;
    private RoomType _roomType;
    private Room _room;
    private bool _isInitialized = false;
    public bool IsInitialized => _isInitialized;
    private EnemySpawnManager _enemySpawnManager;
    
    private void Awake()
    {
        _room = GetComponent<Room>();
        if (_room == null)
        {
            Debug.LogError($"RoomEventProcessor {gameObject.name}: Room component not found!");
            return;
        }

        _roomType = _room.RoomType;
        Debug.Log($"RoomEventProcessor {gameObject.name}: Initializing for room type {_roomType}");
        
        // Normal과 Elite 방에서만 EnemySpawnManager 생성
        if (_roomType == RoomType.Normal || _roomType == RoomType.Elite)
        {
            _enemySpawnManager = GetComponent<EnemySpawnManager>();
            if (_enemySpawnManager == null)
            {
                _enemySpawnManager = gameObject.AddComponent<EnemySpawnManager>();
                Debug.Log($"RoomEventProcessor {gameObject.name}: Added EnemySpawnManager for {_roomType} room");
            }
        }
        else
        {
            Debug.Log($"RoomEventProcessor {gameObject.name}: Skipping EnemySpawnManager for {_roomType} room");
        }
        
        SetState(CreateState(_roomType));
        _isInitialized = true;

        // 스폰 방 초기화는 MapData가 설정된 후에 수행
        if (_roomType == RoomType.Spawn)
        {
            StartCoroutine(InitializeSpawnRoom());
        }
    }

    private IEnumerator InitializeSpawnRoom()
    {
        // MapData가 설정될 때까지 대기
        while (_room.GetMapData() == null)
        {
            yield return null;
        }
        
        // 모든 문 열기
        foreach (var door in _room.GetDoors())
        {
            if (door != null)
            {
                door.Open();
                Debug.Log($"RoomEventProcessor {gameObject.name}: Opening spawn room door {door.gameObject.name}");
            }
        }
        
        _currentRoomState.Enter(this);
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
            Debug.LogError($"RoomEventProcessor {gameObject.name}: Cannot process room enter - not initialized!");
            return;
        }

        Debug.Log($"RoomEventProcessor {gameObject.name}: Processing room enter for type {_roomType}");
        
        // 방 상태 처리
        _currentRoomState?.OnPlayerEnter(this);
    }
    
    public void OnRoomCleared(RoomClearedEvent roomClearedEvent)
    {
        _room.MarkRoomCleared();
    }
}
