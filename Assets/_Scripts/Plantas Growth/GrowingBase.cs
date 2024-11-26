using System;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;
using DateTime = FaRUtils.Systems.DateTime.DateTime;
using Utils;
using FaRUtils.Systems.DateTime;

public abstract class GrowingBase : MonoBehaviour
{
    protected int interactableLayerInt = 7;

    protected int daysPlanted; //Dias que pasaron desde que se plantó.
    [SerializeField] protected int maxDaysWithoutHarvest = 3;
    protected int daysWithoutHarvest;
    [SerializeField] protected int maxDaysDry = 2;
    [SerializeField] protected int daysDry;

    public bool isFruit;
    [SerializeField] protected GrowingState[] states;
    public GrowingState currentState;

    [HideInInspector] public MeshFilter meshFilter;
    [HideInInspector] public MeshCollider meshCollider;
    [HideInInspector] public MeshRenderer meshRenderer;

    public int DaysPlanted => daysPlanted;
    public int DaysDry => daysDry;
    public int MaxDaysDry => maxDaysDry;
    public int DaysWithoutHarvest => daysWithoutHarvest;
    public int MaxDaysWithoutHarvest => maxDaysWithoutHarvest;
    
    public bool hasCaughtUp = false;

    protected virtual void Awake()
    {
        TryGetComponent(out meshFilter);
        TryGetComponent(out meshCollider);
        TryGetComponent(out meshRenderer);
    }

    protected virtual void Start()
    {
        GrowthEventManager.Instance.OnHourChanged += OnHourChanged;
        CatchUpMissedGrowth();
    }

    protected virtual void CatchUpMissedGrowth()
    {
        this.Log("Catching up missed growth...");
        if (hasCaughtUp) return;
        
        // Calculate days between current time and last save
        var currentTime = TimeManager.DateTime;
        var lastSaveTime = TimeManager.Instance.GetLastTimeInScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

        var currentDayCount = currentTime.Hour < 3 ? currentTime.TotalNumDays - 1 : currentTime.TotalNumDays;
        var lastDayCount = lastSaveTime.Hour < 3 ? lastSaveTime.TotalNumDays - 1 : lastSaveTime.TotalNumDays;
        var daysPassed = currentDayCount - lastDayCount;
        
        this.Log($"Days passed: {daysPassed}", $"Current time: {currentTime.Date}", $"Last save time: {lastSaveTime.Date}");

        for (int i = 0; i < daysPassed; i++)
        {
            DayPassed();
        }

        hasCaughtUp = true;
        CheckDayGrow();
    }
    protected abstract void DayPassed();

    public virtual void Water()
    {
        daysDry = 0;
    }

    public virtual void Harvest()
    {
        daysWithoutHarvest = 0;
    }

    public virtual void OnHourChanged(int hour)
    {
        if(hour == 0) // At midnight
        {
            daysDry++;
            if(currentState.isLastPhase)
                daysWithoutHarvest++;
                
            var validation = GrowthValidator.ValidateGrowthState(this);
            if(!validation.IsValid)
            {
                GrowthValidator.HandleFailedValidation(this, validation);
                return;
            }
        }
    }

    public virtual void CheckDayGrow() //SE FIJA LOS DIAS DEL CRECIMIENTO.
    {
        GrowingState lastState = currentState;
        currentState = states.FirstOrDefault<GrowingState>(state => state.IsThisState(daysPlanted));
        
        if(currentState != lastState) UpdateState(); // Only change mesh data if changed state
        
    }

    protected virtual void UpdateState()
    {
        meshFilter.mesh = currentState.mesh;
        meshRenderer.material = currentState.material;
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = currentState.mesh;
        }
        if (currentState.isLastPhase) SetInteractable();
        
        // Notify subscribers about the state change
        GrowthEventManager.Instance.NotifyGrowthStateChanged(this, currentState);
    }

    public virtual void SetInteractable()
    {
        if (isFruit) return;

        gameObject.layer = interactableLayerInt; //layer interactuable.
    }

    public virtual void LoadData(CropSaveData cropSaveData) // TODO: Que use como parametro un padre en común de CropSave y PlantSave
    {
        daysPlanted = cropSaveData.DiasPlantado;
        daysDry = cropSaveData.DaysDry;
        daysWithoutHarvest = cropSaveData.DaysWithoutHarvest;
        currentState = cropSaveData.GrowingState;
        UpdateState();
    }

    void OnDisable()
    {
        GrowthEventManager.Instance.OnHourChanged -= OnHourChanged;
    }
}