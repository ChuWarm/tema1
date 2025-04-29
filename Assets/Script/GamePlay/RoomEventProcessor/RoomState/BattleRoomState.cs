using System.Collections.Generic;
using UnityEngine;

public class BattleRoomState : IRoomState
{
    // private List<EnemyBase> _enemies = new();
    
    public void Enter(RoomEventProcessor processor)
    {
        // Datamanager datamanager = Object.FindObjectOfType<DataManager>();

        for (int i = 0; i < 3; i++)
        {
            //var enemy = EnemyFactory.SpawnEnemy(processor.GetComponent<RoomEventHolder>(),
                //datamanager.GetEmeyData("dummy_enemy"));
            //if (enemy != null)
            {
               // _enemies.Add(enemy);
            }
        }
    }

    public void Update(RoomEventProcessor processor)
    {
        //_enemies.RemoveAll(e => e == null);

        //if (_enemies.Count == 0)
        {
            processor.MarkRoomCleared();
            processor.SetState(null);
        }
    }

    public void OnEnemyDead(RoomEventProcessor processor)
    {
        
    }
}