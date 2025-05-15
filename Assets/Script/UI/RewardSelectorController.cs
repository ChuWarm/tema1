using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RewardSelectorController : MonoBehaviour
{
    [SerializeField] List<RewardSO> rewardDatas;
    [SerializeField] GameObject rewardSelectorSlot_PREFAB;

    private void OnEnable()
    {
        for (int i = 0; i < rewardDatas.Count; i++)
        {
            var go = Instantiate(rewardSelectorSlot_PREFAB, transform);
            go.TryGetComponent<RewardSelectorSlot>(out var slot);

            if (slot.IsUnityNull())
                Debug.LogError($"Prefab Error: {rewardSelectorSlot_PREFAB}");

            var data = rewardDatas[i];
            slot.Set(data);
            slot.onSubmitCallback += () =>
            {
                foreach (Transform item in transform)
                    Destroy(item.gameObject);

                GameEventBus.Publish<UserSelectDoneEvent>(new UserSelectDoneEvent());
            };
        }
    }
}
