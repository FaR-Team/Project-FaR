using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] private FishingSpot spotPrefab;
    [SerializeField] private FishItemData[] fishItemDatas;
    [SerializeField] private Transform[] possibleSpots;
    
    Dictionary<Transform, FishingSpot> currentFishingSpots = new Dictionary<Transform, FishingSpot>();
    
    #if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            SpawnRandomFish();
        }
    }
    #endif
    void SpawnRandomFish()
    {
        var spawnpoint = GetRandomSpawnpoint();
        
        var newSpot = Instantiate(spotPrefab, spawnpoint.position, spawnpoint.rotation);
        newSpot.Setup(GetRandomFish(), this);
        
        currentFishingSpots.Add(spawnpoint, newSpot);
    }

    FishItemData GetRandomFish()
    {
        // TODO: Rarity and weights
        return fishItemDatas[Random.Range(0, fishItemDatas.Length)];
    }

    Transform GetRandomSpawnpoint()
    {
        if (currentFishingSpots.Count == possibleSpots.Length)
        {
            Debug.LogError("No Fishing Spawnpoints available");
            return null;
        }
        Transform[] available = possibleSpots.Where(s => !currentFishingSpots.ContainsKey(s)).ToArray();
        
        return available[Random.Range(0, available.Length)];
    }

    public void FreeSpot(FishingSpot spot)
    {
        if (!currentFishingSpots.ContainsValue(spot))
        {
            Debug.LogError("Tried to free a fishing spot that is not being used");
        }
        
        currentFishingSpots.Remove(spot.transform);
    }
}