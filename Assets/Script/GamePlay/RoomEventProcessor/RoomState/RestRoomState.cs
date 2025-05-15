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
        // Debug.Log($"[휴식방] {_room.gameObject.name}: 휴식 준비");
    }

    public void OnStateExit(RoomEventProcessor processor)
    {
        // Debug.Log($"[휴식방] {_room.gameObject.name}: 휴식 종료");
    }

    public void OnPlayerEnter(RoomEventProcessor processor)
    {
        if (_room.IsCleared)
        {
            // Debug.Log($"[휴식방] {_room.gameObject.name}: OnPlayerEnter - 이미 클리어된 상태.");
            return;
        }
        StartRest();
        // 휴식방은 들어오면 바로 클리어 처리
        // Debug.Log($"[휴식방] {_room.gameObject.name}: OnPlayerEnter - 휴식 시작 후 바로 클리어 처리 요청.");
        processor.OnRoomCleared(new RoomClearedEvent { sender = processor, ClearedRoom = _room });
    }

    public void OnRoomCleared(RoomEventProcessor processor)
    {
        // Debug.Log($"[휴식방] {_room.gameObject.name}: 휴식 완료");
    }

    public void OnStateUpdate(RoomEventProcessor processor)
    {
        // OnStateUpdate에서 매번 클리어하던 로직 제거
        // if (!_room.IsCleared)
        // {
        //     processor.OnRoomCleared(null);
        // }
    }

    private void StartRest()
    {
        // TODO: 플레이어 체력 회복 로직
        // Debug.Log($"[휴식방] {_room.gameObject.name}: 휴식 시작");
    }
}