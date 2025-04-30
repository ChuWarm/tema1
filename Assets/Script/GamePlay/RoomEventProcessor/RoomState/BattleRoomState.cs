using System.Collections.Generic;
using UnityEngine;

public class BattleRoomState : IRoomState
{
    public void Enter(RoomEventProcessor processor)
    {
        Debug.Log("Normal Room 입장: 적 스폰 시작");
        
        RoomEventHolder holder = processor.GetComponent<RoomEventHolder>();
        
        var spawnPositions = new List<Vector3>();
        var enemyDatas = new List<EnemyData>();

        for (int i = 0; i < 3; i++)
        {
            enemyDatas.Add(new EnemyData
            {
                enemyID = "dummy_enemy", 
                enemyName = "더미", 
                health = 10
            });
            
            spawnPositions.Add(
                processor.transform.position + 
                new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)));
        }
        
        holder.SpawnEnemies(enemyDatas, spawnPositions);
    }

    public void Update(RoomEventProcessor processor)
    {
        
    }
}