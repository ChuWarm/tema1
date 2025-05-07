using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AIBrains
{
    EnemyBehaviorTree m_activeBT;
    EnemyBase m_EnemyBase;

    public AIBrains(EnemyBehaviorTree activeBT, EnemyBase enemyBase)
    {
        m_activeBT = activeBT;
        m_EnemyBase = enemyBase;
    }

    public void Update_BT()
    {
        if (m_activeBT != null)
        {
            m_activeBT.Execute(m_EnemyBase);
        }
    }
}
