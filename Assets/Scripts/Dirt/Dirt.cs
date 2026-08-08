using FaRUtils.Systems.Weather;
using FaRUtils.Systems.GridSystem;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

[RequireComponent(typeof(DirtAreaHarvest))]
public class Dirt : MonoBehaviour, IGridEntity
{
    public bool _isWet;

    public bool testing;

    public GameObject colliders;

    public int abilityLevelPlaceholder = 1;

    public bool IsEmpty { get; private set; }
    public bool IsBeingDestroyed { get; private set; }
    public GameObject violeta { get; private set; }

    public GrowingBase currentCrop;
    public SeedItemData currentSeedData { get; private set; }
    public PlantData cropSaveData { get; private set; }

    public static Color wetDirtColor = new(0.5f, 0.3f, 0.3f);
    [SerializeField] private Animator animator;
    private Vector3Int _registeredCoord;
    private bool _isRegisteredOnGrid;

    public Vector3Int Coordinate => WorldGrid.WorldToCell(transform.position);
    public Vector3Int FootprintSize => Vector3Int.one;
    public Vector3Int FootprintOffset => Vector3Int.zero;
    public bool CanOverlap => false;
    public string EntityName => gameObject.name;

    public void OnGridRegistered(Vector3Int coord) 
    { 
        _registeredCoord = coord;
        _isRegisteredOnGrid = true;
    }
    public void OnGridUnregistered()
    {
        _isRegisteredOnGrid = false;
    }

    void Start()
    {
        FaRUtils.Systems.DateTime.DateTime.OnHourChanged.AddListener(DryDirt);
        if (WeatherManager.Instance != null && WeatherManager.Instance.IsRaining != null)
        {
            WeatherManager.Instance.IsRaining.AddListener(DirtIsWet);

            if (WeatherManager.Instance.CurrentWeather == Weather.Rain)
            {
                DirtIsWet();
            }
        }
    }

    public async Task LoadData(DirtData data)
    {
        _isWet = data._isWet;
        IsEmpty = data.IsEmpty;
        currentSeedData = data.currentCropData;
        cropSaveData = data.plantData;

        transform.position = WorldGrid.CellToWorld(data.coordinate);

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
        currentCrop.footprintSize = itemData.footprintSize;
        currentCrop.footprintOffset = itemData.footprintOffset;
        currentSeedData = itemData;

        GridDataManager.Instance.Register(currentCrop);

        GridGhost.UpdateRandomSeed();
        return (instantiated != null);
    }

    public void DirtIsWet()
    {
        _isWet = true;
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = wetDirtColor;
    }

    public void GetDown() // And move it all around
    {
        colliders.transform.position = new Vector3(colliders.transform.position.x, -2, colliders.transform.position.z);
    }

    public void RaiseColliders()
    {
        if (colliders != null)
        {
            colliders.transform.position = new Vector3(
                colliders.transform.position.x, 
                0, 
                colliders.transform.position.z
            );
        }
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
        animator = GetComponentInChildren<Animator>();
    }

    void OnDisable()
    {
        if (_isRegisteredOnGrid && GridDataManager.Instance != null)
        {
            GridDataManager.Instance.Unregister(this, _registeredCoord);
        }
        Reset();
    }

    public void DestroyDirtAndCrop()
    {
        if (IsBeingDestroyed) return;
        IsBeingDestroyed = true;

        if (currentCrop != null)
        {
            Destroy(currentCrop.gameObject);
        }
        animator.SetBool("Plow", true);
        StartCoroutine(DestroyDirtAndCropCoroutine());
    }

    private IEnumerator DestroyDirtAndCropCoroutine()
    {
        yield return new WaitForSeconds(4.25f);
        DirtSpawnerPooling.DeSpawn(gameObject);
    }

    void OnDestroy()
    {
        FaRUtils.Systems.DateTime.DateTime.OnHourChanged.RemoveListener(DryDirt);
        if (WeatherManager.Instance != null && WeatherManager.Instance.IsRaining != null)
        {
            WeatherManager.Instance.IsRaining.RemoveListener(DirtIsWet);
        }
    }

    public void Reset()
    {
        if(currentCrop) Destroy(currentCrop.gameObject);
        currentCrop = null;
        currentSeedData = null;
        cropSaveData = null;
        IsEmpty = true;
        _isWet = false;
        IsBeingDestroyed = false;
        colliders.transform.position = this.transform.position;
        FaRUtils.Systems.DateTime.DateTime.OnHourChanged.RemoveListener(DryDirt);
        if (WeatherManager.Instance != null && WeatherManager.Instance.IsRaining != null)
        {
            WeatherManager.Instance.IsRaining.RemoveListener(DirtIsWet);
        }
    }
}