using System.Collections.Generic;
using UnityEngine;

public class DirtSpawnerPooling : MonoBehaviour
{
    [SerializeField] GameObject Prefab;

    public static DirtSpawnerPooling instance;
    private static bool firstLoad = false;

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
        if(firstLoad)
        {
            Debug.Log("Load temporary dirt");
            DirtSetter.Load(_DirtPrefab, gameObject, true); // If already loaded when opening game, load temporal save
        }
        else
        {
            Debug.Log("Load non-temporary dirt save file");
            DirtSetter.Load(_DirtPrefab, gameObject, false); // On Start, load non-temporary dirt data
            firstLoad = true;
        }
        
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

    public void Reload()
    {
        DirtSetter.ReloadDirtData(gameObject);
    }
}
