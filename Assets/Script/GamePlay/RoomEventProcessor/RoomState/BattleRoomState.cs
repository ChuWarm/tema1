using System.Collections.Generic;
using UnityEngine;
using Script.Characters;

public class BattleRoomState : IRoomState
{
    private readonly RoomEventProcessor _processor;
    private readonly HashSet<Enemy> _activeEnemies = new();
    private readonly Room _room;

    public BattleRoomState(RoomEventProcessor processor)
    {
        _processor = processor;
        _room = processor.GetRoom();
        GameEventBus.Subscribe<RoomEnemyDeadEvent>(OnEnemyDeadEvent);
    }

    public void OnStateEnter(RoomEventProcessor processor)
    {
        _activeEnemies.Clear();
        Debug.Log($"[전투방] {_room.gameObject.name}: 전투 준비");
    }

    public void OnStateExit(RoomEventProcessor processor)
    {
        GameEventBus.Unsubscribe<RoomEnemyDeadEvent>(OnEnemyDeadEvent);
        Debug.Log($"[전투방] {_room.gameObject.name}: 전투 종료");
    }

    public void OnPlayerEnter(RoomEventProcessor processor)
    {
        if (_room.IsCleared) return;

        var enemySpawnManager = processor.GetEnemySpawnManager();
        if (enemySpawnManager == null)
        {
            Debug.LogError($"[전투방] {_room.gameObject.name}: EnemySpawnManager를 찾을 수 없음");
            return;
        }

        SpawnEnemies(enemySpawnManager);
    }

    public void OnRoomCleared(RoomEventProcessor processor)
    {
        Debug.Log($"[전투방] {_room.gameObject.name}: 전투 승리");
    }

    public void OnStateUpdate(RoomEventProcessor processor)
    {
        CheckAndClearRoomIfWon();
    }

    private void SpawnEnemies(EnemySpawnManager enemySpawnManager)
    {
        Debug.Log($"[전투방] {_room.gameObject.name}: 적 스폰 시작");
        var spawnedEnemies = enemySpawnManager.SpawnEnemiesForRoom(_processor.GetRoomType(), _room.transform);
        _activeEnemies.UnionWith(spawnedEnemies);
    }

    private void OnEnemyDeadEvent(RoomEnemyDeadEvent enemyDeadEvent)
    {
        if (enemyDeadEvent.sender != _processor) return;

        _activeEnemies.Remove(enemyDeadEvent.enemy);
        Debug.Log($"[전투방] {_room.gameObject.name}: 적 처치 (남은 적: {_activeEnemies.Count}마리)");

        CheckAndClearRoomIfWon();
    }

    private void CheckAndClearRoomIfWon()
    {
        if (!_room.IsCleared && _activeEnemies.Count == 0)
        {
            _processor.OnRoomCleared(null);
        }
    }
}