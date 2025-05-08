using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform slotsParent;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private GameObject itemInfoPanel;
    [SerializeField] private TextMeshProUGUI tooltipNameText;
    [SerializeField] private TextMeshProUGUI tooltipDescriptionText;
    [SerializeField] private TextMeshProUGUI tooltipStatsText;

    private List<InventorySlot> slots = new List<InventorySlot>();
    private bool isInventoryOpen;

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

        InitializeInventory();
    }

    private void Start()
    {
        // 인벤토리 이벤트 구독
        Inventory.Instance.OnItemAdded += OnItemAdded;
        Inventory.Instance.OnItemRemoved += OnItemRemoved;
        Inventory.Instance.OnItemEquipped += OnItemEquipped;
        Inventory.Instance.OnItemUnequipped += OnItemUnequipped;

        // 초기 UI 상태 설정
        inventoryPanel.SetActive(false);
        tooltipPanel.SetActive(false);
        itemInfoPanel.SetActive(false);
    }

    private void Update()
    {
        // 인벤토리 토글 (I 키)
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    private void InitializeInventory()
    {
        // 인벤토리 슬롯 생성
        for (int i = 0; i < Inventory.Instance.MaxSlots; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotsParent);
            InventorySlot slot = slotObj.GetComponent<InventorySlot>();
            slots.Add(slot);
        }
    }

    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);
        
        if (isInventoryOpen)
        {
            UpdateInventoryUI();
        }
        else
        {
            HideTooltip();
            itemInfoPanel.SetActive(false);
        }
    }

    private void UpdateInventoryUI()
    {
        var items = Inventory.Instance.GetAllItems();
        int slotIndex = 0;

        // 모든 슬롯 초기화
        foreach (var slot in slots)
        {
            slot.ClearSlot();
        }

        // 아이템으로 슬롯 채우기
        foreach (var item in items.Values)
        {
            if (slotIndex < slots.Count)
            {
                slots[slotIndex].SetItem(item);
                slotIndex++;
            }
        }
    }

    public void ShowTooltip(Inventory.InventoryItem item, Vector3 position)
    {
        tooltipPanel.SetActive(true);
        tooltipPanel.transform.position = position;

        tooltipNameText.text = item.itemData.itemName;
        tooltipDescriptionText.text = item.itemData.description;
        
        // 아이템 효과 표시
        string statsText = "";
        if (!string.IsNullOrEmpty(item.itemData.effect))
        {
            string[] effects = item.itemData.effect.Split(',');
            foreach (string effect in effects)
            {
                statsText += effect + "\n";
            }
        }
        tooltipStatsText.text = statsText;
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }

    public void ShowItemInfo(Inventory.InventoryItem item)
    {
        itemInfoPanel.SetActive(true);
        // 아이템 상세 정보 표시 로직
    }

    #region Event Handlers
    private void OnItemAdded(Inventory.InventoryItem item)
    {
        if (isInventoryOpen)
        {
            UpdateInventoryUI();
        }
    }

    private void OnItemRemoved(Inventory.InventoryItem item)
    {
        if (isInventoryOpen)
        {
            UpdateInventoryUI();
        }
    }

    private void OnItemEquipped(Inventory.InventoryItem item)
    {
        if (isInventoryOpen)
        {
            UpdateInventoryUI();
        }
    }

    private void OnItemUnequipped(Inventory.InventoryItem item)
    {
        if (isInventoryOpen)
        {
            UpdateInventoryUI();
        }
    }
    #endregion

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (Inventory.Instance != null)
        {
            Inventory.Instance.OnItemAdded -= OnItemAdded;
            Inventory.Instance.OnItemRemoved -= OnItemRemoved;
            Inventory.Instance.OnItemEquipped -= OnItemEquipped;
            Inventory.Instance.OnItemUnequipped -= OnItemUnequipped;
        }
    }
} 