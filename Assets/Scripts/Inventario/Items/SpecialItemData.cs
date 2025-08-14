using UnityEngine;

public abstract class SpecialItemData : InventoryItemData
{
    public override ItemCategory Category => ItemCategory.Special;
    
    public abstract override bool UseItem();
}