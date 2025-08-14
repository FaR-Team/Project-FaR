using System.Collections.Generic;
using System.Linq;

public static class ItemTypeHelper
{
    private static readonly HashSet<TypeOfItem> ToolTypes = new HashSet<TypeOfItem>
    {
        TypeOfItem.Hoe, TypeOfItem.Axe, TypeOfItem.Bucket, TypeOfItem.Shovel
    };
    
    private static readonly HashSet<TypeOfItem> SeedTypes = new HashSet<TypeOfItem>
    {
        TypeOfItem.CropSeed, TypeOfItem.TreeSeed
    };
    
    public static bool IsToolType(TypeOfItem type) => ToolTypes.Contains(type);
    public static bool IsSeedType(TypeOfItem type) => SeedTypes.Contains(type);
    
    public static ItemCategory GetCategoryFromType(TypeOfItem type)
    {
        return type switch
        {
            TypeOfItem.Hoe or TypeOfItem.Axe or TypeOfItem.Bucket or TypeOfItem.Shovel => ItemCategory.Tool,
            TypeOfItem.CropSeed or TypeOfItem.TreeSeed => ItemCategory.Seed,
            TypeOfItem.Crop => ItemCategory.Crop,
            TypeOfItem.Special => ItemCategory.Special,
            _ => ItemCategory.Special
        };
    }
    
    public static T[] GetItemsOfType<T>(InventoryItemData[] items) where T : InventoryItemData
    {
        return items.OfType<T>().ToArray();
    }
}