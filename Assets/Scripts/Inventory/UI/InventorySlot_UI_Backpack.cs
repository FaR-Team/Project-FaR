using UnityEngine;
using UnityEngine.UI;

public class InventorySlot_UI_Backpack : InventorySlot_UIBasic
{
    private TypesOfInventory types;
    private Image thisSlotImage;

    public bool isSlotBlocked;

    protected override void Awake()
    {
        base.Awake();
        thisSlotImage = GetComponent<Image>();
    }
    private void BlockSlot()
    {
        if (types == TypesOfInventory.PANTALON && !PlayerStats.hasPants)
        {
            isSlotBlocked = true;
            thisSlotImage.color = Color.red;
        }
        if (types == TypesOfInventory.CAMISA && !PlayerStats.hasShirt)
        {
            isSlotBlocked = true;
            thisSlotImage.color = Color.blue;
        }
    }
    void OnEnable()
    {
        UnblockSlotOfPants();
        UnblockSlotOfShirt();
    }

    public void Init(InventorySlot slot, TypesOfInventory thisEnum)
    {
        base.Init(slot);
        types = thisEnum;
        BlockSlot();
    }
    public void UnblockSlotOfPants()
    {
        if (!PlayerStats.hasPants || types != TypesOfInventory.PANTALON) return;

        isSlotBlocked = false;
        thisSlotImage.color = Color.white;
    }

    public void UnblockSlotOfShirt()
    {
        if (!PlayerStats.hasShirt || types != TypesOfInventory.CAMISA) return;

        isSlotBlocked = false;
        thisSlotImage.color = Color.white;
    }

    public override void OnUISlotClick()
    {
        if (isSlotBlocked) return;

        ParentDisplay?.SlotClicked(this);
    }
}
