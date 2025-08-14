using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Jueguito Granjil/Fishing/Fish Data", fileName = "New Fish Data")]
public class FishDataSO : InventoryItemData
{
    [Header("Fish Specific")]
    [SerializeField] private string fishName;
    [SerializeField] private GameObject modelPrefab;
    [SerializeField] private FishRarity rarity = FishRarity.Common;
    
    public override ItemCategory Category => ItemCategory.Fish;
    
    public GameObject ModelPrefab => modelPrefab;
    public string FishName => fishName;
    public FishRarity Rarity => rarity;
    
}
