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
            Debug.LogError($"[{GetLogStateNameForEnter()}] {_room.gameObject.name}: EnemySpawnManager not found!");
            return;
        }
        
        if (!_combatStarted)
        { 
            PerformSpecificSpawn(enemySpawnManager);
            _combatStarted = true;
        }
    }

    protected override void PerformSpecificSpawn(EnemySpawnManager enemySpawnManager)
    {
        var spawnedEnemiesList = enemySpawnManager.SpawnEnemiesForRoom(_processor.GetRoomType(), _room.transform);
        
        if (spawnedEnemiesList == null)
        {
            Debug.LogWarning($"[{GetRoomStateName()}] {_room.gameObject.name}: SpawnEnemiesForRoom returned null.");
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

        if (_activeEnemies.Count == 0 && spawnedEnemiesList.Count > 0)
        {
            Debug.LogWarning($"[{GetRoomStateName()}] {_room.gameObject.name}: Spawned enemies but _activeEnemies (base) is still empty!");
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