using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Jueguito Granjil/Fishing/Fish Data", fileName = "New Fish Data")]
public class FishDataSO : ScriptableObject
{
    [SerializeField] private string fishName;
    [SerializeField] private GameObject modelPrefab;
    
    public GameObject ModelPrefab => modelPrefab;
    public string FishName => fishName;
    
}
