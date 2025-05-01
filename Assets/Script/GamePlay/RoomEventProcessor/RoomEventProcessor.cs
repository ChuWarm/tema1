using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEventProcessor : MonoBehaviour
{
    private IRoomState _currentRoomState;
    private RoomType _roomType;

    private void Start()
    {
        // _roomType = GetComponentInParent<Room>()?.RoomType ?? RoomType.Normal;
        
        
    }

    private void Update()
    {
        _currentRoomState?.Update(this);
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
        _currentRoomState?.Enter(this);
    }

    public void OnEnemyDead()
    {
        if (_currentRoomState is BattleRoomState battleRoomState)
        {
            // battleRoomState.OnEnemyDead(this);
        }
    }

    public void MarkRoomCleared()
    {
        GetComponentInParent<Room>()?.MarkRoomCleared();
    }
}
