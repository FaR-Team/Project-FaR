using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dirt : MonoBehaviour
{
    bool _isEmpty;

    public bool IsEmpty => _isEmpty;

    public GridGhost gridGhost;
    public GameObject currentCrop;
    private SeedItemData currentCropData;

    void Start()
    {
        _isEmpty = true;
    }

    public bool GetCrop(SeedItemData itemData)
    {
        _isEmpty = false;
        GameObject instantiated = GameObject.Instantiate(itemData.DirtPrefab, transform.position, Quaternion.identity, transform);
        currentCrop = instantiated;
        currentCropData = itemData;

        return (instantiated != null);
    }

    public SeedItemData GetCurrentCrop()
    {
        if(currentCropData == null)
        {
            return null;
        }
        else
        {
            return currentCropData;
        }
    }

    public void CheckAreaHarvest()
    {
        Dirt[] dirts = gridGhost.CheckDirtArray(this.transform.position, 10f);

        for(int i = 0; i < dirts.Length; i++)
        {
            if (dirts[i] == null || dirts[i].IsEmpty) return;
            else
            {
                Debug.Log(dirts[i]);
            }
        }
    }
}