using UnityEngine;

public abstract class SpecialItemData : InventoryItemData
{
    public override ItemCategory Category => ItemCategory.Special;
    
    public abstract override bool UseItem();

    public override ItemUseResult UseItem(ItemUseContext ctx)
    {
        if (UseItem())
        {
            return new ItemUseResult
            {
                Success = true,
                ShouldConsume = true,
                PlaySound = useItemSound != null
            };
        }
        return default;
    }
}