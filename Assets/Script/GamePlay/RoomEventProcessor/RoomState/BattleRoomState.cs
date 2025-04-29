using System.Collections.Generic;
using UnityEngine;

public class BattleRoomState : IRoomState
{
    private readonly List<EnemyBase> _enemies = new();
    
    public void Enter(RoomEventProcessor processor)
    {
        Debug.Log("노말 방 입장: 적 스폰 시작");
        SpawnEnemies(processor);
    }

    public void Update(RoomEventProcessor processor)
    {
        
    }

    public void OnEnemyDead(RoomEventProcessor processor)
    {
        // 적 리스트에서 죽은 적 제거
        _enemies.RemoveAll(e => e == null);

        // 남은 적이 하나도 없으면 방 클리어
        if (_enemies.Count == 0)
        {
            processor.MarkRoomCleared();
            processor.SetState(null);
        }
    }

    private void SpawnEnemies(RoomEventProcessor processor)
    {
        RoomEventHolder holder = processor.GetComponent<RoomEventHolder>();
        DataManager dataManager= Object.FindObjectOfType<DataManager>();

        for (int i = 0; i < 3; i++)
        {
            EnemyBase enemyBase = EnemyFactory.SpawnEnemy(holder, dataManager.GetEnemyData("dummy_enemy"));
            if (enemyBase != null)
            {
                _enemies.Add(enemyBase);
            }
        }
    }
}