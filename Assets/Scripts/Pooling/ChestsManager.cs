using System;
using UnityEngine;

public class ChestsManager : MonoBehaviour
{
    [SerializeField] GameObject Prefab, parentGO;

    public static ChestsManager instance;

    public static GameObject chestPrefab => instance.Prefab;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        ChestSetter.Load(chestPrefab, gameObject ,parentGO);
    }

    public static int GetActiveChestsAmount()
    {
        return ObjectPooling.GetActiveObjects(chestPrefab);
    }

    public static void CreateChest(Vector3 position, Quaternion rotation)
    {
        Instantiate(chestPrefab, position, rotation);
    }

    private void OnDestroy()
    {
        ObjectPooling.ClearReferencesFromPool(chestPrefab, parentGO);
    }
}