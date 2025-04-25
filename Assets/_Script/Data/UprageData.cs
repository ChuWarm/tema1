using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UprageData
{
    public enum UprageType
    {
        Stat,
    }

    public string upgradeID;
    public string upgradeName;
    public UprageType upgradeType;
    public string upgradeIconID;
    public string targetStatOrSkill;
    public float upgradeValue;
    public int requiredMemoryPoint;
    public string prerequisiteUpgradeID;
    public string upgradeDescription;

}


[System.Serializable]
public class MemoryDataSheet
{
    public UprageData[] memoryUpgradeSheet;
}