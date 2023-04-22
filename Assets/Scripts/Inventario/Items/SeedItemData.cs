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

    public override bool UseItem()
    {
        gridGhost = FindObjectOfType<GridGhost>();
        grid = FindObjectOfType<Grid>();

        if(Seed == true)
        {
            if (gridGhost.PlantNear(DirtPrefab) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        if (TreeSeed == true)
        {
            if (gridGhost.PlantTreeNear(DirtPrefab) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
