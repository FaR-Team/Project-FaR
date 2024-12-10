using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class PlantData : SaveData
{
    public int daysPlanted;
    public int daysWithoutHarvest;
    public int reGrowCounter;
    public int daysWithoutFruitsCounter;
    public List<Vector3> usedSpawnPointsPos;
    public Vector3 position;
    public GrowingState growingState;
    
    public PlantData(int daysPlanted, int reGrowCounter, int daysWithoutHarvest, int daysWithoutFruitsCounter, List<Vector3> usedSpawnPointsPos, GrowingState growingState, Vector3 position)
    {
        this.daysPlanted = daysPlanted;
        this.reGrowCounter = reGrowCounter;
        this.daysWithoutHarvest = daysWithoutHarvest;
        this.daysWithoutFruitsCounter = daysWithoutFruitsCounter;
        this.usedSpawnPointsPos = usedSpawnPointsPos;
        foreach (Vector3 usedSpawnPoint in usedSpawnPointsPos)
        {
            Debug.Log("Saving spawnpoint: " + usedSpawnPoint);
        }
        this.growingState = growingState;
        this.position = position;
    }
    public PlantData()
    {
        
    }
}
