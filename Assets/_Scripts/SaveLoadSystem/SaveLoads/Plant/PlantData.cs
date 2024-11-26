using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlantData : SaveData
{
    public int daysPlanted;
    public int daysWithoutHarvest;
    public int reGrowCounter;
    public int daysWithoutFruitsCounter;
    public HashSet<Transform> usedSpawnPoints;
    public Vector3 position;
    public GrowingState growingState;

    public PlantData(int daysPlanted, int reGrowCounter, int daysWithoutHarvest, int daysWithoutFruitsCounter, HashSet<Transform> usedSpawnPoints, GrowingState growingState, Vector3 position)
    {
        this.daysPlanted = daysPlanted;
        this.reGrowCounter = reGrowCounter;
        this.daysWithoutHarvest = daysWithoutHarvest;
        this.daysWithoutFruitsCounter = daysWithoutFruitsCounter;
        this.usedSpawnPoints = usedSpawnPoints;
        this.growingState = growingState;
        this.position = position;
    }
    public PlantData()
    {
        
    }
}
