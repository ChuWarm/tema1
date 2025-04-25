using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class EnemyFactory
{
    public static EnemyBase SpawnEnemy(RoomEventHolder room, EnemyData enemyData)
    {
<<<<<<< Updated upstream
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
=======
        if (room.IsUnityNull()) return null;

        var basePrefab = Resources.Load<GameObject>("EnemyBase");

        MonoBehaviour.Instantiate(basePrefab, room.transform);

        if (!basePrefab.TryGetComponent<EnemyBase>(out var enemyBase))
            return null;

        return enemyBase.Init(enemyData);
    }
}


public class EnemyBase : MonoBehaviour
{
    [SerializeField] Transform visualHolder;

    string m_enemyID;
    int m_health;

    public EnemyBase Init(EnemyData data)
    {
        m_enemyID = data.enemyID;
        m_health = data.health;

        /*
        var visual = Resources.Load<Transform>($"{data.visualResourceID}");

        visual.SetParent(visualHolder);
        visual.transform.localPosition = Vector3.zero;
        */

        return this;
>>>>>>> Stashed changes
    }
}
