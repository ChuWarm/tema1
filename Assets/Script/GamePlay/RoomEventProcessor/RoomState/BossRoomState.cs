using UnityEngine;
using Script.Characters;

public class BossRoomState : CombatRoomBaseState
{
    public BossRoomState(RoomEventProcessor processor) : base(processor) { }

    protected override string GetRoomStateName() => "보스방";
    protected override string GetLogStateNameForEnter() => "보스전";
    protected override string GetLogStateNameForCleared() => "보스 처치";

    protected override void PerformSpecificSpawn(EnemySpawnManager enemySpawnManager)
    {
        Debug.Log($"[{GetRoomStateName()}] {_room.gameObject.name}: Performing Boss Spawn.");
        var spawnedBossList = enemySpawnManager.SpawnEnemiesForRoom(RoomType.Boss, _room.transform);

        if (spawnedBossList == null)
        {
            Debug.LogWarning($"[{GetRoomStateName()}] {_room.gameObject.name}: SpawnEnemiesForRoom (Boss) returned null.");
            return;
        }

        foreach (var bossEnemy in spawnedBossList)
        {
            if (bossEnemy != null)
            {
                bossEnemy.SetRoomProcessor(_processor);
            }
        }

        int beforeCount = _activeEnemies.Count;
        _activeEnemies.UnionWith(spawnedBossList);
        int afterCount = _activeEnemies.Count;

        Debug.Log($"[{GetRoomStateName()}] {_room.gameObject.name}: Boss spawned and processor set. Before: {beforeCount}, After: {afterCount}, Spawned list count: {spawnedBossList.Count}");

        if (afterCount == 0 && spawnedBossList.Count > 0)
        {
            Debug.LogWarning($"[{GetRoomStateName()}] {_room.gameObject.name}: Spawned boss but _activeEnemies is still empty!");
        }
        else if (afterCount == 0 && spawnedBossList.Count == 0)
        {
            Debug.LogWarning($"[{GetRoomStateName()}] {_room.gameObject.name}: No boss was spawned and _activeEnemies is empty. Check EnemySpawnManager boss setup.");
        }
    }

    public override void OnRoomCleared(RoomEventProcessor processor)
    {
        base.OnRoomCleared(processor);
    }
}
