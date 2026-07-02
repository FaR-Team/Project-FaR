using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System;

public class InventorySlot_UIBasic : MonoBehaviour
{
    [SerializeField] protected Image itemSprite;
    [SerializeField] private TextMeshProUGUI itemCount;
    [SerializeField] protected GameObject _slotHighlight;
    [SerializeField] protected InventorySlot assignedInventorySlot;


    private bool isHighlighted;
    public bool _IsHighlighted => isHighlighted;

    private UIButton button;
    public InventorySlot AssignedInventorySlot => assignedInventorySlot;
    public InventoryDisplay ParentDisplay { get; private set; }

    public void SetParentDisplay(InventoryDisplay parent)
    {
        ParentDisplay = parent;
    }

    protected virtual void Awake()
    {
        ClearSlot();

        button = GetComponent<UIButton>();
        button?.onClick.AddListener(OnUISlotClick);
        button?.onRightClick.AddListener(OnUISlotRightClick);
        button?.onMouseOver.AddListener(OnUISlotMouseOver);
        button?.onMouseExit.AddListener(OnUISlotMouseExit);
        itemSprite.preserveAspect = true;

        ParentDisplay = GetComponentInParent<InventoryDisplay>();
    }

    public virtual void UpdateUISlot(InventorySlot slot)
    {
        if (slot.ItemData != null)
        {
            itemSprite.sprite = slot.ItemData.Icono;
            itemSprite.color = Color.white;

            if (slot.StackSize > 1)
            {
                itemCount.text = slot.StackSize.ToString();
            }
            else
            {
                itemCount.text = "";
            }
        }

        else
        {
            ClearSlot();
        }
    }

    public virtual void Init(InventorySlot slot)
    {
        assignedInventorySlot = slot;
        slot.UISlot = this;
        UpdateUISlot(slot);
    }

    public virtual void UpdateUISlot()
    {
        if (assignedInventorySlot == null) return;

        UpdateUISlot(assignedInventorySlot);
    }

    public void ClearSlot()
    {
        assignedInventorySlot?.ForcedClearSlot();
        itemSprite.sprite = null;
        itemSprite.color = Color.clear;
        itemCount.text = "";
    }

    public virtual void ToggleHighlight()
    {
        isHighlighted = !_slotHighlight.activeInHierarchy;
        _slotHighlight.SetActive(isHighlighted);
    }

    public virtual void OnUISlotClick()
    {
        if (ParentDisplay == null) ParentDisplay = GetComponentInParent<InventoryDisplay>();
        ParentDisplay?.SlotClicked(this);
    }

    public virtual void OnUISlotRightClick()
    {
        if (ParentDisplay == null) ParentDisplay = GetComponentInParent<InventoryDisplay>();
        ParentDisplay?.SlotClicked(this, true);
    }

    private void OnUISlotMouseExit()
    {
        InventoryUIController.instance.hoveredUISlot = null;
    }

    private void OnUISlotMouseOver()
    {
        InventoryUIController.instance.hoveredUISlot = this;
    }

    public Task SaveSlot()
    {
        return Task.CompletedTask;
    }
}

