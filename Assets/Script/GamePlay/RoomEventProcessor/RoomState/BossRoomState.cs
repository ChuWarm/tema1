using System.Collections.Generic;
using UnityEngine;
using Script.Characters;

public class BossRoomState : IRoomState
{
    private readonly RoomEventProcessor _processor;
    private readonly HashSet<Enemy> _activeEnemies = new();
    private readonly Room _room;

    public BossRoomState(RoomEventProcessor processor)
    {
        _processor = processor;
        _room = processor.GetRoom();
        GameEventBus.Subscribe<RoomEnemyDeadEvent>(OnEnemyDeadEvent);
    }

    public void OnStateEnter(RoomEventProcessor processor)
    {
        _activeEnemies.Clear();
        Debug.Log($"[보스방] {_room.gameObject.name}: 보스전 준비");
    }

    public void OnStateExit(RoomEventProcessor processor)
    {
        GameEventBus.Unsubscribe<RoomEnemyDeadEvent>(OnEnemyDeadEvent);
        Debug.Log($"[보스방] {_room.gameObject.name}: 보스전 종료");
    }

    public void OnPlayerEnter(RoomEventProcessor processor)
    {
        if (_room.IsCleared) return;

        var enemySpawnManager = processor.GetEnemySpawnManager();
        if (enemySpawnManager == null)
        {
            Debug.LogError($"[보스방] {_room.gameObject.name}: EnemySpawnManager를 찾을 수 없음");
            return;
        }

        SpawnBoss(enemySpawnManager);
    }

    public void OnRoomCleared(RoomEventProcessor processor)
    {
        Debug.Log($"[보스방] {_room.gameObject.name}: 보스 처치");
        // TODO: 보스 처치 후 특별 이벤트 발생
    }

    public void OnStateUpdate(RoomEventProcessor processor)
    {
        if (!_room.IsCleared && _activeEnemies.Count == 0)
        {
            processor.OnRoomCleared(null);
        }
    }

    private void SpawnBoss(EnemySpawnManager enemySpawnManager)
    {
        Debug.Log($"[보스방] {_room.gameObject.name}: 보스 스폰 시작");
        var spawnedEnemies = enemySpawnManager.SpawnEnemiesForRoom(RoomType.Boss, _room.transform);
        _activeEnemies.UnionWith(spawnedEnemies);
    }

    private void OnEnemyDeadEvent(RoomEnemyDeadEvent enemyDeadEvent)
    {
        if (enemyDeadEvent.sender != _processor) return;

        _activeEnemies.Remove(enemyDeadEvent.enemy);
        Debug.Log($"[보스방] {_room.gameObject.name}: 보스 처치 (남은 적: {_activeEnemies.Count}마리)");

        if (_activeEnemies.Count == 0)
        {
            _processor.OnRoomCleared(null);
        }
    }
}
