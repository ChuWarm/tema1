using System.Collections.Generic;
using Script.Characters;
using UnityEngine;

public class BattleRoomState : IRoomState
{
    private List<EnemyBase> _activeEnemies = new();
    private RoomEventProcessor _roomEventProcessor;
    private bool _isCleared = false;

    public BattleRoomState(RoomEventProcessor roomEventProcessor)
    {
        _roomEventProcessor = roomEventProcessor;
        GameEventBus.Subscribe<RoomEnemyDeadEvent>(OnEnemyDeadEvent);
    }
    
    public void Enter(RoomEventProcessor processor)
    {
        _isCleared = false;
        Debug.Log($"BattleRoomState: Entering {processor.gameObject.name}");
    }

    public void OnPlayerEnter(RoomEventProcessor processor)
    {
        if (_isCleared) return;

        var enemySpawnManager = processor.GetComponent<EnemySpawnManager>();
        if (enemySpawnManager != null)
        {
            // 적 생성 이벤트 구독
            enemySpawnManager.OnEnemySpawned += OnEnemySpawned;
            
            // 적 생성 시작
            enemySpawnManager.SpawnEnemiesForRoom(processor.GetComponent<Room>().RoomType, processor.transform);
            Debug.Log($"BattleRoomState: Started enemy spawn in {processor.gameObject.name}");
        }
        else
        {
            Debug.LogError($"BattleRoomState: EnemySpawnManager not found in {processor.gameObject.name}");
            // 적 생성이 불가능한 경우 자동 클리어
            _isCleared = true;
            processor.OnRoomCleared(new RoomClearedEvent());
        }
    }

    private void OnEnemySpawned(EnemyBase enemy)
    {
        if (enemy != null && !_isCleared)
        {
            _activeEnemies.Add(enemy);
            Debug.Log($"BattleRoomState: Enemy spawned, total count: {_activeEnemies.Count}");
        }
    }

    public void Update(RoomEventProcessor processor)
    {
        // 적이 없고 클리어되지 않은 상태라면 자동 클리어
        if (!_isCleared && _activeEnemies.Count == 0)
        {
            _isCleared = true;
            processor.OnRoomCleared(new RoomClearedEvent());
        }
    }

    public void Exit(RoomEventProcessor processor)
    {
        var enemySpawnManager = processor.GetComponent<EnemySpawnManager>();
        if (enemySpawnManager != null)
        {
            enemySpawnManager.OnEnemySpawned -= OnEnemySpawned;
        }
        GameEventBus.Unsubscribe<RoomEnemyDeadEvent>(OnEnemyDeadEvent);
        _activeEnemies.Clear();
        Debug.Log($"BattleRoomState: Exiting {processor.gameObject.name}");
    }
    
    private void OnEnemyDeadEvent(RoomEnemyDeadEvent enemyDeadEvent)
    {
        if (enemyDeadEvent.sender != _roomEventProcessor || _isCleared) return;

        _activeEnemies.Remove(enemyDeadEvent.enemy);
        Debug.Log($"BattleRoomState: Enemy died, remaining: {_activeEnemies.Count}");

        if (_activeEnemies.Count == 0)
        {
            _isCleared = true;
            _roomEventProcessor.OnRoomCleared(new RoomClearedEvent());
        }
    }
}