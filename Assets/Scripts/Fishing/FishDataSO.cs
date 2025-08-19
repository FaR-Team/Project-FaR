using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Jueguito Granjil/Fishing/Fish Data", fileName = "New Fish Data")]
public class FishDataSO : ScriptableObject
{
    [Header("Fish Specific")]
    [SerializeField] private string fishName;
    [SerializeField] private string fishDescription;
    [SerializeField] private GameObject modelPrefab;
    [SerializeField] private FishRarity rarity = FishRarity.Common;
    
    public GameObject ModelPrefab => modelPrefab;
    public string FishName => fishName;
    public string FishDescription => fishDescription;
    public FishRarity Rarity => rarity;
    
}
