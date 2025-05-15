using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/RewardData/Gold")]
public class RewardSO_Gold : RewardSO
{
    public override void Excute()
    {
        // GameEventBus.Publish<PlayerGetGold>(new PlayerGetGold { gold = gold });
    }
}
