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
            // Debug.Log($"[{_room.gameObject.name}] SpawnRoomState: OnPlayerEnter - Already cleared.");
            return;
        }
        // Debug.Log($"[{_room.gameObject.name}] SpawnRoomState: OnPlayerEnter - Initializing spawn point and clearing room.");
        InitializeSpawnPoint();
        // Spawn room is instantly cleared on player enter if not already.
        processor.OnRoomCleared(new RoomClearedEvent { sender = processor, ClearedRoom = _room });
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
