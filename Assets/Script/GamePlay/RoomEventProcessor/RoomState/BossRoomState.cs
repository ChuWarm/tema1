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
        var spawnedBossList = enemySpawnManager.SpawnEnemiesForRoom(RoomType.Boss, _room.transform);
        
        if (spawnedBossList == null)
        {
            return;
        }

        int beforeCount = _activeEnemies.Count;
        _activeEnemies.UnionWith(spawnedBossList);
        int afterCount = _activeEnemies.Count;

        if (afterCount == 0 && spawnedBossList.Count > 0)
        {
        }
    }

    public override void OnRoomCleared(RoomEventProcessor processor)
    {
        base.OnRoomCleared(processor);
    }
}
