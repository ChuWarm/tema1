public class SpawnRoomState : IRoomState
{
    private RoomEventProcessor _roomEventProcessor;
    public void Enter(RoomEventProcessor processor)
    {
        _roomEventProcessor = processor;
        processor.OnRoomCleared(new RoomClearedEvent());
    }

    public void OnPlayerEnter(RoomEventProcessor processor)
    {
        // 스폰룸은 이미 클리어된 상태이므로 추가 작업이 필요 없음
    }

    public void Update(RoomEventProcessor processor)
    {

    }

    public void Exit(RoomEventProcessor processor)
    {
        _roomEventProcessor = null;
    }
}
