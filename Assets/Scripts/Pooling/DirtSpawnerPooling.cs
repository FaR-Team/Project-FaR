using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class DirtSpawnerPooling : MonoBehaviour
{
    [SerializeField] GameObject Prefab;

    public static DirtSpawnerPooling instance;

    public static GameObject _DirtPrefab => instance.Prefab;
    public static AllDirtsData dirtsData;

    private void Awake()
    {
        instance = this;
    }

    public static int GetActiveDirtPool()
    {
        int count = ObjectPooling.GetActiveObjects(_DirtPrefab);
        return count;
    }

    private void Start()
    {
        TryPreloadSavedDirts();
    }

    /*
     PODEMOS HACER QUE ESTOS OBJETOS SIEMPRE ESTEN, AUQNEU SEA VACIOS SE ENCONTRARAN CADA VEZ QUE SE CREA UNA NUEVA RUN.
    HACIENDO QUE ESTE TRY SEA INUTIL.
     */
    private void TryPreloadSavedDirts()
    {
        try
        {
            dirtsData = LoadAllDirtData.GetData(false);

            List<GameObject> gos = ObjectPooling.PreLoadSavedObjects(_DirtPrefab, dirtsData.DirtCounter, gameObject);
            
            foreach (var obj in gos)
            {
                obj.GetComponent<Dirt>().LoadData(dirtsData.data.Dequeue());
            }
        }
        catch (Exception e) 
        {
            Debug.LogWarning(e);
            PreloadDirts();
        }
    }

    private void PreloadDirts()
    {
        List<GameObject> gos = ObjectPooling.PreLoad(_DirtPrefab, 10, gameObject);
    }

    public static void SpawnObject(Vector3 position, Quaternion rotation)
    {
        GameObject dirtGO = ObjectPooling.GetObject(_DirtPrefab);
        dirtGO.transform.SetPositionAndRotation(position, rotation);
    }

    public static void DeSpawn(GameObject primitive, GameObject go)
    {
        ObjectPooling.RecicleObject(primitive, go);
    }

    #region DEBUG FNCTN
    public List<GameObject> GetActiveDirts()
    {
        List<GameObject> dirtList = new List<GameObject>();
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            GameObject currentGO = gameObject.transform.GetChild(i).gameObject;
            if (currentGO.activeInHierarchy)
            {
                dirtList.Add(currentGO);
            }
        }

        return dirtList;
    }
    #endregion
}




