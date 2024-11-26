using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public static class PlantSetter
{

    public static AllPlantsData plantsData;

    static List<GameObject> plantsGO;
    static GameObject plantPrefab;

    public static DummyLogger logger;

    static PlantSetter()
    {
        logger = GameObject.FindObjectOfType<DummyLogger>();
        if (logger == null)
        {
            GameObject loggerObject = new GameObject("DataSaverLogger");
            logger = loggerObject.AddComponent<DummyLogger>();
            UnityEngine.Object.DontDestroyOnLoad(loggerObject);
        }
    }

    public static void Load(GameObject _DirtPrefab, GameObject parentGO, bool temporary)
    {
        plantPrefab = _DirtPrefab;
        TryPreloadSavedDirts(parentGO, temporary);
    }
    private static void TryPreloadSavedDirts(GameObject parentGO, bool temporary)
    {
        try
        {
            plantsData = LoadAllData.GetData<AllPlantsData>(temporary);
            // Se hardcodea false, pero esto debe ser dado por un game status manager,
            // que le diga si viene por primera vez o bien, si vuelve de algun sitio.
            
            if(!temporary) PlantSaver.instance.LoadScenesData(plantsData.scenesDataList);

            var plantDataQueue = plantsData.GetSceneDataFromName(SceneManager.GetActiveScene().name).datas;
            plantsGO = ObjectPooling.LoadSavedObjects(plantPrefab, plantDataQueue.Count, parentGO);

            plantsGO.ForEach(dirt => { dirt.GetComponent<TreeGrowing>().LoadData(plantsData.data.Dequeue()); });

        }
        catch (Exception e)
        {
            logger.LogWarning($"Failed to preload dirt information. reason {e}");
            PreloadDirts(plantPrefab, parentGO);
        }
    }

    private static void PreloadDirts(GameObject _DirtPrefab, GameObject gameObject)
    {
        ObjectPooling.PreLoad(_DirtPrefab, 10, gameObject);
    }

    public static void ReloadDirtData(GameObject parentGO)
    {
        plantsGO.ForEach((chest) =>
        {
            ObjectPooling.RecicleObject(chest, plantPrefab);
        });
        TryPreloadSavedDirts(parentGO, false);
    }
}
