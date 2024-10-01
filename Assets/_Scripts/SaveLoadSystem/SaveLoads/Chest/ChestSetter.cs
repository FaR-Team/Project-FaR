using System;
using System.Collections.Generic;
using UnityEngine;

public static class ChestSetter
{
    static AllChestSystems chestsDatas;
    static List<GameObject> chestsGOs;
    static GameObject _chestPrefab;
    public static void Load(GameObject chestPrefab, GameObject parentGO)
    {
        _chestPrefab = chestPrefab;
        TryPreloadSavedChests(parentGO);
    }

    private static void TryPreloadSavedChests(GameObject parentGO)
    {
        try
        {
            chestsDatas = LoadAllData.GetData<AllChestSystems>();

            chestsGOs = ObjectPooling.LoadSavedObjects(_chestPrefab, chestsDatas.dataCounter, parentGO);

            chestsGOs.ForEach(chest => { chest.GetComponent<Cofre>().LoadData(chestsDatas.data.Dequeue()); });

        }
        catch (Exception e)
        {
            Debug.LogWarning(e + "Chests couldnt be loaded. using preload system.");
            PreloadChests(parentGO);
        }
    }

    private static void PreloadChests(GameObject parentGO)
    {
        chestsGOs = ObjectPooling.PreLoad(_chestPrefab, 1, parentGO);
    }
    public static int GetActiveChestsAmount()
    {
        return chestsGOs.Count;
    }

    public static void ReloadChestData(GameObject parentGO)
    {
        chestsGOs.ForEach((chest) =>
        {
            ObjectPooling.RecicleObject(chest, _chestPrefab);
        });
        TryPreloadSavedChests(parentGO);
    }
}
