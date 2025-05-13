using UnityEngine;

public class BossRoomState : IRoomState
{
    private RoomEventProcessor _roomEventProcessor;
    
    public void Enter(RoomEventProcessor processor)
    {
        _roomEventProcessor = processor;
    }

    public void OnPlayerEnter(RoomEventProcessor processor)
    {
        Debug.Log("Boss Room: 플레이어 진입 - 보스 스폰 시작");
        // 보스 생성 로직은 나중에 구현
    }

    public void Update(RoomEventProcessor processor)
    {
        
    }

    public void Exit(RoomEventProcessor processor)
    {
        _roomEventProcessor = null;
    }
}
