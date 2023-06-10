using System.Security.AccessControl;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DirtAreaHarvest))]
public class Dirt : MonoBehaviour
{
    bool _isEmpty;
    public bool _isWet;


    public int abilityLevelPlaceholder = 1;
    public bool IsEmpty => _isEmpty;

    public GameObject violeta;
    public GameObject currentCrop;
    public SeedItemData currentCropData;
    
   
    public static Color wetDirtColor = new(0.5f, 0.3f, 0.3f);

    void Start()
    {
        _isEmpty = true;
    }
    public bool GetCrop(SeedItemData itemData)
    {
        _isEmpty = false;

        GameObject instantiated = GameObject.Instantiate(itemData.DirtPrefab, transform.position, GridGhost.Rotation(), transform);
        
        currentCrop = instantiated;
        currentCropData = itemData;
        
        GridGhost.UpdateRandomSeed();
        return (instantiated != null);
    }

    public void DirtIsHorny()
    {
        _isWet = true;
        this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = wetDirtColor;
    }

    public void DirtIsNotHorny()
    {
        _isWet = false;
        this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.white;
    }
}
