using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public static Dictionary<int, Queue<GameObject>> pool = new Dictionary<int, Queue<GameObject>>();
    static Dictionary<int, Queue<GameObject>> activePool = new Dictionary<int, Queue<GameObject>>();

    static Dictionary<int, GameObject> parents = new Dictionary<int, GameObject>();

    public static List<GameObject> PreLoad(GameObject objectToPool, int amount, GameObject parent)
    {
        int id = objectToPool.GetInstanceID();

        parents.Add(id, parent);
        pool.Add(id, new Queue<GameObject>());
        activePool.Add(id, new Queue<GameObject>());
        
        List<GameObject> gosPreloaded = new List<GameObject>();
        
        for (int i = 0; i < amount; i++)
        {
            gosPreloaded.Add(CreateObject(objectToPool));
        }

        return gosPreloaded;
    }
    public static List<GameObject> PreLoadSavedObjects(GameObject objectToPool, int amount, GameObject parent)
    {
        int id = objectToPool.GetInstanceID();

        parents.Add(id, parent);
        pool.Add(id, new Queue<GameObject>());
        activePool.Add(id, new Queue<GameObject>());

        List<GameObject> gosPreloaded = new List<GameObject>();

        for (int i = 0; i < amount; i++)
        {
            gosPreloaded.Add(CreateSavedObject(objectToPool));
        }

        return gosPreloaded;
    }
    static GameObject CreateSavedObject(GameObject objectToPool)
    {
        int id = objectToPool.GetInstanceID();

        GameObject go = Instantiate(objectToPool);
        go.transform.SetParent(GetParent(id).transform);
        
        activePool[id].Enqueue(go);

        return go;
    }

    static GameObject CreateObject(GameObject objectToPool)
    {
        int id = objectToPool.GetInstanceID();

        GameObject go = Instantiate(objectToPool);
        go.transform.SetParent(GetParent(id).transform);
        go.SetActive(false);

        pool[id].Enqueue(go);
        return go;
    }

    static GameObject GetParent(int parentID)
    {
        GameObject parent;
        parents.TryGetValue(parentID, out parent);

        return parent;
    }

    public static int GetActiveObjects(GameObject objectToPool)
    {
        int count = activePool[objectToPool.GetInstanceID()].Count;
        return count;
    }

    public static GameObject GetObject(GameObject objectToPool)
    {

        int id = objectToPool.GetInstanceID();

        if (pool[id].Count == 0) CreateObject(objectToPool);

        GameObject go = pool[id].Dequeue();
        activePool[id].Enqueue(objectToPool);
        go.SetActive(true);

        return go;
    }

    public static void RecicleObject(GameObject objectToPool, GameObject objectToRecicle)
    {
        int id = objectToPool.GetInstanceID();

        pool[id].Enqueue(objectToRecicle);
        objectToRecicle.SetActive(false);
    }
}


