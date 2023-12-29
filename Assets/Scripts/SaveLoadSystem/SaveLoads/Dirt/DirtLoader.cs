using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DirtSetter
{

    public static AllDirtsData dirtsData;
    /*
     PODEMOS HACER QUE ESTOS OBJETOS SIEMPRE ESTEN, AUQNEU SEA VACIOS SE ENCONTRARAN CADA VEZ QUE SE CREA UNA NUEVA RUN.
    HACIENDO QUE ESTE TRY SEA INUTIL.
     */
    public static void Load(GameObject _DirtPrefab, GameObject gameObject)
    {
        TryPreloadSavedDirts(_DirtPrefab, gameObject);
    }
    private static void TryPreloadSavedDirts(GameObject _DirtPrefab, GameObject gameObject)
    {
        try
        {
            dirtsData = LoadAllData.GetData<AllDirtsData>(false);
            // Se hardcodea false, pero esto debe ser dado por un game status manager,
            // que le diga si viene por primera vez o bien, si vuelve de algun sitio.

            List<GameObject> DirtsGOs = ObjectPooling.LoadSavedObjects(_DirtPrefab, dirtsData.counter, gameObject);

            DirtsGOs.ForEach(dirt => { dirt.GetComponent<Dirt>().LoadData(dirtsData.data.Dequeue()); });
            
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to preload dirt information. reason {e}");
            PreloadDirts(_DirtPrefab, gameObject);
        }
    }

    private static void PreloadDirts(GameObject _DirtPrefab, GameObject gameObject)
    {
        ObjectPooling.PreLoad(_DirtPrefab, 10, gameObject);
    }
}
