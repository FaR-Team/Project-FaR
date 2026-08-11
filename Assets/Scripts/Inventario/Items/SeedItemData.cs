using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Jueguito Granjil/Inventario/SeedItem")]
[Serializable]
public class SeedItemData : InventoryItemData
{
    [Header("Seed Specific")]
    public GameObject DirtPrefab;
    public GameObject cropPrefab;
    public int growthTime = 3;
    
    public override ItemCategory Category => ItemCategory.Seed;
    
    public SeedType SeedType 
    { 
        get 
        {
            return typeOfItem switch
            {
                TypeOfItem.CropSeed => SeedType.CropSeed,
                TypeOfItem.TreeSeed => SeedType.TreeSeed,
                _ => SeedType.CropSeed
            };
        }
    }

    public override bool UseItem(Dirt dirt)
    {
        return typeOfItem == TypeOfItem.CropSeed && dirt.GetCrop(this);
    }

    public override bool UseItem()
    {
        Debug.Log("Called UseItem");
        if (typeOfItem != TypeOfItem.TreeSeed) return false;

        return GridGhost.instance.PlantTreeNear(DirtPrefab);
    }

    public override ItemUseResult UseItem(ItemUseContext ctx)
    {
        if (IsCropSeed() && ctx.Interactor != null && ctx.Interactor._LookingAtDirt)
        {
            if (ctx.DirtToTest != null && ctx.DirtToTest.IsEmpty && UseItem(ctx.DirtToTest))
            {
                return new ItemUseResult { Success = true, ShouldConsume = true };
            }
        }
        else if (IsTreeSeed() && ctx.Interactor != null && !ctx.Interactor._LookingAtDirt)
        {
            if (ctx.GridGhost != null && ctx.GridGhost.CheckCrop(ctx.GridGhost.finalPosition, 1))
            {
                if (UseItem())
                {
                    return new ItemUseResult { Success = true, ShouldConsume = true };
                }
            }
        }
        return default;
    }
}
