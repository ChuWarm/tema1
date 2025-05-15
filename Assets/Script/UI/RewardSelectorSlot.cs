using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardSelectorSlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI title;
    public TextMeshProUGUI desc;
    public Button submit;

    RewardSO myReward;

    public Action onSubmitCallback;

    public void Set(RewardSO rewardData)
    {
        myReward = rewardData;

        icon.sprite = myReward.rewardIcon;
        title.text = myReward.rewardTitle;
        desc.text = myReward.rewardDescription;


        submit.onClick.AddListener(() =>
        {
            myReward.Excute();
            onSubmitCallback();
        });
    }
}
