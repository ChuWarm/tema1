using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damage);
    bool IsDead();
    Transform GetTransform();
} 