using UnityEngine;

public class ShopRoomState : IRoomState
{
    private RoomEventProcessor _roomEventProcessor;
    
    public void Enter(RoomEventProcessor processor)
    {
        _roomEventProcessor = processor;
        Debug.Log("상점 오픈");
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