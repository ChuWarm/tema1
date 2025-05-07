using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRoomState
{
    void Enter(RoomEventProcessor processor);
    void Update(RoomEventProcessor processor);
    void Exit(RoomEventProcessor processor);
}
