using UnityEngine;

public class RestRoomState : IRoomState
{
    private RoomEventProcessor _roomEventProcessor;

    public void Enter(RoomEventProcessor processor)
    {
        _roomEventProcessor = processor;
    }

    public void OnPlayerEnter(RoomEventProcessor processor)
    {
        // 휴식방은 자동으로 클리어됨
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