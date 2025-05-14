using UnityEngine;

public class ShopRoomState : IRoomState
{
    private readonly RoomEventProcessor _processor;
    private readonly Room _room;

    public ShopRoomState(RoomEventProcessor processor)
    {
        _processor = processor;
        _room = processor.GetRoom();
    }

    public void OnStateEnter(RoomEventProcessor processor)
    {
        // Debug.Log($"[상점방] {_room.gameObject.name}: 상점 준비");
    }

    public void OnStateExit(RoomEventProcessor processor)
    {
        // Debug.Log($"[상점방] {_room.gameObject.name}: 상점 종료");
    }

    public void OnPlayerEnter(RoomEventProcessor processor)
    {
        if (_room.IsCleared) 
        {
            // Debug.Log($"[상점방] {_room.gameObject.name}: OnPlayerEnter - 이미 클리어된 상태.");
            return;
        }
        OpenShop();
        // 상점은 들어오면 바로 클리어 처리
        // Debug.Log($"[상점방] {_room.gameObject.name}: OnPlayerEnter - 상점 열고 바로 클리어 처리 요청.");
        processor.OnRoomCleared(new RoomClearedEvent { sender = processor, ClearedRoom = _room }); 
    }

    public void OnRoomCleared(RoomEventProcessor processor)
    {
        // Debug.Log($"[상점방] {_room.gameObject.name}: 상점 이용 완료");
    }

    public void OnStateUpdate(RoomEventProcessor processor)
    {
        // OnStateUpdate에서 매번 클리어하던 로직 제거
        // if (!_room.IsCleared)
        // {
        //     processor.OnRoomCleared(null);
        // }
    }

    private void OpenShop()
    {
        // TODO: 상점 UI 표시 로직
        // Debug.Log($"[상점방] {_room.gameObject.name}: 상점 시작");
    }
}