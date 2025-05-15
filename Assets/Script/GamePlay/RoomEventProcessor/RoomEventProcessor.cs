using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class RoomEventProcessor : MonoBehaviour
{
    private IRoomState _currentRoomState;
    private RoomType _roomType;
    private Room _room;
    private bool _isInitialized;
    public bool IsInitialized => _isInitialized;
    private EnemySpawnManager _enemySpawnManager;
    private UniTaskCompletionSource<bool> _initCompletionSource;
    public UniTask<bool> InitializationTask => _initCompletionSource.Task;
    
    private void Awake()
    {
        _initCompletionSource = new UniTaskCompletionSource<bool>();
        InitializeComponents();
        StartCoroutine(InitializeRoom());
    }

    private void Update()
    {
        if (_isInitialized)
        {
            _currentRoomState?.OnStateUpdate(this);
        }
    }

    private void InitializeComponents()
    {
        _room = GetComponent<Room>();
        if (_room == null)
        {
            Debug.LogError($"[방이벤트] {gameObject.name}: Room 컴포넌트를 찾을 수 없음");
            return;
        }

        _roomType = _room.RoomType;

        if (_roomType == RoomType.Normal || _roomType == RoomType.Normal2 || 
            _roomType == RoomType.Elite || _roomType == RoomType.Boss)
        {
            _enemySpawnManager = GetComponent<EnemySpawnManager>();
            if (_enemySpawnManager == null)
            {
                _enemySpawnManager = gameObject.AddComponent<EnemySpawnManager>();
            }
        }
    }

    private IEnumerator InitializeRoom()
    {
        // MapData가 필요한 방의 경우 대기
        if (RequiresMapData(_roomType))
        {
            while (_room.GetMapData() == null)
            {
                yield return null;
            }
        }

        try
        {
            // 상태 생성 및 초기화
            _currentRoomState = CreateState(_roomType);
            _currentRoomState.OnStateEnter(this);
            _isInitialized = true;
            
            _initCompletionSource.TrySetResult(true);
        }
        catch (Exception e)
        {
            Debug.LogError($"[방이벤트] {gameObject.name}: 초기화 실패 - {e.Message}");
            _initCompletionSource.TrySetException(e);
        }
    }

    private bool RequiresMapData(RoomType roomType)
    {
        return roomType == RoomType.Spawn || roomType == RoomType.Boss;
    }

    private IRoomState CreateState(RoomType roomType)
    {
        return roomType switch
        {
            RoomType.Spawn => new SpawnRoomState(this),
            RoomType.Normal => new BattleRoomState(this),
            RoomType.Normal2 => new BattleRoomState(this),
            RoomType.Elite => new BattleRoomState(this),
            RoomType.Rest => new RestRoomState(this),
            RoomType.Shop => new ShopRoomState(this),
            RoomType.Boss => new BossRoomState(this),
            _ => new BattleRoomState(this)
        };
    }

    public async UniTask OnPlayerEnterRoomAsync()
    {
        if (!_isInitialized)
        {
            try
            {
                await _initCompletionSource.Task;
            }
            catch (Exception e)
            {
                Debug.LogError($"[방이벤트] {gameObject.name}: 초기화 실패로 인한 플레이어 진입 실패 - {e.Message}");
                return;
            }
        }

        
        if (_roomType == RoomType.Normal || _roomType == RoomType.Normal2 ||
            _roomType == RoomType.Elite || _roomType == RoomType.Boss)
        {
            _room.CloseAllDoors();
        }
        
        _currentRoomState?.OnPlayerEnter(this);
    }

    public void OnPlayerEnterRoom()
    {
        OnPlayerEnterRoomAsync().Forget();
    }
    
    public void OnRoomCleared(RoomClearedEvent roomClearedEvent)
    {
        if (!_isInitialized)
        {
            // Debug.Log($"[{gameObject.name}] RoomEventProcessor.OnRoomCleared - Processor not initialized. Returning."); // 유지 또는 삭제 (일단 삭제)
            return;
        }

        if (roomClearedEvent == null || roomClearedEvent.ClearedRoom == null)
        {
            Debug.LogError($"[{gameObject.name}] RoomEventProcessor.OnRoomCleared - Received null event or null ClearedRoom.");
            if (_room != null) {
                 if (!_room.IsCleared) 
                 {
                    Debug.LogWarning($"[{gameObject.name}] RoomEventProcessor.OnRoomCleared - Bad event, attempting to clear and open current room: {_room.name}");
                    _room.MarkRoomAsCleared();
                 }
                 // else if (_room.IsCleared) { _room.OpenAllDoors(); } // 이미 클리어 되었다면 문을 열어줌 (위의 roomToProcess.IsCleared 경우와 유사)
            }
            return;
        }

        Room roomToProcess = roomClearedEvent.ClearedRoom;
        if (roomClearedEvent.sender != null && roomClearedEvent.sender != this)
        {
            // Debug.LogWarning($"[{gameObject.name}] RoomEventProcessor.OnRoomCleared - Event sender mismatch. Event for {roomToProcess.name} (sender: {roomClearedEvent.sender.GetInstanceID()}), this processor: {GetInstanceID()}. Ignoring."); // 삭제
            return;
        }

        if (roomToProcess.IsCleared)
        {

            roomToProcess.OpenAllDoors(); 
            _currentRoomState?.OnRoomCleared(this); 
            return;
        }
        
        roomToProcess.MarkRoomAsCleared();
        _currentRoomState?.OnRoomCleared(this); 
    }

    private void OnDestroy()
    {
        if (_isInitialized)
        {
            _currentRoomState?.OnStateExit(this);
        }
        _initCompletionSource?.TrySetCanceled();
    }

    // 상태 클래스에서 사용할 수 있는 헬퍼 메서드들
    public Room GetRoom() => _room;
    public RoomType GetRoomType() => _roomType;
    public EnemySpawnManager GetEnemySpawnManager() => _enemySpawnManager;
}
