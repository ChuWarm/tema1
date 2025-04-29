using UnityEngine;

public class ShopRoomState : IRoomState
{
    public void Enter(RoomEventProcessor processor)
    {
        Debug.Log("상점 오픈");
        processor.MarkRoomCleared();
    }

    public void Update(RoomEventProcessor processor)
    {
        
    }
}