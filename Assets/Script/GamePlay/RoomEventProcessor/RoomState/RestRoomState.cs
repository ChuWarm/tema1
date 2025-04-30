using UnityEngine;

public class RestRoomState : IRoomState
{
    public void Enter(RoomEventProcessor processor)
    {
        Debug.Log("휴식 방: 체력 회복");
        processor.OnRoomCleared();
    }

    public void Update(RoomEventProcessor processor)
    {
        
    }
}