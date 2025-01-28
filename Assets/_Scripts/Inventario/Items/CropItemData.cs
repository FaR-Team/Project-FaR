using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Jueguito Granjil/Inventario/CropItem")]
public class CropItemData : InventoryItemData
{
    public GameObject CropBoxPrefab;
    public SellSystem _sellSystem;
    private float   _maxRayDistance = 50f;

    public override bool UseItem()
    {
        _sellSystem = FindObjectOfType<SellSystem>();


        _sellSystem.SellItem(CropBoxPrefab, this);
        return true;
    }
}
