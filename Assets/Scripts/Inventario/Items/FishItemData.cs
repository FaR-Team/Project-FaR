using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Jueguito Granjil/Inventario/FishItem")]
[Serializable]
public class FishItemData : InventoryItemData
{
    [SerializeField] private FishDataSO fishData;
    public FishDataSO FishData => fishData;
    public override ItemCategory Category => ItemCategory.Fish;
}