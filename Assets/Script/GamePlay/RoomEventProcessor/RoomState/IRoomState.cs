using UnityEngine;

public interface IRoomState
{
    void OnStateEnter(RoomEventProcessor processor);
    void OnStateExit(RoomEventProcessor processor);
    void OnPlayerEnter(RoomEventProcessor processor);
    void OnRoomCleared(RoomEventProcessor processor);
    void OnStateUpdate(RoomEventProcessor processor);
}
