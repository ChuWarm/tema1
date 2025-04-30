using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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

    private IRoomState CreateState(RoomType roomType)
    {
        return roomType switch
        {
            RoomType.Normal => new BattleRoomState(),
            RoomType.Elite => new BattleRoomState(),
            RoomType.Rest => new RestRoomState(),
            RoomType.Shop => new ShopRoomState(),
            RoomType.Boss => new BossRoomState(),
            _ => new BattleRoomState()
        };
    }

    public void SetState(IRoomState newState)
    {
        _currentRoomState = newState;
    }

    public void OnPlayerEnterRoom()
    {
        if (_eventTriggered) return;
        
        _eventTriggered = true;
        _currentRoomState?.Enter(this);
    }

    public void OnRoomCleared()
    {
        _room.MarkRoomCleared();
    }
}
