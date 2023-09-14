using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Jueguito Granjil/Inventario/SeedItem")]
public class SeedItemData : InventoryItemData
{
    private Grid grid;
    public GridGhost gridGhost;
    public GameObject DirtPrefab;

    void Awake()
    {
        gridGhost = FindObjectOfType<GridGhost>();
        grid = FindObjectOfType<Grid>();
    }

    public override bool UseItem(Dirt dirt)
    {
        return typeOfItem == TypeOfItem.CropSeed && dirt.GetCrop(this);
    }

    public override bool UseItem()
    {
        if (typeOfItem != TypeOfItem.TreeSeed) return false;

        gridGhost = FindObjectOfType<GridGhost>();
        return gridGhost.PlantTreeNear(DirtPrefab);
    }
}
