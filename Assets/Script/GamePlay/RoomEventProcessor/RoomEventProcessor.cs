using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEventProcessor : MonoBehaviour
{
    private IRoomState _currentRoomState;
    private RoomType _roomType;
    private Room _room;
    private bool _eventTriggered;
    
    private void Start()
    {
        _room = GetComponent<Room>();
        _roomType = _room.RoomType;
        
        SetState(CreateState(_roomType));
    }

    private void OnDisable()
    {
        GameEventBus.RemoveAllSubscribes();
    }

    private IRoomState CreateState(RoomType roomType)
    {
        return roomType switch
        {
            RoomType.Normal => new BattleRoomState(this),
            RoomType.Elite => new BattleRoomState(this),
            RoomType.Rest => new RestRoomState(),
            RoomType.Shop => new ShopRoomState(),
            RoomType.Boss => new BossRoomState(),
            _ => new BattleRoomState(this)
        };
    }

    private void SetState(IRoomState newState)
    {
        _currentRoomState = newState;
    }

    public void OnPlayerEnterRoom()
    {
        if (_eventTriggered) return;

        _eventTriggered = true;
        _currentRoomState?.Enter(this);
    }
    
    public void OnRoomCleared(RoomClearedEvent roomClearedEvent)
    {
        _room.MarkRoomCleared();
    }
}
