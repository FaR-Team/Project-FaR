using System;
using System.Collections.Generic;
using UnityEngine;

public class DirtSpawnerPooling : MonoBehaviour
{
    [SerializeField] GameObject Prefab;

    public static DirtSpawnerPooling instance;

    public static GameObject _DirtPrefab => instance.Prefab;

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
        DirtSetter.Load(_DirtPrefab, gameObject);
    }

    public static void SpawnObject(Vector3 position, Quaternion rotation)
    {
        GameObject dirtGO = ObjectPooling.GetObject(_DirtPrefab);
        dirtGO.transform.SetPositionAndRotation(position, rotation);
    }

    public static void DeSpawn(GameObject go)
    {
        ObjectPooling.RecicleObject(_DirtPrefab, go);
    }

    #region DEBUG FNCTN
    public List<GameObject> GetActiveDirts()
    {
        List<GameObject> dirtList = new();
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

    private void OnDestroy()
    {
        ObjectPooling.ClearReferencesFromPool(_DirtPrefab, gameObject);
    }
}
