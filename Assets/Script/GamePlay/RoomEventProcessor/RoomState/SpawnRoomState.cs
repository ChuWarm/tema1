using UnityEngine;

public class SpawnRoomState : IRoomState
{
    private readonly RoomEventProcessor _processor;
    private readonly Room _room;

    public SpawnRoomState(RoomEventProcessor processor)
    {
        _processor = processor;
        _room = processor.GetRoom();
    }

    public void OnStateEnter(RoomEventProcessor processor)
    {
    }

    public void OnStateExit(RoomEventProcessor processor)
    {
    }

    public void OnPlayerEnter(RoomEventProcessor processor)
    {
        if (_room.IsCleared)
        {
            return;
        }
        InitializeSpawnPoint();
        processor.OnRoomCleared(null);
    }

    public void OnRoomCleared(RoomEventProcessor processor)
    {
    }

    public void OnStateUpdate(RoomEventProcessor processor)
    {

    }

    private void InitializeSpawnPoint()
    {
    }
}
