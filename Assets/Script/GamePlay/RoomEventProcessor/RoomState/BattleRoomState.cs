using System.Collections.Generic;
using Script.Characters;
using UnityEngine;

public class BattleRoomState : IRoomState
{
    private List<EnemyBase> _activeEnemies = new();
    private RoomEventProcessor _roomEventProcessor;

    public BattleRoomState(RoomEventProcessor roomEventProcessor)
    {
        _roomEventProcessor = roomEventProcessor;
        
        GameEventBus.Subscribe<RoomEnemyDeadEvent>(OnEnemyDeadEvent);
    }
    
    public void Enter(RoomEventProcessor processor)
    {
        Debug.Log("Normal Room 입장: 적 스폰 시작");
        
        var positions = new List<Vector3>();

        for (int i = 0; i < 3; i++)
        {
            positions.Add(processor.transform.position + 
                          new Vector3(Random.Range(-15f, 15f), 0, Random.Range(-15f, 15f)));

            EnemyData dummydata = new EnemyData
            {
                enemyID = "dummy_enemy",
                enemyName = "더미",
                health = 10
            };
            
            var enemy = EnemyFactory.SpawnEnemy(dummydata, positions[i], processor.transform);
            if (enemy != null)
                _activeEnemies.Add(enemy);
        }
    }

    public void Update(RoomEventProcessor processor)
    {
        
    }

    public void Exit(RoomEventProcessor processor)
    {

    }
    
    private void OnEnemyDeadEvent(RoomEnemyDeadEvent enemyDeadEvent)
    {
        if (enemyDeadEvent.sender != _roomEventProcessor) return;

        _activeEnemies.Remove(enemyDeadEvent.enemy);
        if (_activeEnemies.Count == 0)
        {
            Debug.Log($"클리어!");
            _roomEventProcessor.OnRoomCleared(new RoomClearedEvent());
        }
    }
}