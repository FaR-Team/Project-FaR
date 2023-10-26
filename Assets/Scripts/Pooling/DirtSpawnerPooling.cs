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
        AllDirtsData data = LoadAllDirtData.GetData();
        
        dirtsData = 
            LoadAllDirtData.GetData() == null ? 
                new AllDirtsData(5) : data;
    }

    public static int GetActiveDirtPool()
    {
        int count = ObjectPooling.GetActiveObjects(_DirtPrefab);
        return count;
    }

    private void Start()
    {
        ObjectPooling.PreLoad(_DirtPrefab, dirtsData.DirtCounter, gameObject);
        print(ObjectPooling.pool[_DirtPrefab.GetInstanceID()]);
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




