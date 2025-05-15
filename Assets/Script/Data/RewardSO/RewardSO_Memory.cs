using Script.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/RewardData/Memory")]
public class RewardSO_Memory : RewardSO
{
    public override void Excute()
    {
        // PlayerManager.Instance.
        //GameEventBus.Publish<PlayerHPChanged>(new PlayerHPChanged { });

        Debug.Log($"±â¾ïÁ¶°¢À» {amount} ¸¸Å­ È¹µæ");
    }
}
