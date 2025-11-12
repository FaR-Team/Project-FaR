using UnityEngine;

[CreateAssetMenu(menuName = "Jueguito Granjil/Inventario/Items Index", fileName = "ItemIndex", order = 51)]
public class ItemsIndex : ScriptableObject
{
    public InventoryItemData[] items = new InventoryItemData[0];
}
