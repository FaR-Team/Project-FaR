using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class TreeBushData : PlantData
{
    public int daysPlanted;
    public int daysWithoutHarvest;
    public int reGrowCounter;
    public int daysWithoutFruitsCounter;
    public List<FruitData> spawnedFruits;
    public Vector3 position;
    public GrowingState growingState;
    public int daysDry;
    
    public TreeBushData(GrowingBase growingBase)
        //int daysPlanted, int reGrowCounter, int daysWithoutHarvest, int daysWithoutFruitsCounter, 
        //List<Vector3> usedSpawnPointsPos, GrowingState growingState, Vector3 position, int daysDry) : base
    {
        GrowingTreeAndPlant growingTreeAndPlant = growingBase as GrowingTreeAndPlant;
        if (growingTreeAndPlant == null)
        {
            Debug.LogError("");
            return;
        }
        this.daysPlanted = growingTreeAndPlant.DaysPlanted;
        this.reGrowCounter = growingTreeAndPlant.ReGrowCounter;
        this.daysWithoutHarvest = growingTreeAndPlant.DaysWithoutHarvest;
        this.daysWithoutFruitsCounter = growingTreeAndPlant.DaysWithoutFruits;
        this.spawnedFruits = growingTreeAndPlant.fruits.Select(f => f.GetData()).ToList();
        this.growingState = growingTreeAndPlant.CurrentState;
        this.position = growingTreeAndPlant.transform.position;
        this.daysDry = growingTreeAndPlant.DaysDry;
    }

}
