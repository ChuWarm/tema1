public class SpawnRoomState : IRoomState
{
    private RoomEventProcessor _roomEventProcessor;
    public void Enter(RoomEventProcessor processor)
    {
        _roomEventProcessor = processor;
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
