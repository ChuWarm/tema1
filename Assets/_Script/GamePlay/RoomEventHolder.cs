using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEventHolder : MonoBehaviour
{
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    [SerializeField] int count;

    private void OnEnable()
    {
        for (int i = 0; i < count; i++)
            EnemyFactory.SpawnEnemy(this, DataManager.Instance.enemyDataDic["dummy_enemy"]);

=======
=======
>>>>>>> Stashed changes
    List<EnemyBase> m_enemies;

    private void Start()
    {
        m_enemies = new List<EnemyBase>();

        DataManager dataM = FindObjectOfType<DataManager>();

        for (int i = 0; i < 2; i++)
        {
            EnemyFactory.SpawnEnemy(this, dataM.GetEnemyData("dummy_enemy"));
        }
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
    }
}
