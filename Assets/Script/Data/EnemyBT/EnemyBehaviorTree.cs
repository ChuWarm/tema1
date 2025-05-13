using System.Collections;
using System.Collections.Generic;
using Script.Characters;
using UnityEngine;

public abstract class BTNode : ScriptableObject
{
    public enum BTNodeState { Idle, Chase, Battle }
    [HideInInspector] public BTNodeState state;

    public abstract BTNodeState Evaluate(Enemy enemyBase);
}

[CreateAssetMenu(menuName = "AI/Enemy/BT")]
public class EnemyBehaviorTree : ScriptableObject
{
    public BTNode rootNode;
    public List<BTNode> children;

    public void Execute(Enemy enemyBase)
    {
        rootNode?.Evaluate(enemyBase);
    }
}
