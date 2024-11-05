using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChestsManager : MonoBehaviour
{
    [SerializeField] GameObject Prefab, parentGO;

    [SerializeField] private Cofre[] chestsInScene;
    
    public static ChestsManager instance;

    public static GameObject chestPrefab => instance.Prefab;
    
    private static bool firstLoad = false;

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
        
        chestsInScene = FindObjectsOfType<Cofre>(); // TODO: O tener un Manager por escena que ya tenga asignadas las referencias, o que los limpie y busque por escena
    }
    
    private void Start()
    {
        if (firstLoad)
        {
            Debug.Log("Load temporary chest");
            ChestSetter.Load(chestsInScene, true); // If already loaded when opening game, load temporal save
        }
        else
        {
            Debug.Log("Load non-temporary chest save file");
            ChestSetter.Load(chestsInScene, false); // On Start, load non-temporary dirt data
            firstLoad = true;
        }

        //ChestSetter.Load(chestPrefab, parentGO, false); // On Start, load non-temporary save
    }

    public void Reload()
    {
        ChestSetter.ReloadChestData(parentGO);
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