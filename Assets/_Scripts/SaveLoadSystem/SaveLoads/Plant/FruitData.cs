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
    {
        GrowingFruit growingTreeAndPlant = growingBase as GrowingFruit;
        if (growingTreeAndPlant == null)
        {
            Debug.LogError("Could not cast GrowingBase to GrowingFruit when creating FruitData");
            return;
        }
        this.daysPlanted = growingTreeAndPlant.DaysPlanted;
        this.growingState = growingTreeAndPlant.CurrentState;
        this.position = growingTreeAndPlant.transform.position;
    }

}
