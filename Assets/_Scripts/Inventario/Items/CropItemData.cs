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

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

#if UNITY_EDITOR
        Debug.DrawRay(ray.origin, ray.direction * _maxRayDistance, Color.green, 0.01f);
#endif

        if (Physics.Raycast(ray, out hit, _maxRayDistance))
        {
            if (hit.transform.tag == "Sell")
            {
                IsLookingAtStore = true;
                _sellSystem.SellItem(CropBoxPrefab, this);
                return true;
            }
            else
            {
                IsLookingAtStore = false;
                return false;
            }
        }
        else {
            return false;
        }
    }
}
