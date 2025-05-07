using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int playerLevel = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;
    
    public int maxHealth = 100;
    public int stamina = 10;
    public int currentHealth = 100;
    public int currentStamina = 10;

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
