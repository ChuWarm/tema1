using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesterScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            GameEventBus.Publish<RoomClearedEvent>(new RoomClearedEvent());
    }

    [ContextMenu("PublishRoomClearEvent")]
    void PublishRoomClearEvent()
    {
        GameEventBus.Publish<RoomClearedEvent>(new RoomClearedEvent());
    }
}
