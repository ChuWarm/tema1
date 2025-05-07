using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{
    [System.Serializable]
    public class LevelUpEvent : UnityEvent<int, int> { } // (oldLevel, newLevel)

    public LevelUpEvent OnLevelUp = new LevelUpEvent();

    public int playerLevel = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;
    
    public int maxHealth = 100;
    public int stamina = 10;
    public int currentHealth = 100;
    public int currentStamina = 10;

    public float moveSpeed = 3.5f;
    public int resistance = 10;
    public int attackPower = 10;
    public float attackSpeed = 0.8f;

    [Header("레벨업시 상승하는 스탯값")]
    public int healthIncreasePerLevel = 10;
    public int attackPowerIncreasePerLevel = 10;
    public int staminaIncreasePerLevel = 2;
    public int expToNextLevelIncrease = 10;

    private void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void GainExperience(int amount)
    {
        currentExp += amount;
        //계정 경험치 획득 반영
        LevelManager.Instance.GainAccountExperience(amount);

        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        int oldLevel = playerLevel;
        playerLevel++;
        currentExp -= expToNextLevel;
        expToNextLevel += expToNextLevelIncrease;
        
        maxHealth += healthIncreasePerLevel;
        attackPower += attackPowerIncreasePerLevel;
        stamina += staminaIncreasePerLevel;
        
        currentHealth = maxHealth;
        currentStamina = stamina;

        // 레벨업 이벤트 발생
        OnLevelUp.Invoke(oldLevel, playerLevel);
    }
}

[System.Serializable]
public class PlayerStatsSheet
{
    public PlayerStats[] playerStatsSheet;
}
