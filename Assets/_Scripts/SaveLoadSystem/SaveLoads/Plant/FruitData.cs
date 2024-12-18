using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class FruitData : PlantData
{
    public int daysPlanted;
    public Vector3 spawnPosition;
    public GrowingState growingState;
    
    public FruitData(GrowingBase growingBase)
    {
        GrowingFruit growingFruit = growingBase as GrowingFruit;
        if (growingFruit == null)
        {
            Debug.LogError("Could not cast GrowingBase to GrowingFruit when creating FruitData");
            return;
        }
        this.daysPlanted = growingFruit.DaysPlanted;
        this.growingState = growingFruit.CurrentState;
        this.spawnPosition = growingFruit.transform.parent.localPosition;
    }

}
