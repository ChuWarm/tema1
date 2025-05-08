using UnityEngine;
using UnityEngine.Events;
using Script.Core;

namespace Script.Characters
{
    [System.Serializable]
    public class LevelUpEvent : UnityEvent<int, int> { } // (oldLevel, newLevel)

    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [Header("레벨 설정")]
        [SerializeField] private int accountLevel = 1;
        [SerializeField] private int currentExp = 0;
        [SerializeField] private int expToNextLevel = 500;
        [SerializeField] private int expIncreasePerLevel = 100;

        public LevelUpEvent OnLevelUp = new LevelUpEvent();

        public int AccountLevel => accountLevel;
        public int CurrentExp => currentExp;
        public int ExpToNextLevel => expToNextLevel;
        public float ExpProgress => (float)currentExp / expToNextLevel;

        private void Awake()
        {
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
            if (amount <= 0) return;

            int oldLevel = accountLevel;
            currentExp += amount;

            while (currentExp >= expToNextLevel)
            {
                LevelUp();
            }

            if (oldLevel != accountLevel)
            {
                OnLevelUp?.Invoke(oldLevel, accountLevel);
            }
        }

        private void LevelUp()
        {
            accountLevel++;
            currentExp -= expToNextLevel;
            expToNextLevel += expIncreasePerLevel;
        }
    }
}
