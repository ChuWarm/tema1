using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEventHolder : MonoBehaviour
{
    List<EnemyBase> m_enemies;
    
    // 추가된 멤버
    RoomEventProcessor m_roomEventProcessor;

    private void OnEnable()
    {
        // 추가된 코드
        m_roomEventProcessor  = GetComponent<RoomEventProcessor>();
        //
        
        m_enemies = new List<EnemyBase>();
    }

    // 추가된 함수
    public void SpawnEnemies(List<EnemyData> enemyDatas, List<Vector3> spawnPoints)
    {
        for (int i = 0; i < 3; i++)
        {
            var enemy = EnemyFactory.SpawnEnemy(this, enemyDatas[i], spawnPoints[i]);
            RegisterEnemy(enemy);
        }
    }

    // 추가된 함수
    public void RegisterEnemy(EnemyBase enemyBase)
    {
        if (enemyBase != null)
            m_enemies.Add(enemyBase);
    }

    // 추가된 함수
    public void OnEnemyDead(EnemyBase enemyBase)
    {
        m_enemies.Remove(enemyBase);
        
        if (m_enemies.Count == 0)
            m_roomEventProcessor?.OnRoomCleared();
    }
}
