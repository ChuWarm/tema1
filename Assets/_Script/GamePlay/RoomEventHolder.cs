using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEventHolder : MonoBehaviour
{
    List<EnemyBase> m_enemies;

    private void OnEnable()
    {
        m_enemies = new List<EnemyBase>();

        for (int i = 0; i < 2; i++)
        {
            EnemyFactory.SpawnEnemy(this, DataManager.GetEnemyData("dummy_enemy"));
        }
    }


}
