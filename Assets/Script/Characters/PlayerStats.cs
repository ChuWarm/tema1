using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int playerLevel;
    public int currentExp;
    public int expToNextLevel;
    
    public int maxHealth;
    public int stamina;
    public int currentHealth;
    public int currentStamina;

    public float moveSpeed;
    public int resistance;
    public int attackPower;
    public float attackSpeed;

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
        playerLevel++;
        currentExp -= expToNextLevel;
        expToNextLevel += 10;// 레벨업 요구량 증가
        
        maxHealth += 10;
        attackPower += 10;
        stamina += 2;
        
        currentHealth = maxHealth;
        currentStamina = stamina;
    }

}

[System.Serializable]
public class PlayerStatsSheet
{
    public PlayerStats[] playerStatsSheet;
}
