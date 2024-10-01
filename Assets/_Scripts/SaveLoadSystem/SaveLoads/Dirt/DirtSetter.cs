using System;
using System.Collections.Generic;
using UnityEngine;

public static class DirtSetter
{

    public static AllDirtsData dirtsData;

    static List<GameObject> dirtsGO;
    static GameObject dirtPrefab;

    public static void Load(GameObject _DirtPrefab, GameObject parentGO)
    {
        dirtPrefab = _DirtPrefab;
        TryPreloadSavedDirts(parentGO);
    }
    private static void TryPreloadSavedDirts(GameObject parentGO)
    {
        try
        {
            dirtsData = LoadAllData.GetData<AllDirtsData>();
            // Se hardcodea false, pero esto debe ser dado por un game status manager,
            // que le diga si viene por primera vez o bien, si vuelve de algun sitio.

            dirtsGO = ObjectPooling.LoadSavedObjects(dirtPrefab, dirtsData.counter, parentGO);

            dirtsGO.ForEach(dirt => { dirt.GetComponent<Dirt>().LoadData(dirtsData.data.Dequeue()); });

        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to preload dirt information. reason {e}");
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
        TryPreloadSavedDirts(parentGO);
    }
}
