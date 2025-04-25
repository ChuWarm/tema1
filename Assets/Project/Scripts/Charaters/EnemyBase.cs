using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class EnemyFactory
{
    public static EnemyBase SpawnEnemy(RoomEventHolder room, EnemyData enemyData)
    {
        var enemyBasePrefab = Resources.Load("EnemyBase_Prefab");

        var go = MonoBehaviour.Instantiate(enemyBasePrefab, room.transform);
        
        return go.GetComponent<EnemyBase>().Init(enemyData);
    }
}

public class EnemyBase : MonoBehaviour
{
    [SerializeField] Transform m_visualHolder;

    string m_enemyID;
    int m_health;
    
    public EnemyBase Init(EnemyData data)
    {
        gameObject.name = data.enemyName;
        m_health = data.health;
        m_enemyID = data.enemyID;

        // 비주얼 적용
        // m_visualHolder

        return this;    
    }
}
