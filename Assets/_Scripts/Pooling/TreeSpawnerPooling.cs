using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TreeSpawnerPooling : MonoBehaviour
{
    [SerializeField] GameObject Prefab;

    public static TreeSpawnerPooling instance;
    private static bool firstLoad = false;

    public static GameObject _TreePrefab => instance.Prefab;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    public static int GetActiveDirtPool()
    {
        int count = ObjectPooling.GetActiveObjects(_TreePrefab);
        return count;
    }

    private void Start()
    {
        if(firstLoad)
        {
            //Debug.Log("Load temporary dirt");
            PlantSetter.Load(_TreePrefab, gameObject, true); // If already loaded when opening game, load temporal save
        }
        else
        {
            //Debug.Log("Load non-temporary dirt save file");
            PlantSetter.Load(_TreePrefab, gameObject, false); // On Start, load non-temporary dirt data
            firstLoad = true;
        }
    }

    public static void SpawnObject(Vector3 position, Quaternion rotation)
    {
        Debug.Log("Called SpawnObject in SpawnerPooling");
        GameObject dirtGO = ObjectPooling.GetObject(_TreePrefab);
        dirtGO.transform.SetPositionAndRotation(position, rotation);
    }

    public static void DeSpawn(GameObject go)
    {
        ObjectPooling.RecicleObject(_TreePrefab, go);
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
        ObjectPooling.ClearReferencesFromPool(_TreePrefab, gameObject);
    }

    public void Reload()
    {
        PlantSetter.ReloadDirtData(gameObject);
    }
}
