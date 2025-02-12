using FaRUtils.Systems.Weather;
using System;
using UnityEngine;
using Utils;

[RequireComponent(typeof(DirtAreaHarvest))]
public class Dirt : MonoBehaviour
{
    public bool _isWet;

    public bool testing;

    public GameObject colliders;

    public int abilityLevelPlaceholder = 1;

    public bool IsEmpty { get; private set; }
    public GameObject violeta { get; private set; }

    public GrowingBase currentCrop;
    public SeedItemData currentSeedData { get; private set; }
    public PlantData cropSaveData { get; private set; }

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
        cropSaveData = data.plantData;
        /*
        if (data.cropData != null)
        {
            this.Log("Loaded as CropData");
            cropSaveData = data.cropData;
        }
        else if (data.treeBushData != null)
        {
            this.Log("Loaded as TreeBushData");
            cropSaveData = data.treeBushData; // TODO: No se si es la mejor forma de hacer esto
        }*/

        transform.position = data.position;

        if (currentSeedData != null)
        {
            LoadCrop();
        }
    }

    private void LoadCrop()
    {
        GetCrop(currentSeedData);
        try
        {
            currentCrop.LoadData(cropSaveData);
        }
        catch (Exception e)
        {
            this.LogWarning(e);
        }
    }
    public PlantData GetCropSaveData()
    {
        if (currentCrop == null) return null;
        
        if (currentCrop is GrowingTreeAndPlant)
        {
            this.Log("Saving treebushdata");
            return new TreeBushData(currentCrop);
        }
        else // TODO: Separar mejor segun tipos de crops y eso
        {
            this.Log("Saving cropsavedata");
            return new CropSaveData(currentCrop);
        }
    }
    public bool GetCrop(SeedItemData itemData)
    {
        IsEmpty = false;

        GameObject instantiated = Instantiate(itemData.DirtPrefab, transform.position, GridGhost.Rotation(), transform);

        currentCrop = instantiated.GetComponent<GrowingBase>();
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
        Destroy(currentCrop);
        currentCrop = null;
        currentSeedData = null;
        IsEmpty = true;
        _isWet = false;
        TextureAnimation.GetComponent<Animation>().clip.SampleAnimation(TextureAnimation, 0f);
        colliders.transform.position = this.transform.position;
        FaRUtils.Systems.DateTime.DateTime.OnHourChanged.RemoveListener(DryDirt);
        WeatherManager.Instance.IsRaining.RemoveListener(DirtIsWet);
    }

    public void DestroyDirtAndCrop()
    {
        // TODO: Chequear si con esto ya es suficiente
        DirtSpawnerPooling.DeSpawn(gameObject);
    }
}