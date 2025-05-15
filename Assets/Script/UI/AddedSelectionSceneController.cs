using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AddedSelectionSceneController : MonoBehaviour
{
    public enum AddedSelectionType
    {
        RoomCleared,
        Store,
    }

    public static AddedSelectionType calledType = 0;


    [SerializeField] GameObject roomClearRewardPanel;
    [SerializeField] GameObject storePanel;

    private void OnEnable()
    {
        GameObject enableTarget = null;
        switch (calledType)
        {
            case AddedSelectionType.RoomCleared:
                enableTarget = roomClearRewardPanel;
                break;
            case AddedSelectionType.Store:
                enableTarget = storePanel;
                break;
            default:
                break;
        }

        if (enableTarget.IsUnityNull())
            Debug.LogError($"씬 타입 호출 에러 {calledType}");

        enableTarget.SetActive(true);
    }
}
