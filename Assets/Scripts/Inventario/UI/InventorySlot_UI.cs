using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot_UI : MonoBehaviour
{
    [SerializeField] private Image itemSprite, thisSlotImage;
    [SerializeField] private TextMeshProUGUI itemCount;
    [SerializeField] protected GameObject _slotHighlight;
    [SerializeField] private InventorySlot assignedInventorySlot;
    [SerializeField] private TypesOfInventory invEnums;

    public bool isSlotBlocked;

    private Button button;
    private TypesOfInventory types;
    public InventorySlot AssignedInventorySlot => assignedInventorySlot;
    public InventoryDisplay ParentDisplay { get; private set; }


    private void Awake()
    {
        ClearSlot();
        
        button = GetComponent<Button>();
        thisSlotImage = GetComponent<Image>();
        button?.onClick.AddListener(OnUISlotClick);
        itemSprite.preserveAspect = true;

        ParentDisplay = transform.parent.GetComponent<InventoryDisplay>();


    }

    void OnEnable(){
        UnblockSlotOfPants();
        UnblockSlotOfShirt();
        
    }
    public void Init(InventorySlot slot, TypesOfInventory thisEnum)
    {
        assignedInventorySlot = slot;
        UpdateUISlot(slot);
        types = thisEnum;
        BlockSlot();
    }

    private void BlockSlot(){
        if (types == TypesOfInventory.PANTALON && !PlayerStats.hasPants)
        {
            isSlotBlocked = true;
            thisSlotImage.color = Color.red;
        }
        if(types == TypesOfInventory.CAMISA && !PlayerStats.hasShirt) 
        {
            isSlotBlocked = true;
            thisSlotImage.color = Color.blue;
        }
    }
    public void UnblockSlotOfPants(){
        if(!PlayerStats.hasPants || types != TypesOfInventory.PANTALON) return;

        isSlotBlocked = false;
        thisSlotImage.color = Color.white;
    }
    
    public void UnblockSlotOfShirt(){
        if(!PlayerStats.hasShirt || types != TypesOfInventory.CAMISA) return;
        
        isSlotBlocked = false;
        thisSlotImage.color = Color.white;
    }
    public void UpdateUISlot(InventorySlot slot)
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

    public void UpdateUISlot()
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
        _slotHighlight.SetActive(!_slotHighlight.activeInHierarchy);
    }

    public void OnUISlotClick()
    {
        if (isSlotBlocked) return;

        ParentDisplay?.SlotClicked(this);
    }
}

public enum TypesOfInventory
{
    INVENTARIO, PANTALON, CAMISA
}