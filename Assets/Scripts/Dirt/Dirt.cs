using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dirt : MonoBehaviour
{
    bool _isEmpty;

    public bool IsEmpty => _isEmpty;

    public GameObject currentCrop;
    void Start()
    {
        _isEmpty = true;
    }

    void Update()
    {
        
    }

    public bool GetCrop(GameObject prefab)
    {
        _isEmpty = false;
        GameObject instantiated = GameObject.Instantiate(prefab, transform.position, Quaternion.identity, transform);
        currentCrop = instantiated;

        return (instantiated != null);

    }
}
