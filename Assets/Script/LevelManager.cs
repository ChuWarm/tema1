using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance; // 싱글톤

    public int accountLevel = 1; // 기억 레벨
    public int currentExp = 0;  // 현재 기억 경험치
    public int expToNextLevel = 500; // 다음 기억 레벨까지 필요한 경험치

    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GainAccountExperience(int amount)
    {
        currentExp += amount;

        // 기억 경험치가 다음 레벨 요구 경험치를 초과하면 레벨업
        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        accountLevel++;
        currentExp -= expToNextLevel;

        // 기억 레벨업에 따라 다음 기억 요구량 증가
        expToNextLevel += 100;
    }
}