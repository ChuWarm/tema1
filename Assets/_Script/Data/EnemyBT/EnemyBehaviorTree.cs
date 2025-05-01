using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BTNode : ScriptableObject
{
    public enum BTNodeState { Idle, Chase, Battle }
    [HideInInspector] public BTNodeState state;

    public abstract BTNodeState Evaluate(EnemyBase enemyBase);
}

[CreateAssetMenu(menuName = "AI/Enemy/BT")]
public class EnemyBehaviorTree : ScriptableObject
{
    public BTNode rootNode;
    public List<BTNode> children;

    public void Execute(EnemyBase enemyBase)
    {
        rootNode?.Evaluate(enemyBase);
    }
}
