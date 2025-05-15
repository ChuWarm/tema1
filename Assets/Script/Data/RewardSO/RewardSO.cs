using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RewardSO : ScriptableObject
{
    public string rewardTitle;
    public string rewardDescription;
    public Sprite rewardIcon;
    public int amount;

    public abstract void Excute();
}
