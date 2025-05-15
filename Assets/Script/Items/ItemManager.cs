using UnityEngine;
using System.Collections.Generic;
using Script.Characters;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    private Dictionary<string, ItemEffect> activeEffects = new Dictionary<string, ItemEffect>();
    private Dictionary<string, float> effectTimers = new Dictionary<string, float>();

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

    private void Update()
    {
        // 효과 타이머 업데이트
        List<string> expiredEffects = new List<string>();
        foreach (var effect in effectTimers)
        {
            effectTimers[effect.Key] -= Time.deltaTime;
            if (effectTimers[effect.Key] <= 0)
            {
                expiredEffects.Add(effect.Key);
            }
        }

        // 만료된 효과 제거
        foreach (var effectId in expiredEffects)
        {
            RemoveEffect(effectId);
        }
    }

    public void UseItem(string itemId, PlayerManager player)
    {
        var itemData = DataManager.GetData<ItemData>(itemId);
        if (itemData == null) return;

        switch (itemData.itemType)
        {
            case ItemData.ItemType.comsumalble:
                UseConsumable(itemData, player);
                break;
            case ItemData.ItemType.weapon:
                EquipWeapon(itemData, player);
                break;
        }
    }

    private void UseConsumable(ItemData item, PlayerManager player)
    {
        // 아이템 효과 생성 및 적용
        var effect = CreateItemEffect(item);
        if (effect != null)
        {
            string effectId = $"{item.itemID}_{Time.time}";
            activeEffects[effectId] = effect;
            effect.ApplyEffect(player);

            // 지속 시간이 있는 경우 타이머 설정
            if (item.effectTime > 0)
            {
                effectTimers[effectId] = item.effectTime;
            }

            // 인벤토리에서 아이템 제거
            Inventory.Instance.RemoveItem(item.itemID);
        }
    }

    private void EquipWeapon(ItemData item, PlayerManager player)
    {
        // 이미 장착된 무기 해제
        var currentItems = Inventory.Instance.GetAllItems();
        foreach (var inventoryItem in currentItems.Values)
        {
            if (inventoryItem.isEquipped && inventoryItem.itemData.itemType == ItemData.ItemType.weapon)
            {
                Inventory.Instance.UnequipItem(inventoryItem.itemData.itemID);
                break;
            }
        }

        // 새 무기 장착
        Inventory.Instance.EquipItem(item.itemID);
    }

    private ItemEffect CreateItemEffect(ItemData item)
    {
        // 아이템 데이터의 effect 문자열을 파싱하여 StatModifier 배열 생성
        // 예시: "Health:10,AttackPower:5" 형식의 문자열을 파싱
        var modifiers = new List<StatModifier>();
        string[] effectParts = item.effect.Split(',');
        
        foreach (var part in effectParts)
        {
            string[] statValue = part.Split(':');
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
                        break;
                    case "stamina":
                        modifier.statType = StatModifier.StatType.Stamina;
                        break;
                    case "attackpower":
                        modifier.statType = StatModifier.StatType.AttackPower;
                        break;
                    case "movespeed":
                        modifier.statType = StatModifier.StatType.MoveSpeed;
                        break;
                    case "resistance":
                        modifier.statType = StatModifier.StatType.Resistance;
                        break;
                }

                modifiers.Add(modifier);
            }
        }

        return new ItemEffect(modifiers.ToArray(), item.effectTime);
    }

    private void RemoveEffect(string effectId)
    {
        if (activeEffects.TryGetValue(effectId, out ItemEffect effect))
        {
            // 효과 제거 로직
            activeEffects.Remove(effectId);
            effectTimers.Remove(effectId);
        }
    }
} 