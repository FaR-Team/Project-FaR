using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    public static Task Load(GameObject _DirtPrefab, GameObject parentGO, bool temporary)
    {
        dirtPrefab = _DirtPrefab;
        return TryPreloadSavedDirts(parentGO, temporary);
        
    }
    
    
    private static async Task TryPreloadSavedDirts(GameObject parentGO, bool temporary)
    {
        try
        {
            dirtsData = LoadAllData.GetData<AllDirtsData>(temporary);
            // Se hardcodea false, pero esto debe ser dado por un game status manager,
            // que le diga si viene por primera vez o bien, si vuelve de algun sitio.
            
            if(!temporary) DirtSaver.instance.LoadScenesData(dirtsData.scenesDataList);

            var dirtDataQueue = dirtsData.GetSceneDataFromName(SceneManager.GetActiveScene().name).datas;
            dirtsGO = ObjectPooling.LoadSavedObjects(dirtPrefab, dirtDataQueue.Count, parentGO);

            foreach (var dirt in dirtsGO)
            {
                await dirt.GetComponent<Dirt>().LoadData(dirtsData.data.Dequeue());
            }
            //dirtsGO.ForEach(dirt => { await dirt.GetComponent<Dirt>().LoadData(dirtsData.data.Dequeue()); });

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
