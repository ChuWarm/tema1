using System.Collections.Generic;
using UnityEngine;
using Script.Characters;

public abstract class CombatRoomBaseState : IRoomState
{
    protected readonly RoomEventProcessor _processor;
    protected readonly HashSet<Enemy> _activeEnemies = new();
    protected readonly Room _room;
    protected bool _combatStarted = false;

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
    }

    public virtual void OnStateExit(RoomEventProcessor processor)
    {
        GameEventBus.Unsubscribe<RoomEnemyDeadEvent>(OnEnemyDeadEvent);
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
            // Debug.LogWarning($"[{GetRoomStateName()}] {_room.gameObject.name}: OnPlayerEnter - Room already cleared. Skipping spawn.");
            return;
        }

        var enemySpawnManager = processor.GetEnemySpawnManager();
        if (enemySpawnManager == null)
        {
            Debug.LogError($"[{GetRoomStateName()}] {_room.gameObject.name}: OnPlayerEnter - EnemySpawnManager not found!");
            return;
        }

        if (!_combatStarted)
        {
            // Debug.Log($"[{GetRoomStateName()}] {_room.gameObject.name}: OnPlayerEnter - Performing specific spawn.");
            PerformSpecificSpawn(enemySpawnManager);
            _combatStarted = true;
            // Debug.Log($"[{GetRoomStateName()}] {_room.gameObject.name}: OnPlayerEnter - Combat started. _combatStarted = true. Initial active enemies: {_activeEnemies.Count}");
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
        // Debug.Log($"[{GetRoomStateName()}] {_room.gameObject.name}: OnEnemyDeadEvent received from sender: {enemyDeadEvent.sender?.GetInstanceID()} for enemy: {enemyDeadEvent.enemy?.name}. Expected processor: {_processor.GetInstanceID()}");

        if (enemyDeadEvent.sender != _processor) 
        {
            Debug.LogWarning($"[{GetRoomStateName()}] {_room.gameObject.name}: OnEnemyDeadEvent - Event sender does not match current room processor. Event ignored.");
            return;
        }

        // int countBeforeRemove = _activeEnemies.Count;
        bool removed = _activeEnemies.Remove(enemyDeadEvent.enemy);
        // Debug.Log($"[{GetRoomStateName()}] {_room.gameObject.name}: Enemy {enemyDeadEvent.enemy?.name} removal attempted. Success: {removed}. Active enemies before: {countBeforeRemove}, after: {_activeEnemies.Count}");
        
        CheckAndClearRoomIfWon();
    }

    private void CheckAndClearRoomIfWon()
    {
        string roomName = _room.gameObject.name;
        string stateName = GetRoomStateName();
        
        if (_combatStarted && !_room.IsCleared && _activeEnemies.Count == 0 && _processor.IsInitialized)
        {
            Debug.Log($"[{stateName}] {roomName}: All enemies cleared. Marking room as cleared.");
            _processor.OnRoomCleared(new RoomClearedEvent { sender = _processor, ClearedRoom = _room });
        }
    }
} 