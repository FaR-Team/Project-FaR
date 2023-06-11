using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class DirtSpawnerPooling : MonoBehaviour
{
    
    [SerializeField] int amountOfGOs;
    [SerializeField] GameObject Prefab;

    public static GameObject _DirtPrefab => instance.Prefab;

    static DirtSpawnerPooling instance;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        ObjectPooling.PreLoad(_DirtPrefab, amountOfGOs, this.gameObject);
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
}




