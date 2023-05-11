using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dirt : MonoBehaviour
{
    bool _isEmpty;

    public bool IsEmpty => _isEmpty;

    public GameObject currentCrop;
    public SeedItemData currentCropData;

    void Start()
    {
        _isEmpty = true;
    }

    void Update()
    {
        
    }

    public bool GetCrop(SeedItemData itemData)
    {
        _isEmpty = false;
        GameObject instantiated = GameObject.Instantiate(itemData.DirtPrefab, transform.position, Quaternion.identity, transform);
        currentCrop = instantiated;
        currentCropData = itemData;

        return (instantiated != null);
    }
}
