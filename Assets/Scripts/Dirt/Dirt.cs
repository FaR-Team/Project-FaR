using System.Security.AccessControl;
using System.Collections.Generic;
using UnityEngine;
using FaRUtils.Systems.DateTime;
using FaRUtils.Systems.Weather;
using System.Collections;

[RequireComponent(typeof(DirtAreaHarvest))]
public class Dirt : MonoBehaviour
{
    bool _isEmpty;
    public bool _isWet;

    public bool testing;

    public GameObject colliders;

    public int abilityLevelPlaceholder = 1;
    public bool IsEmpty => _isEmpty;

    public GameObject violeta;
    public GameObject currentCrop;
    public SeedItemData currentCropData;

    public GameObject TextureAnimation;
    
    public static Color wetDirtColor = new(0.5f, 0.3f, 0.3f);

    void Start()
    {
        _isEmpty = true;

        DateTime.OnHourChanged.AddListener(DryDirt);
        WeatherManager.Instance.IsRaining.AddListener(DirtIsWet);
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

    public void GetDown()
    {
        //AND MOVE IT ALL AROUND
        colliders.transform.position = new Vector3(colliders.transform.position.x, -2, colliders.transform.position.z);
    }

    public void DryDirt(int hour)
    {
        if (testing) return;
        
        if(hour != 5) return;

        _isWet = false;
        this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.white;
    }

    void OnEnable()
    {
        TextureAnimation = GetComponentInChildren<Animation>().gameObject;
        TextureAnimation.GetComponent<Animation>().enabled = true;
    }

    void OnDisable()
    {
        currentCrop = null;
        currentCropData = null;
        _isEmpty = true;
        _isWet= false;
        //GetComponentInChildren<Animation>().Stop() ;
        TextureAnimation.GetComponent<Animation>().clip.SampleAnimation(TextureAnimation, 0f);
        colliders.transform.position = this.transform.position;
        DateTime.OnHourChanged.RemoveListener(DryDirt);
        WeatherManager.Instance.IsRaining.RemoveListener(DirtIsWet);
    }
}
