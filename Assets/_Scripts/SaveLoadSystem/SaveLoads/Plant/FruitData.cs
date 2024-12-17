using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class FruitData : PlantData
{
    public int daysPlanted;
    public Vector3 position;
    public GrowingState growingState;
    
    public FruitData(GrowingBase growingBase)
        //int daysPlanted, int reGrowCounter, int daysWithoutHarvest, int daysWithoutFruitsCounter, 
        //List<Vector3> usedSpawnPointsPos, GrowingState growingState, Vector3 position, int daysDry) : base
    {
        GrowingTreeAndPlant growingTreeAndPlant = growingBase as GrowingTreeAndPlant;
        if (growingTreeAndPlant == null)
        {
            Debug.LogError("Could not cast GrowingBase to GrowingTreeAndPlant when creating FruitData");
            return;
        }
        this.daysPlanted = growingTreeAndPlant.DaysPlanted;
        this.growingState = growingTreeAndPlant.CurrentState;
        this.position = growingTreeAndPlant.transform.position;
    }

}
