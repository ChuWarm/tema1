using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, 
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private GameObject equippedIndicator;
    [SerializeField] private GameObject selectedIndicator;
    [SerializeField] private Image slotBackground;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private Color dropZoneColor = new Color(0.5f, 1f, 0.5f, 0.5f);

    [Header("Sound Effects")]
    [SerializeField] private AudioClip dragStartSound;
    [SerializeField] private AudioClip dropSound;
    [SerializeField] private AudioClip invalidDropSound;

    private Inventory.InventoryItem item;
    private bool isSelected;
    private static InventorySlot draggedSlot;
    private static GameObject draggedIcon;
    private static AudioSource audioSource;

    private void Awake()
    {
        if (draggedIcon == null)
        {
            // 드래그 중인 아이템을 표시할 UI 생성
            draggedIcon = new GameObject("DraggedItem");
            draggedIcon.transform.SetParent(transform.root);
            Image draggedImage = draggedIcon.AddComponent<Image>();
            draggedImage.raycastTarget = false;
            draggedIcon.SetActive(false);

            // 오디오 소스 생성
            audioSource = draggedIcon.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    public void SetItem(Inventory.InventoryItem newItem)
    {
        item = newItem;
        UpdateUI();
    }

    public void ClearSlot()
    {
        item = null;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (item != null)
        {
            itemIcon.sprite = Resources.Load<Sprite>($"Icons/{item.itemData.iconID}");
            itemIcon.enabled = true;
            quantityText.text = item.quantity > 1 ? item.quantity.ToString() : "";
            equippedIndicator.SetActive(item.isEquipped);
        }
        else
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
            quantityText.text = "";
            equippedIndicator.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item != null && !eventData.dragging)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                // 왼쪽 클릭: 아이템 사용/장착
                ItemManager.Instance.UseItem(item.itemData.itemID, PlayerStats.Instance);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                // 오른쪽 클릭: 아이템 정보 표시
                InventoryUI.Instance.ShowItemInfo(item);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null && draggedSlot == null)
        {
            // 아이템 툴팁 표시
            InventoryUI.Instance.ShowTooltip(item, transform.position);
        }
        else if (draggedSlot != null && draggedSlot != this)
        {
            // 드래그 중일 때 다른 슬롯에 마우스를 올리면 하이라이트
            slotBackground.color = dropZoneColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (draggedSlot == null)
        {
            // 툴팁 숨기기
            InventoryUI.Instance.HideTooltip();
        }
        // 하이라이트 제거
        slotBackground.color = normalColor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            draggedSlot = this;
            draggedIcon.SetActive(true);
            draggedIcon.GetComponent<Image>().sprite = itemIcon.sprite;
            itemIcon.enabled = false;
            
            // 드래그 시작 시 툴팁 숨기기
            InventoryUI.Instance.HideTooltip();

            // 드래그 시작 사운드 재생
            if (dragStartSound != null)
            {
                audioSource.PlayOneShot(dragStartSound);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedSlot != null)
        {
            draggedIcon.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedSlot != null)
        {
            draggedIcon.SetActive(false);
            draggedSlot.itemIcon.enabled = true;
            draggedSlot = null;

            // 모든 슬롯의 하이라이트 제거
            foreach (var slot in FindObjectsOfType<InventorySlot>())
            {
                slot.slotBackground.color = normalColor;
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (draggedSlot != null && draggedSlot != this)
        {
            // 아이템 스왑
            var tempItem = item;
            SetItem(draggedSlot.item);
            draggedSlot.SetItem(tempItem);

            // 인벤토리 데이터 업데이트
            Inventory.Instance.SwapItems(draggedSlot.item?.itemData.itemID, item?.itemData.itemID);

            // 드롭 사운드 재생
            if (dropSound != null)
            {
                audioSource.PlayOneShot(dropSound);
            }
        }
        else
        {
            // 유효하지 않은 드롭 사운드 재생
            if (invalidDropSound != null)
            {
                audioSource.PlayOneShot(invalidDropSound);
            }
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        selectedIndicator.SetActive(selected);
    }
} 