using UnityEngine;

public class RestRoomState : IRoomState
{
    private readonly RoomEventProcessor _processor;
    private readonly Room _room;

    public RestRoomState(RoomEventProcessor processor)
    {
        _processor = processor;
        _room = processor.GetRoom();
    }

    public void OnStateEnter(RoomEventProcessor processor)
    {
        Debug.Log($"[휴식방] {_room.gameObject.name}: 휴식 준비");
    }

    public void OnStateExit(RoomEventProcessor processor)
    {
        Debug.Log($"[휴식방] {_room.gameObject.name}: 휴식 종료");
    }

    public void OnPlayerEnter(RoomEventProcessor processor)
    {
        if (_room.IsCleared) return;
        StartRest();
    }

    public void OnRoomCleared(RoomEventProcessor processor)
    {
        Debug.Log($"[휴식방] {_room.gameObject.name}: 휴식 완료");
    }

    public void OnStateUpdate(RoomEventProcessor processor)
    {
        if (!_room.IsCleared)
        {
            processor.OnRoomCleared(null);
        }
    }

    private void StartRest()
    {
        // TODO: 플레이어 체력 회복 로직
        Debug.Log($"[휴식방] {_room.gameObject.name}: 휴식 시작");
    }
}