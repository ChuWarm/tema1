using UnityEngine;
using System;

public interface IItemEffect
{
    void ApplyEffect(PlayerStats player);
    void RemoveEffect(PlayerStats player);
}

[System.Serializable]
public class StatModifier
{
    public enum ModifierType
    {
        Flat,       // 고정값 증가
        Percent     // 퍼센트 증가
    }

    public ModifierType type;
    public float value;
    public StatType statType;

    public enum StatType
    {
        Health,
        Stamina,
        AttackPower,
        MoveSpeed,
        Resistance
    }
}

public class ItemEffect : IItemEffect
{
    protected StatModifier[] modifiers;
    protected float duration;
    protected bool isPermanent;

    public ItemEffect(StatModifier[] modifiers, float duration = 0f)
    {
        this.modifiers = modifiers;
        this.duration = duration;
        this.isPermanent = duration <= 0f;
    }

    public virtual void ApplyEffect(PlayerStats player)
    {
        foreach (var modifier in modifiers)
        {
            switch (modifier.statType)
            {
                case StatModifier.StatType.Health:
                    if (modifier.type == StatModifier.ModifierType.Flat)
                        player.maxHealth += (int)modifier.value;
                    break;
                case StatModifier.StatType.Stamina:
                    if (modifier.type == StatModifier.ModifierType.Flat)
                        player.stamina += (int)modifier.value;
                    break;
                case StatModifier.StatType.AttackPower:
                    if (modifier.type == StatModifier.ModifierType.Flat)
                        player.attackPower += (int)modifier.value;
                    break;
                case StatModifier.StatType.MoveSpeed:
                    if (modifier.type == StatModifier.ModifierType.Flat)
                        player.moveSpeed += modifier.value;
                    break;
                case StatModifier.StatType.Resistance:
                    if (modifier.type == StatModifier.ModifierType.Flat)
                        player.resistance += (int)modifier.value;
                    break;
            }
        }
    }

    public virtual void RemoveEffect(PlayerStats player)
    {
        foreach (var modifier in modifiers)
        {
            switch (modifier.statType)
            {
                case StatModifier.StatType.Health:
                    if (modifier.type == StatModifier.ModifierType.Flat)
                        player.maxHealth -= (int)modifier.value;
                    break;
                case StatModifier.StatType.Stamina:
                    if (modifier.type == StatModifier.ModifierType.Flat)
                        player.stamina -= (int)modifier.value;
                    break;
                case StatModifier.StatType.AttackPower:
                    if (modifier.type == StatModifier.ModifierType.Flat)
                        player.attackPower -= (int)modifier.value;
                    break;
                case StatModifier.StatType.MoveSpeed:
                    if (modifier.type == StatModifier.ModifierType.Flat)
                        player.moveSpeed -= modifier.value;
                    break;
                case StatModifier.StatType.Resistance:
                    if (modifier.type == StatModifier.ModifierType.Flat)
                        player.resistance -= (int)modifier.value;
                    break;
            }
        }
    }
} 