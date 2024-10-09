using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public static class DirtSetter
{

    public static AllDirtsData dirtsData;

    static List<GameObject> dirtsGO;
    static GameObject dirtPrefab;

    public static DummyLogger logger;

    static DirtSetter()
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
        dirtPrefab = _DirtPrefab;
        TryPreloadSavedDirts(parentGO, temporary);
    }
    private static void TryPreloadSavedDirts(GameObject parentGO, bool temporary)
    {
        try
        {
            dirtsData = LoadAllData.GetData<AllDirtsData>(temporary);
            // Se hardcodea false, pero esto debe ser dado por un game status manager,
            // que le diga si viene por primera vez o bien, si vuelve de algun sitio.
            
            if(!temporary) DirtSaver.instance.LoadDictionary(dirtsData.sceneDataDictionary);

            var dirtDataQueue = dirtsData.sceneDataDictionary[SceneManager.GetActiveScene().name];
            dirtsGO = ObjectPooling.LoadSavedObjects(dirtPrefab, dirtDataQueue.Count, parentGO);

            dirtsGO.ForEach(dirt => { dirt.GetComponent<Dirt>().LoadData(dirtsData.data.Dequeue()); });

        }
        catch (Exception e)
        {
            logger.LogWarning($"Failed to preload dirt information. reason {e}");
            PreloadDirts(dirtPrefab, parentGO);
        }
    }

    private static void PreloadDirts(GameObject _DirtPrefab, GameObject gameObject)
    {
        ObjectPooling.PreLoad(_DirtPrefab, 10, gameObject);
    }

    public static void ReloadDirtData(GameObject parentGO)
    {
        dirtsGO.ForEach((chest) =>
        {
            ObjectPooling.RecicleObject(chest, dirtPrefab);
        });
        TryPreloadSavedDirts(parentGO, false);
    }
}
