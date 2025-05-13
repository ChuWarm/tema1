using UnityEngine;

public class SpawnRoomState : IRoomState
{
    private readonly RoomEventProcessor _processor;
    private readonly Room _room;

    public SpawnRoomState(RoomEventProcessor processor)
    {
        _processor = processor;
        _room = processor.GetRoom();
    }

    public void OnStateEnter(RoomEventProcessor processor)
    {
        Debug.Log($"[스폰방] {_room.gameObject.name}: 시작 지점 준비");
    }

    public void OnStateExit(RoomEventProcessor processor)
    {
        Debug.Log($"[스폰방] {_room.gameObject.name}: 시작 지점 종료");
    }

    public void OnPlayerEnter(RoomEventProcessor processor)
    {
        if (_room.IsCleared) return;
        InitializeSpawnPoint();
    }

    public void OnRoomCleared(RoomEventProcessor processor)
    {
        Debug.Log($"[스폰방] {_room.gameObject.name}: 시작 지점 활성화");
    }

    public void OnStateUpdate(RoomEventProcessor processor)
    {
        if (!_room.IsCleared)
        {
            processor.OnRoomCleared(null);
        }
    }

    private void InitializeSpawnPoint()
    {
        Debug.Log($"[스폰방] {_room.gameObject.name}: 시작 지점 초기화");
    }
}
