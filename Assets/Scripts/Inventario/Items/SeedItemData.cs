using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Jueguito Granjil/Inventario/SeedItem")]
[Serializable]
public class SeedItemData : InventoryItemData
{
    public GameObject DirtPrefab;

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
}
