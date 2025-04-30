using UnityEngine;

public class ShopRoomState : IRoomState
{
    public void Enter(RoomEventProcessor processor)
    {
        Debug.Log("상점 오픈");
        processor.OnRoomCleared();
    }

    public void Update(RoomEventProcessor processor)
    {
        
    }
}