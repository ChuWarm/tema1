using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IGameData { }

[System.Serializable]
public class EnemyData : IGameData
{
    public enum EnemyClass
    {
        Enforcer,   //
        Warden,     //
    }

    public enum PersionalityType
    {
        Aggressive,     //
    }

    public string enemyID;
    public string enemyName;
    public EnemyClass enemyClass;
    public PersionalityType persionalityType;

    public int health;
    public float moveSpeed;
    public int resistance;
    public int attackPower;
    public float attackSpeed;
    public float attackRange;
    public float attackCooldown;
    public int experienceGiven;
    public string dropTalbeID;
    public string visualResourceID;
    public string soundResourceID_Attack;
    public string soundResourceID_Hit;
    public string soundResourceID_Death;
    public string notes;
}

[System.Serializable]
public class EnemyDataSheet
{
    public EnemyData[] enemyDataSheet;
}