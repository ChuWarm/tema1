using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour, IDamageable
{
    public static PlayerManager Instance { get; private set; }

    [System.Serializable]
    public class LevelUpEvent : UnityEvent<int, int> { } // (oldLevel, newLevel)
    [System.Serializable]
    public class EquipmentChangedEvent : UnityEvent<ItemData, bool> { } // (item, isEquipped)
    [System.Serializable]
    public class StatsChangedEvent : UnityEvent { }

    public LevelUpEvent OnLevelUp = new LevelUpEvent();
    public EquipmentChangedEvent OnEquipmentChanged = new EquipmentChangedEvent();
    public StatsChangedEvent OnStatsChanged = new StatsChangedEvent();

    [Header("기본 스탯")]
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

    [Header("장비 슬롯")]
    [SerializeField] private Dictionary<string, ItemData> equippedItems = new Dictionary<string, ItemData>();
    [SerializeField] private List<StatModifier> equipmentModifiers = new List<StatModifier>();

    [Header("전투 설정")]
    public float attackRange = 2f;
    public float attackCooldown = 0.5f;
    private float lastAttackTime;
    private bool isDead;

    [Header("전투 UI")]
    public GameObject healthBarPrefab;
    public GameObject staminaBarPrefab;
    public GameObject damageTextPrefab;
    private HealthBar healthBar;
    private HealthBar staminaBar;

    [Header("전투 피드백")]
    public float hitStopDuration = 0.1f;
    public float hitShakeIntensity = 0.1f;
    public float hitShakeDuration = 0.2f;
    private Camera mainCamera;

    [System.Serializable]
    public class PlayerDeathEvent : UnityEvent { }
    public PlayerDeathEvent OnPlayerDeath = new PlayerDeathEvent();

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

    private void Start()
    {
        InitializePlayer();
        InitializeUI();
        mainCamera = Camera.main;
    }

    private void InitializePlayer()
    {
        currentHealth = maxHealth;
        currentStamina = stamina;
        InitializeEquipmentSlots();
    }

    private void InitializeEquipmentSlots()
    {
        // 기본 장비 슬롯 초기화
        equippedItems.Clear();
        equippedItems.Add("Weapon", null);
        equippedItems.Add("Armor", null);
        equippedItems.Add("Accessory", null);
    }

    private void InitializeUI()
    {
        if (healthBarPrefab != null)
        {
            GameObject healthBarObj = Instantiate(healthBarPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
            healthBar = healthBarObj.GetComponent<HealthBar>();
            healthBar.Initialize(maxHealth);
        }

        if (staminaBarPrefab != null)
        {
            GameObject staminaBarObj = Instantiate(staminaBarPrefab, transform.position + Vector3.up * 2.2f, Quaternion.identity);
            staminaBar = staminaBarObj.GetComponent<HealthBar>();
            staminaBar.Initialize(stamina);
        }
    }

    public bool EquipItem(ItemData item)
    {
        if (item == null || item.itemType != ItemData.ItemType.weapon) return false;

        // 이미 장착된 아이템이 있다면 해제
        if (equippedItems.ContainsKey("Weapon") && equippedItems["Weapon"] != null)
        {
            UnequipItem(equippedItems["Weapon"]);
        }

        // 새 아이템 장착
        equippedItems["Weapon"] = item;
        ApplyEquipmentEffects(item);
        OnEquipmentChanged?.Invoke(item, true);
        OnStatsChanged?.Invoke();
        return true;
    }

    public bool UnequipItem(ItemData item)
    {
        if (item == null) return false;

        // 장비 효과 제거
        RemoveEquipmentEffects(item);
        equippedItems["Weapon"] = null;
        OnEquipmentChanged?.Invoke(item, false);
        OnStatsChanged?.Invoke();
        return true;
    }

    private void ApplyEquipmentEffects(ItemData item)
    {
        if (string.IsNullOrEmpty(item.effect)) return;

        // 아이템 효과 파싱 및 적용
        string[] effects = item.effect.Split(',');
        foreach (string effect in effects)
        {
            string[] statValue = effect.Split(':');
            if (statValue.Length == 2)
            {
                var modifier = new StatModifier
                {
                    type = StatModifier.ModifierType.Flat,
                    value = float.Parse(statValue[1])
                };

                // 스탯 타입 매핑
                switch (statValue[0].ToLower())
                {
                    case "health":
                        modifier.statType = StatModifier.StatType.Health;
                        maxHealth += (int)modifier.value;
                        break;
                    case "stamina":
                        modifier.statType = StatModifier.StatType.Stamina;
                        stamina += (int)modifier.value;
                        break;
                    case "attackpower":
                        modifier.statType = StatModifier.StatType.AttackPower;
                        attackPower += (int)modifier.value;
                        break;
                    case "movespeed":
                        modifier.statType = StatModifier.StatType.MoveSpeed;
                        moveSpeed += modifier.value;
                        break;
                    case "resistance":
                        modifier.statType = StatModifier.StatType.Resistance;
                        resistance += (int)modifier.value;
                        break;
                }

                equipmentModifiers.Add(modifier);
            }
        }
    }

    private void RemoveEquipmentEffects(ItemData item)
    {
        if (string.IsNullOrEmpty(item.effect)) return;

        // 아이템 효과 제거
        string[] effects = item.effect.Split(',');
        foreach (string effect in effects)
        {
            string[] statValue = effect.Split(':');
            if (statValue.Length == 2)
            {
                float value = float.Parse(statValue[1]);
                switch (statValue[0].ToLower())
                {
                    case "health":
                        maxHealth -= (int)value;
                        break;
                    case "stamina":
                        stamina -= (int)value;
                        break;
                    case "attackpower":
                        attackPower -= (int)value;
                        break;
                    case "movespeed":
                        moveSpeed -= value;
                        break;
                    case "resistance":
                        resistance -= (int)value;
                        break;
                }

                // 수정자 목록에서 제거
                equipmentModifiers.RemoveAll(m => m.value == value);
            }
        }
    }

    public ItemData GetEquippedItem(string slot)
    {
        return equippedItems.ContainsKey(slot) ? equippedItems[slot] : null;
    }

    public Dictionary<string, ItemData> GetAllEquippedItems()
    {
        return new Dictionary<string, ItemData>(equippedItems);
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
        OnLevelUp?.Invoke(oldLevel, playerLevel);
        OnStatsChanged?.Invoke();
    }

    // 스탯 관련 유틸리티 메서드들
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        int actualDamage = Mathf.Max(1, damage - resistance);
        currentHealth = Mathf.Max(0, currentHealth - actualDamage);
        
        // 데미지 이펙트와 텍스트
        EffectManager.Instance.PlayEffect("Hit", transform.position, Quaternion.identity);
        EffectManager.Instance.ShowDamageText(actualDamage, transform.position + Vector3.up);
        
        // UI 업데이트
        healthBar?.UpdateValue(currentHealth);
        
        // 피드백 효과
        StartCoroutine(HitStop());
        StartCoroutine(CameraShake());
        
        OnStatsChanged?.Invoke();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private System.Collections.IEnumerator HitStop()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = 1f;
    }

    private System.Collections.IEnumerator CameraShake()
    {
        Vector3 originalPosition = mainCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < hitShakeDuration)
        {
            float x = Random.Range(-1f, 1f) * hitShakeIntensity;
            float y = Random.Range(-1f, 1f) * hitShakeIntensity;
            mainCamera.transform.localPosition = new Vector3(x, y, originalPosition.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = originalPosition;
    }

    private void Die()
    {
        isDead = true;
        OnPlayerDeath?.Invoke();
        
        // TODO: 죽음 이펙트 구현
        // TODO: 게임 오버 처리
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void Attack(IDamageable target)
    {
        if (!CanAttack() || isDead) return;

        lastAttackTime = Time.time;
        
        // 공격 이펙트
        EffectManager.Instance.PlayEffect("Attack", transform.position + transform.forward, transform.rotation);
        
        // 약간의 딜레이 후 데미지 적용
        StartCoroutine(DelayedDamage(target));
    }

    private System.Collections.IEnumerator DelayedDamage(IDamageable target)
    {
        yield return new WaitForSeconds(0.3f); // 애니메이션 타이밍에 맞춰 조정
        target.TakeDamage(attackPower);
    }

    public bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    public float GetAttackRange()
    {
        return attackRange;
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        
        // 힐 이펙트와 텍스트
        EffectManager.Instance.PlayEffect("Heal", transform.position, Quaternion.identity);
        EffectManager.Instance.ShowHealText(amount, transform.position + Vector3.up);
        
        // UI 업데이트
        healthBar?.UpdateValue(currentHealth);
        
        OnStatsChanged?.Invoke();
    }

    public void UseStamina(int amount)
    {
        currentStamina = Mathf.Max(0, currentStamina - amount);
        
        // UI 업데이트
        staminaBar?.UpdateValue(currentStamina);
        
        OnStatsChanged?.Invoke();
    }

    public void RestoreStamina(int amount)
    {
        currentStamina = Mathf.Min(stamina, currentStamina + amount);
        
        // UI 업데이트
        staminaBar?.UpdateValue(currentStamina);
        
        OnStatsChanged?.Invoke();
    }

    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    public float GetStaminaPercentage()
    {
        return (float)currentStamina / stamina;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}

[System.Serializable]
public class PlayerData
{
    public PlayerManager[] playerData;
} 