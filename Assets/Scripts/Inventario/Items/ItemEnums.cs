using System;

[Serializable]
public enum ItemCategory
{
    Tool,
    Seed, 
    Fish,
    Crop,
    Consumable,
    Resource,
    Special
}

[Serializable]
public enum ToolType
{
    Hoe,
    Axe,
    Bucket,
    Shovel
}

[Serializable]
public enum SeedType
{
    CropSeed,
    TreeSeed
}

[Serializable]
public enum FishRarity
{
    Common,
    Uncommon,
    Rare,
    Legendary
}