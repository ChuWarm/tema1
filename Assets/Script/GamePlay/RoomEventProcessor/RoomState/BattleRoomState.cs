using System.Collections.Generic;
using UnityEngine;
using Script.Characters;

public class BattleRoomState : CombatRoomBaseState
{
    public BattleRoomState(RoomEventProcessor processor) : base(processor)
    {
    }

    public override void OnStateEnter(RoomEventProcessor processor)
    {
        base.OnStateEnter(processor);
    }

    public override void OnStateExit(RoomEventProcessor processor)
    {
        base.OnStateExit(processor);
    }

    public override void OnPlayerEnter(RoomEventProcessor processor)
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
        
        if (_combatStarted == false)
{ 
    PerformSpecificSpawn(enemySpawnManager);
}
    }

    protected override void PerformSpecificSpawn(EnemySpawnManager enemySpawnManager)
    {
        var spawnedEnemiesList = enemySpawnManager.SpawnEnemiesForRoom(_processor.GetRoomType(), _room.transform);
        
        if (spawnedEnemiesList == null)
        {
            Debug.LogWarning($"[BattleRoomState] {_room.gameObject.name}: SpawnEnemiesForRoom returned null.");
            return;
        }

        foreach (var enemy in spawnedEnemiesList)
        {
            if (enemy != null)
            {
                enemy.SetRoomProcessor(_processor);
            }
        }
        
        _activeEnemies.UnionWith(spawnedEnemiesList);

        Debug.Log($"[BattleRoomState] {_room.gameObject.name}: Enemies spawned and processor set. Active: {_activeEnemies.Count}, Spawned list: {spawnedEnemiesList.Count}");

        if (_activeEnemies.Count == 0 && spawnedEnemiesList.Count > 0)
        {
            Debug.LogWarning($"[BattleRoomState] {_room.gameObject.name}: Spawned enemies but _activeEnemies (base) is still empty!");
        }
        else if (_activeEnemies.Count == 0 && spawnedEnemiesList.Count == 0)
        {
            Debug.LogWarning($"[BattleRoomState] {_room.gameObject.name}: No enemies were spawned and _activeEnemies (base) is empty.");
        }
    }

    protected override string GetRoomStateName()
    {
        return "BattleRoomState";
    }

    protected override string GetLogStateNameForEnter()
    {
        return "Battle Room";
    }

    protected override string GetLogStateNameForCleared()
    {
        return "Battle Room Cleared";
    }
}