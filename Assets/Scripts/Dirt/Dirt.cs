using UnityEngine;
using FaRUtils.Systems.Weather;
using System;

[RequireComponent(typeof(DirtAreaHarvest))]
public class Dirt : MonoBehaviour
{
    public bool _isWet;

    public bool testing;

    public GameObject colliders;

    public int abilityLevelPlaceholder = 1;

    public bool IsEmpty { get; private set; }
    public GameObject violeta { get; private set; }

    public GameObject currentCrop;
    public SeedItemData currentSeedData { get; private set; }
    public CropSaveData cropSaveData { get; private set; }

    public GameObject TextureAnimation;
    public static Color wetDirtColor = new(0.5f, 0.3f, 0.3f);

    void Start()
    {
        FaRUtils.Systems.DateTime.DateTime.OnHourChanged.AddListener(DryDirt);
        WeatherManager.Instance.IsRaining.AddListener(DirtIsWet);
    }

    public void LoadData(DirtData data)
    {
        _isWet = data._isWet;
        IsEmpty = data.IsEmpty;
        currentSeedData = data.currentCropData;
        cropSaveData = data.cropSaveData;
        transform.position = data.position;

        if(currentSeedData != null) 
        {
            LoadCrop();
        }
    }

    private void LoadCrop()
    {
        GetCrop(currentSeedData);
        try
        {
            currentCrop.GetComponent<GrowingBase>().LoadData(cropSaveData);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }
    }
    public CropSaveData GetCropSaveData()
    {
        if( currentCrop == null ) return null;

        var growing = currentCrop.GetComponent<GrowingBase>();
        CropSaveData cropdata = new CropSaveData(growing.DiasPlantado, growing.currentState);
        return cropdata;
    }
    public bool GetCrop(SeedItemData itemData)
    {
        IsEmpty = false;

        GameObject instantiated = Instantiate(itemData.DirtPrefab, transform.position, GridGhost.Rotation(), transform);

        currentCrop = instantiated;
        currentSeedData = itemData;

        GridGhost.UpdateRandomSeed();
        return (instantiated != null);
    }

    public void DirtIsWet()
    {
        _isWet = true;
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = wetDirtColor;
    }

    public void GetDown()
    {
        colliders.transform.position = new Vector3(colliders.transform.position.x, -2, colliders.transform.position.z);
    }

    public void DryDirt(int hour)
    {
        if (testing) return;

        if (hour != 5) return;

        _isWet = false;
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.white;
    }

    void OnEnable()
    {
        TextureAnimation = GetComponentInChildren<Animation>().gameObject;
        TextureAnimation.GetComponent<Animation>().enabled = true;
    }

    void OnDisable()
    {
        currentCrop = null;
        currentSeedData = null;
        IsEmpty = true;
        _isWet = false;
        TextureAnimation.GetComponent<Animation>().clip.SampleAnimation(TextureAnimation, 0f);
        colliders.transform.position = this.transform.position;
        FaRUtils.Systems.DateTime.DateTime.OnHourChanged.RemoveListener(DryDirt);
        WeatherManager.Instance.IsRaining.RemoveListener(DirtIsWet);
    }
}
