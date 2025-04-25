using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEventHolder : MonoBehaviour
{
    [SerializeField] int count;

    private void OnEnable()
    {
        for (int i = 0; i < count; i++)
            EnemyFactory.SpawnEnemy(this, DataManager.Instance.enemyDataDic["dummy_enemy"]);

    }
}
