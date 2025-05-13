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
        Debug.Log("Battle Room 입장: 초기화");
    }

    public void OnPlayerEnter(RoomEventProcessor processor)
    {
        Debug.Log("Battle Room: 플레이어 진입 - 적 스폰 시작");
    }

    public void Update(RoomEventProcessor processor)
    {
    }

    public void Exit(RoomEventProcessor processor)
    {
        GameEventBus.Unsubscribe<RoomEnemyDeadEvent>(OnEnemyDeadEvent);
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