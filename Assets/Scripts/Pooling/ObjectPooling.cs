using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public static Dictionary<int, Queue<GameObject>> pool = new Dictionary<int, Queue<GameObject>>();
    static Dictionary<int, Queue<GameObject>> activePool = new Dictionary<int, Queue<GameObject>>();

    static Dictionary<int, GameObject> parents = new Dictionary<int, GameObject>();

    public static void PreLoad(GameObject objectToPool, int amount, GameObject parent)
    {
        int id = objectToPool.GetInstanceID();

        parents.Add(id, parent);
        pool.Add(id, new Queue<GameObject>());
        activePool.Add(id, new Queue<GameObject>());
        
        for (int i = 0; i < amount; i++)
        {
            CreateObject(objectToPool);
        }
    }


    static void CreateObject(GameObject objectToPool)
    {
        int id = objectToPool.GetInstanceID();

        GameObject go = Instantiate(objectToPool) as GameObject;
        go.transform.SetParent(GetParent(id).transform);
        go.SetActive(false);

        pool[id].Enqueue(go);

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


