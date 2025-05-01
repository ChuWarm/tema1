using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{

    [ContextMenu("Run TestFunc")]
    void TestFunc()
    {
        var data = DataManager.GetData<EnemyData>("dummy_enemy");
        print(data);
    }

}
