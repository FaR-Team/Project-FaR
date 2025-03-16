using System;
using UnityEngine;
using System.Linq;
using Utils;
using FaRUtils.Systems.DateTime;

public abstract class GrowingBase : MonoBehaviour
{
    protected int interactableLayerInt = 7;
    protected int initialLayerInt = 0;

    protected int daysPlanted; //Dias que pasaron desde que se plantó.
    [SerializeField] protected int maxDaysWithoutHarvest = 3;
    protected int daysWithoutHarvest;
    [SerializeField] protected int maxDaysDry = 2;
    [SerializeField] protected int daysDry;
    protected bool _isDead;

    public bool isFruit;
    [SerializeField] protected GrowingState[] states;
    public GrowingState currentState;
    [HideInInspector] public MeshFilter meshFilter;
    [HideInInspector] public MeshCollider meshCollider;
    public MeshRenderer meshRenderer;

    public int DaysPlanted => daysPlanted;
    public int DaysDry => daysDry;
    public int MaxDaysDry => maxDaysDry;
    public int DaysWithoutHarvest => daysWithoutHarvest;
    public int MaxDaysWithoutHarvest => maxDaysWithoutHarvest;
    public GrowingState CurrentState => currentState;
    
    public bool hasCaughtUp = false;
    public bool IsDead => _isDead;
    
    public event Action OnDeath;
    public event Action OnDayPassed;

    protected virtual void Awake()
    {
        TryGetComponent(out meshFilter);
        TryGetComponent(out meshCollider);
        TryGetComponent(out meshRenderer);

        initialLayerInt = gameObject.layer;
        
        UpdateState();
    }
    
    protected virtual void Start()
    {
        //CatchUpMissedGrowth();
    }

    protected virtual void CatchUpMissedGrowth(int daysPassed)
    {
        if (hasCaughtUp || _isDead) return;

        for (int i = 0; i < daysPassed; i++)
        {
            DayPassed();
        }

        hasCaughtUp = true;
        CheckGrowState();
    }

    protected virtual void DayPassed()
    {
        if (_isDead) return;
        OnDayPassed?.Invoke();
    }

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
        if(hour == 0) // At midnight TODO: probably not using, turn into abstract method
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

    public virtual void CheckGrowState() //SE FIJA LOS DIAS DEL CRECIMIENTO.
    {
        if (_isDead) return;
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

    public virtual void SetInteractable(bool interactable = true)
    {
        if (isFruit) return;

        if (interactable) gameObject.layer = interactableLayerInt; //layer interactuable.
        else gameObject.layer = initialLayerInt;

    }

    public virtual void LoadData(PlantData data) // TODO: Que use como parametro un padre en común de CropSave y PlantSave, y en los hijos usar Save as CropSave o as PlantSave
    {
        if (data is not CropSaveData cropSaveData)
        {
            this.LogError("Wasn't able to cast SaveData to a CropSaveData");
            return;
        }
        
        daysPlanted = cropSaveData.DiasPlantado;
        daysDry = cropSaveData.DaysDry;
        daysWithoutHarvest = cropSaveData.DaysWithoutHarvest;
        currentState = cropSaveData.GrowingState;
        UpdateState();
        if(cropSaveData.isDead) Die();
    }

    public virtual void Die()
    {
        _isDead = true;
        OnDeath?.Invoke();
        // meshFilter.mesh = deadState.mesh;
        if(meshRenderer)
        {
            this.Log($"MeshRenderer found on {gameObject.name}");
            meshRenderer.material.SetFloat("_UseMultiplyTexture", 1f);
            this.Log($"{meshRenderer.material.GetFloat("_UseMultiplyTexture")}");
        }
        SetInteractable(false); // Para deshabilitar interaccion y limpiar con la pala
    }

    protected virtual void OnEnable()
    {
        GrowthEventManager.Instance.OnHourChanged += OnHourChanged;
        CatchUpBroadcaster.Instance.OnCatchUpBroadcast += CatchUpMissedGrowth;
    }

    protected virtual void OnDisable()
    {
        GrowthEventManager.Instance.OnHourChanged -= OnHourChanged;
        CatchUpBroadcaster.Instance.OnCatchUpBroadcast -= CatchUpMissedGrowth;
    }
}