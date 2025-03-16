using FaRUtils.Systems.Weather;
using System;
using System.Threading.Tasks;
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

        if (WeatherManager.Instance.CurrentWeather == Weather.Rain)
        {
            DirtIsWet();
        }
    }

    public async Task LoadData(DirtData data)
    {
        _isWet = data._isWet;
        IsEmpty = data.IsEmpty;
        currentSeedData = data.currentCropData;
        cropSaveData = data.plantData;

        transform.position = data.position;

        if (currentSeedData != null)
        {
            await LoadCrop();
        }
    }

    private Task LoadCrop()
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

        if (WeatherManager.Instance.CurrentWeather == Weather.Rain)
        {
            DirtIsWet();
        }
        
        return Task.CompletedTask;
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
        Reset();
    }

    public void DestroyDirtAndCrop()
    {
        // TODO: Chequear si con esto ya es suficiente
        DirtSpawnerPooling.DeSpawn(gameObject);
    }

    public void Reset()
    {
        if(currentCrop) Destroy(currentCrop.gameObject);
        currentCrop = null;
        currentSeedData = null;
        cropSaveData = null;
        IsEmpty = true;
        _isWet = false;
        TextureAnimation.GetComponent<Animation>().clip.SampleAnimation(TextureAnimation, 0f);
        colliders.transform.position = this.transform.position;
        FaRUtils.Systems.DateTime.DateTime.OnHourChanged.RemoveListener(DryDirt);
        WeatherManager.Instance.IsRaining.RemoveListener(DirtIsWet);
    }
}