using System.Collections.Generic;
using UnityEngine;
using Script.Characters;

public class BattleRoomState : IRoomState
{
    private readonly RoomEventProcessor _processor;
    private readonly HashSet<Enemy> _activeEnemies = new();
    private readonly Room _room;
    private bool _combatStarted = false;

    public BattleRoomState(RoomEventProcessor processor)
    {
        _processor = processor;
        _room = processor.GetRoom();
        GameEventBus.Subscribe<RoomEnemyDeadEvent>(OnEnemyDeadEvent);
    }

    public void OnStateEnter(RoomEventProcessor processor)
    {
        _activeEnemies.Clear();
        _combatStarted = false;
    }

    public void OnStateExit(RoomEventProcessor processor)
    {
        GameEventBus.Unsubscribe<RoomEnemyDeadEvent>(OnEnemyDeadEvent);
    }

    public void OnPlayerEnter(RoomEventProcessor processor)
    {
        if (_room.IsCleared)
        {
            return;
        }

        var enemySpawnManager = processor.GetEnemySpawnManager();
        if (enemySpawnManager == null)
        { 
            return;
        }
        
        if (!_combatStarted)
        { 
            SpawnEnemies(enemySpawnManager);
            _combatStarted = true;
        }
        else
        {
        }
    }

    public void OnRoomCleared(RoomEventProcessor processor)
    {
    }

    public void OnStateUpdate(RoomEventProcessor processor)
    {
        CheckAndClearRoomIfWon();
    }

    private void SpawnEnemies(EnemySpawnManager enemySpawnManager)
    {
        var spawnedEnemiesList = enemySpawnManager.SpawnEnemiesForRoom(_processor.GetRoomType(), _room.transform);
        
        if (spawnedEnemiesList == null)
        {
            return;
        }
        
        int beforeCount = _activeEnemies.Count;
        _activeEnemies.UnionWith(spawnedEnemiesList);
        int afterCount = _activeEnemies.Count;

        if (afterCount == 0 && spawnedEnemiesList.Count > 0)
        {
        }
        else if (afterCount == 0 && spawnedEnemiesList.Count == 0)
        {
        }
    }

    private void OnEnemyDeadEvent(RoomEnemyDeadEvent enemyDeadEvent)
    {
        if (enemyDeadEvent.sender != _processor) return;

        if (_activeEnemies.Remove(enemyDeadEvent.enemy))
        {
        }
        else
        {
        }

        CheckAndClearRoomIfWon();
    }

    private void CheckAndClearRoomIfWon()
    {
        if (_combatStarted && !_room.IsCleared && _activeEnemies.Count == 0)
        {
            _processor.OnRoomCleared(null);
        }
    }
}