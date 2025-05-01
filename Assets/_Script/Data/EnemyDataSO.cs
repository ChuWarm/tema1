using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyData;

public class EnemyDataSO : ScriptableObject
{
    public string enemyID;
    public string enemyName;
    public EnemyClass enemyClass;
    public PersionalityType persionalityType;

    public int health;
    public float moveSpeed;
    public int resistance;
    public float attackPower;
    public float attackSpeed;
    public int experienceGiven;
    public string dropTalbeID;
    public string visualResourceID;
    public string soundResourceID_Attack;
    public string soundResourceID_Hit;
    public string soundResourceID_Death;
    public string notes;


}
