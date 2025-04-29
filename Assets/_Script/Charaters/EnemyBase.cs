using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class EnemyFactory
{
    public static EnemyBase SpawnEnemy(RoomEventHolder room, EnemyData enemyData)
    {
        if (room.IsUnityNull()) return null;

        var basePrefab = Resources.Load<GameObject>("EnemyBase");

        // 추가된 코드
        var instance = MonoBehaviour.Instantiate(basePrefab, room.transform);

        if (instance.TryGetComponent<EnemyBase>(out var enemyBase))
        {
            var enemy = enemyBase.Init(enemyData);
            room.RegisterEnmey(enemy);
            return enemy;
        }
        
        return null;
        //
        
        // MonoBehaviour.Instantiate(basePrefab, room.transform);
        //
        // if (!basePrefab.TryGetComponent<EnemyBase>(out var enemyBase))
        //     return null;
        //
        // return enemyBase.Init(enemyData);

    }
}

public class EnemyBase : MonoBehaviour
{
    [SerializeField] Transform m_visualHolder;

    // 추가 된 멤버
    private bool m_isDead = false;
    
    string m_enemyID;
    int m_health;

    public EnemyBase Init(EnemyData data)
    {
        gameObject.name = data.enemyName;
        this.m_health = data.health;
        this.m_enemyID = data.enemyID;

        /*
        var visual = Resources.Load<Transform>($"{data.visualResourceID}");

        visual.SetParent(visualHolder);
        visual.transform.localPosition = Vector3.zero;
        */

        return this;    
    }
}
