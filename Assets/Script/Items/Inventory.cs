using UnityEngine;
using System.Collections.Generic;
using System;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [SerializeField] private int maxSlots = 20;
    public int MaxSlots => maxSlots;
    private Dictionary<string, InventoryItem> items = new Dictionary<string, InventoryItem>();

    public class InventoryItem
    {
        public ItemData itemData;
        public int quantity;
        public bool isEquipped;

        public InventoryItem(ItemData data, int count = 1)
        {
            itemData = data;
            quantity = count;
            isEquipped = false;
        }
    }

    public event Action<InventoryItem> OnItemAdded;
    public event Action<InventoryItem> OnItemRemoved;
    public event Action<InventoryItem> OnItemEquipped;
    public event Action<InventoryItem> OnItemUnequipped;
    public event Action OnItemsSwapped;

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

    public bool AddItem(ItemData item, int quantity = 1)
    {
        if (items.Count >= maxSlots && !items.ContainsKey(item.itemID))
        {
            Debug.LogWarning("인벤토리가 가득 찼습니다!");
            return false;
        }

        if (items.TryGetValue(item.itemID, out InventoryItem existingItem))
        {
            existingItem.quantity += quantity;
            OnItemAdded?.Invoke(existingItem);
        }
        else
        {
            var newItem = new InventoryItem(item, quantity);
            items.Add(item.itemID, newItem);
            OnItemAdded?.Invoke(newItem);
        }

        return true;
    }

    public bool RemoveItem(string itemId, int quantity = 1)
    {
        if (items.TryGetValue(itemId, out InventoryItem item))
        {
            item.quantity -= quantity;
            if (item.quantity <= 0)
            {
                items.Remove(itemId);
                OnItemRemoved?.Invoke(item);
            }
            return true;
        }
        return false;
    }

    public bool EquipItem(string itemId)
    {
        if (items.TryGetValue(itemId, out InventoryItem item))
        {
            if (item.itemData.itemType == ItemData.ItemType.weapon)
            {
                item.isEquipped = true;
                OnItemEquipped?.Invoke(item);
                return true;
            }
        }
        return false;
    }

    public bool UnequipItem(string itemId)
    {
        if (items.TryGetValue(itemId, out InventoryItem item))
        {
            if (item.isEquipped)
            {
                item.isEquipped = false;
                OnItemUnequipped?.Invoke(item);
                return true;
            }
        }
        return false;
    }

    public void SwapItems(string itemId1, string itemId2)
    {
        if (string.IsNullOrEmpty(itemId1) || string.IsNullOrEmpty(itemId2)) return;

        if (items.TryGetValue(itemId1, out InventoryItem item1) && 
            items.TryGetValue(itemId2, out InventoryItem item2))
        {
            // 아이템 데이터 스왑
            var tempData = item1.itemData;
            item1.itemData = item2.itemData;
            item2.itemData = tempData;

            // 수량 스왑
            var tempQuantity = item1.quantity;
            item1.quantity = item2.quantity;
            item2.quantity = tempQuantity;

            // 장착 상태 스왑
            var tempEquipped = item1.isEquipped;
            item1.isEquipped = item2.isEquipped;
            item2.isEquipped = tempEquipped;

            OnItemsSwapped?.Invoke();
        }
    }

    public InventoryItem GetItem(string itemId)
    {
        items.TryGetValue(itemId, out InventoryItem item);
        return item;
    }

    public Dictionary<string, InventoryItem> GetAllItems()
    {
        return new Dictionary<string, InventoryItem>(items);
    }
} 