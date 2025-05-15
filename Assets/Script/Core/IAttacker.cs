using UnityEngine;

public interface IAttacker
{
    void Attack(IDamageable target);
    bool CanAttack();
    float GetAttackRange();
    int GetAttackDamage();
} 