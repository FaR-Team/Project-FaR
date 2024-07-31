using System;
using System.Collections.Generic;
using UnityEngine;

public static class ChestSetter

{
    static AllChestSystems chestsDatas;

    public static void Load(GameObject chestPrefab, GameObject gameObject, GameObject parentGO)
    {
        TryPreloadSavedChests(chestPrefab, gameObject, parentGO);
    }

    private static void TryPreloadSavedChests(GameObject chestPrefab, GameObject gameObject, GameObject parentGO)
    {
        try
        {
            chestsDatas = LoadAllData.GetData<AllChestSystems>();

            List<GameObject> chestsGOs = ObjectPooling.LoadSavedObjects(chestPrefab, chestsDatas.dataCounter, gameObject);

            chestsGOs.ForEach(chest => { chest.GetComponent<Cofre>().LoadData(chestsDatas.data.Dequeue()); });

        }
        catch (Exception e)
        {
            Debug.LogWarning(e + "Chests couldnt be loaded. using preload system.");
            PreloadChests(chestPrefab, parentGO);
        }
    }

    private static void PreloadChests(GameObject chestPrefab, GameObject parentGO)
    {
        ObjectPooling.PreLoad(chestPrefab, 1, parentGO);
    }
}
