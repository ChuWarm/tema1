using System.Collections.Generic;
using UnityEngine;
using Script.Characters;

public abstract class CombatRoomBaseState : IRoomState
{
    protected readonly RoomEventProcessor _processor;
    protected readonly HashSet<Enemy> _activeEnemies = new();
    protected readonly Room _room;
    private bool _combatStarted = false;

    protected CombatRoomBaseState(RoomEventProcessor processor)
    {
        _processor = processor;
        _room = processor.GetRoom();
        GameEventBus.Subscribe<RoomEnemyDeadEvent>(OnEnemyDeadEvent);
    }

    public virtual void OnStateEnter(RoomEventProcessor processor)
    {
        _activeEnemies.Clear();
        _combatStarted = false;
        // Debug.Log($"[{GetRoomStateName()}] {_room.gameObject.name}: {GetLogStateNameForEnter()} 준비. _combatStarted = {_combatStarted}");
    }

    public virtual void OnStateExit(RoomEventProcessor processor)
    {
        GameEventBus.Unsubscribe<RoomEnemyDeadEvent>(OnEnemyDeadEvent);
        // Debug.Log($"[{GetRoomStateName()}] {_room.gameObject.name}: {GetLogStateNameForEnter()} 종료");
    }

    public virtual void OnPlayerEnter(RoomEventProcessor processor)
    {
        var mapData = _room.GetMapData();
        if (mapData == null)
        {
            // Debug.LogError($"[{GetRoomStateName()}] {_room.gameObject.name}: OnPlayerEnter - _mapData가 null입니다! Room.Init()가 제대로 호출되었는지 확인 필요.");
        }
        else
        {
            // Debug.Log($"[{GetRoomStateName()}] {_room.gameObject.name}: OnPlayerEnter - _mapData.isCleared = {mapData.isCleared}");
        }

        if (_room.IsCleared) 
        {
            // Debug.LogWarning($"[{GetRoomStateName()}] {_room.gameObject.name}: OnPlayerEnter 호출 시 이미 IsCleared가 true입니다. 스폰 및 전투 시작을 건너뜁니다.");
            return;
        }

        var enemySpawnManager = processor.GetEnemySpawnManager();
        if (enemySpawnManager == null)
        {
            // Debug.LogError($"[{GetRoomStateName()}] {_room.gameObject.name}: EnemySpawnManager를 찾을 수 없음. 스폰 실패.");
            return;
        }

        if (!_combatStarted)
        {
            // Debug.Log($"[{GetRoomStateName()}] {_room.gameObject.name}: PerformSpecificSpawn 호출 준비.");
            PerformSpecificSpawn(enemySpawnManager);
            _combatStarted = true;
            // Debug.Log($"[{GetRoomStateName()}] {_room.gameObject.name}: OnPlayerEnter - 전투 시작! _combatStarted = {_combatStarted}");
        }
        else
        {
            // Debug.Log($"[{GetRoomStateName()}] {_room.gameObject.name}: OnPlayerEnter - 이미 전투가 시작된 상태 (_combatStarted = true). 추가 스폰 없음.");
        }
    }

    public virtual void OnRoomCleared(RoomEventProcessor processor)
    {
        // Debug.Log($"[{GetRoomStateName()}] {_room.gameObject.name}: {GetLogStateNameForCleared()} (OnRoomCleared 호출됨)");
    }

    public void OnStateUpdate(RoomEventProcessor processor)
    {
        CheckAndClearRoomIfWon();
    }

    protected abstract void PerformSpecificSpawn(EnemySpawnManager enemySpawnManager);
    protected abstract string GetRoomStateName();
    protected abstract string GetLogStateNameForEnter();
    protected abstract string GetLogStateNameForCleared();


    private void OnEnemyDeadEvent(RoomEnemyDeadEvent enemyDeadEvent)
    {
        if (enemyDeadEvent.sender != _processor) return;

        if (_activeEnemies.Remove(enemyDeadEvent.enemy))
        {
            // Debug.Log($"[{GetRoomStateName()}] {_room.gameObject.name}: 적 처치 ({enemyDeadEvent.enemy.name}). 남은 적: {_activeEnemies.Count}마리");
        }
        else
        {
            // Debug.LogWarning($"[{GetRoomStateName()}] {_room.gameObject.name}: OnEnemyDeadEvent - _activeEnemies에서 {enemyDeadEvent.enemy.name} 제거 실패.");
        }

        CheckAndClearRoomIfWon();
    }

    private void CheckAndClearRoomIfWon()
    {
        if (_combatStarted && !_room.IsCleared && _activeEnemies.Count == 0 && _processor.IsInitialized)
        {
            // Debug.LogWarning($"[{GetRoomStateName()}] {_room.gameObject.name}: CheckAndClearRoomIfWon 조건 만족! 방 클리어 처리를 요청합니다. (_combatStarted: true, IsCleared: false, ActiveEnemies: 0, ProcessorInitialized: true)");
            _processor.OnRoomCleared(null);
        }
    }
} 