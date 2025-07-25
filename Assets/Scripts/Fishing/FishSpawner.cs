using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] private FishDataSO[] fishDatas;
    [SerializeField] private Transform[] possibleSpots;

    void SpawnRandomFish()
    {
        // TODO: Elegir un spot random, y spawnear un prefab de FishingSpot ahí, hacerle Setup al FishingSpot para que reciba el FishData de ese spot, y se lo de al player al agarrarlo
    }
}