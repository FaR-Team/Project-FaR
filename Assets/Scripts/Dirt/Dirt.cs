using System.Security.AccessControl;
using System.Collections.Generic;
using UnityEngine;
using FaRUtils.Systems.DateTime;

[RequireComponent(typeof(DirtAreaHarvest))]
public class Dirt : MonoBehaviour
{
    bool _isEmpty;
    public bool _isWet;

    public bool testing;


    public int abilityLevelPlaceholder = 1;
    public bool IsEmpty => _isEmpty;

    public GameObject violeta;
    public GameObject currentCrop;
    public SeedItemData currentCropData;
    
    public static Color wetDirtColor = new(0.5f, 0.3f, 0.3f);

    void Start()
    {
        _isEmpty = true;
        DateTime.OnHourChanged.AddListener(DryDirt);
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

    public void DirtIsWet()
    {
        _isWet = true;
        this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = wetDirtColor;
    }

    public void DryDirt(int hour)
    {
        if (testing) return;
        
        if(hour != 6) return;

        _isWet = false;
        this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.white;
    }


    private void OnDisable()
    {
        currentCrop = null;
        currentCropData = null;
        _isEmpty = true;
        _isWet= false;
    }
}
