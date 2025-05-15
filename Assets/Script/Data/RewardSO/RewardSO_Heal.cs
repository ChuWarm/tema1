using Script.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/RewardData/Heal")]
public class RewardSO_Heal : RewardSO
{
    public override void Excute()
    {
        PlayerManager.Instance.Heal(amount);

        //GameEventBus.Publish<PlayerHPChanged>(new PlayerHPChanged { });
    }
}
