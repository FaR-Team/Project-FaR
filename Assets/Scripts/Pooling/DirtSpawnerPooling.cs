using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class DirtSpawnerPooling : MonoBehaviour
{
    [SerializeField] GameObject Prefab;

    public static GameObject _DirtPrefab => instance.Prefab;

    static DirtSpawnerPooling instance;

    public static AllDirtsData dirtsData;

    private void Awake()
    {
        instance = this;
       // if( (DataDeTodasLasDirts)Loader.Load("filePath") != null)
       // {
       //     dirtsData = (DataDeTodasLasDirts)Loader.Load("filePath");
      //  }
      //  else
      //  {
            dirtsData = new AllDirtsData();
            dirtsData.DirtCounter = 5;
      //  }
    }
    private void Start()
    {
        ObjectPooling.PreLoad(Prefab, 5, gameObject);
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
        for(int i = 0; i < this.gameObject.transform.childCount; i++)
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




