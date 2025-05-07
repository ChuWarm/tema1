using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData : IGameData
{
    public enum ItemType
    {
        weapon,
        comsumalble,
    }


    public string itemID;
    public string itemName;
    public ItemType itemType;
    public string ammo;
    public string effect;
    public float effectTime;
    public int value;
    public string description;
    public string iconID;
}

[System.Serializable]   
public class ItemDataSheet
{
    public ItemData[] itemDataSheet;
}