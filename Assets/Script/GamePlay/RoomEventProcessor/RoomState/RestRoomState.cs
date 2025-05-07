using UnityEngine;

public class RestRoomState : IRoomState
{
    private RoomEventProcessor _roomEventProcessor;
    public void Enter(RoomEventProcessor processor)
    {
        _roomEventProcessor = processor;
        Debug.Log("휴식 방: 체력 회복");
        processor.OnRoomCleared(new RoomClearedEvent());
    }
    public void Update(RoomEventProcessor processor)
    {
        
    }
    public void Exit(RoomEventProcessor processor)
    {
        _roomEventProcessor = null;
    }
}