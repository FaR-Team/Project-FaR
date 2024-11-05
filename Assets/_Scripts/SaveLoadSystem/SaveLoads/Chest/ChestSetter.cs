using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public static class ChestSetter
{
    static AllChestSystems chestsDatas;
    static List<GameObject> chestsGOs;
    static GameObject _chestPrefab;

    public static DummyLogger logger;

    static ChestSetter()
    {
        logger = GameObject.FindObjectOfType<DummyLogger>();
        if (logger == null)
        {
            GameObject loggerObject = new GameObject("DataSaverLogger");
            logger = loggerObject.AddComponent<DummyLogger>();
            UnityEngine.Object.DontDestroyOnLoad(loggerObject);
        }
    }

    public static void Load(GameObject chestPrefab, GameObject parentGO, bool temporary) // TODO: No es necesario tantas funciones entre medio que pasen el mismo parámetro
    {
        _chestPrefab = chestPrefab;
        TryPreloadSavedChests(parentGO, temporary);
    }

    public static void Load(Cofre[] chestsInstances, bool temporary)
    {
        chestsDatas = LoadAllData.GetData<AllChestSystems>(temporary);
        var sceneDatas = chestsDatas.GetSceneDataFromName(SceneManager.GetActiveScene().name).datas; // Get datas from this scene
        
        if(!temporary) ChestSaver.instance.LoadScenesData(chestsDatas.scenesDataList);

        for (int i = 0; i < chestsInstances.Length; i++)
        {
            var chestID = chestsInstances[i].ID;
            chestsInstances[i].LoadData(sceneDatas.First(data => data.id.Equals(chestID)));
        }
    }

    private static void TryPreloadSavedChests(GameObject parentGO, bool temporary)
    {
        try
        {
            chestsDatas = LoadAllData.GetData<AllChestSystems>(temporary);
            
            if(!temporary) ChestSaver.instance.LoadScenesData(chestsDatas.scenesDataList);

            // Creo que es medio al pedo conseguir la lista, se puede usar el count de la chestsDatas.data pero pa testear no quiero romper nada (lo mismo en DirtSetter
            var chestDataQueue = chestsDatas.GetSceneDataFromName(SceneManager.GetActiveScene().name).datas;
            chestsGOs = ObjectPooling.LoadSavedObjects(_chestPrefab, chestDataQueue.Count, parentGO); 

            chestsGOs.ForEach(chest => { chest.GetComponent<Cofre>().LoadData(chestsDatas.data.Dequeue()); });

        }
        catch (Exception e)
        {
            logger.LogWarning(e + "Chests couldnt be loaded. using preload system.");
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
        TryPreloadSavedChests(parentGO, false);
    }
}
