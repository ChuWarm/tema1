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

        // 변경
        // DataManager dataM = FindObjectOfType<DataManager>();
        //
        // for (int i = 0; i < 2; i++)
        // {
        //     EnemyFactory.SpawnEnemy(this, dataM.GetEnemyData("dummy_enemy"));
        // }
        //
    }

    // 추가된 함수
    public void RegisterEnmey(EnemyBase enemyBase)
    {
        if (enemyBase != null)
            m_enemies.Add(enemyBase);
    }
    
    // 추가된 함수
    public void OnEnemyDead(EnemyBase enemyBase)
    {
        if (m_enemies.Contains(enemyBase))
            m_enemies.Remove(enemyBase);
        
        // 방에 남은 적이 모두 죽었으면 알림
        if (m_enemies.Count == 0)
            m_roomEventProcessor?.OnEnemyDead();
    }
}
