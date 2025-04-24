using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int PlayerLevel = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;
    
    public int maxHealth = 100;
    public int stamina = 10;
    public int currentHealth = 100;
    public int currentstamina = 10;

    public float moveSpeed = 3.5f;
    public int resistance = 10;
    public int attackPower = 10;
    public float attackSpeed = 0.8f;

    private void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void GainExperience(int amount)
    {
        currentExp += amount;
        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        PlayerLevel++;
        currentExp -= expToNextLevel;
        expToNextLevel += 10;// 레벨업 요구량 증가
        
        maxHealth += 10;
        attackPower += 10;
        stamina += 2;
        
        currentHealth = maxHealth;
        currentstamina = stamina;
    }

}
