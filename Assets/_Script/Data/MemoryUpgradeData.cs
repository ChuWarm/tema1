using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MemoryUpgradeData
{
    public enum UpgradeTpye
    {
        Stat
    }

    public string upgradeID;
    public string upgradeName;
    public UpgradeTpye upgradeType;
    public string upgradeIconID;
    public string targetStatOrSkill;
    public float upgradeValue;
    public int requiredMemoryPoint;
    public string prerequisiteUpgradeID;
    public string upgradeDescription;
}


[System.Serializable]
public class MemoryUpgradeDataSheet
{
    public MemoryUpgradeData[] memoryUpgradeSheet;
}