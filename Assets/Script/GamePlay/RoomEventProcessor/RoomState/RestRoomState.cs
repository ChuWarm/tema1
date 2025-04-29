using UnityEngine;

public class RestRoomState : IRoomState
{
    public void Enter(RoomEventProcessor processor)
    {
        Debug.Log("휴식 방: 체력 회복");
        processor.MarkRoomCleared();
    }

    public void Update(RoomEventProcessor processor)
    {
        
    }
}