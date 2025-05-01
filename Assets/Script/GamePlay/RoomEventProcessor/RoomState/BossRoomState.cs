using UnityEngine;

public class BossRoomState : IRoomState
{
    private RoomEventProcessor _roomEventProcessor;
    
    public void Enter(RoomEventProcessor processor)
    {
        _roomEventProcessor = processor;
    }

    public void Update(RoomEventProcessor processor)
    {
        
    }

    public void Exit(RoomEventProcessor processor)
    {
        _roomEventProcessor = null;
    }
}
