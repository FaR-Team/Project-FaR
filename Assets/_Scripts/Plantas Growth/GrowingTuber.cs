using System.Linq;
using FaRUtils.Systems.DateTime;
using UnityEngine;
using Utils;

[RequireComponent(typeof(UniqueID))]
public class GrowingTuber : GrowingBase
{

    private string id;
    public Dirt tierra;
    public GameObject interactablePrefab;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    protected GameObject spawnedInteractable;

    protected override void Awake()
    {
        base.Awake();
        tierra = transform.parent.GetComponent<Dirt>();
    }

    protected override void Start()
    {
        base.Start();

        id = GetComponent<UniqueID>().ID;
    }

    protected override void DayPassed()
    {
        base.DayPassed();
        
        if (tierra._isWet)
        {
            tierra.DryDirt(5);
            daysDry = 0;
            daysPlanted++;
        }
        else
        {
            daysDry++;
        }

        if(currentState.isLastPhase)
            daysWithoutHarvest++;
            
        CheckGrowState();
        
        var validation = GrowthValidator.ValidateGrowthState(this);
        if(!validation.IsValid)
        {
            GrowthValidator.HandleFailedValidation(this, validation);
            return;
        }
    }

    public override void OnHourChanged(int hour)
    {
        if (hour != 4) return;

        DayPassed();
        
    }
    public override void CheckGrowState() //SE FIJA LOS DIAS DEL CRECIMIENTO.
    {
        GrowingState lastState = currentState;
        currentState = states.FirstOrDefault<GrowingState>(state => state.IsThisState(daysPlanted));

        if (currentState != lastState) UpdateState(); // Only change mesh data if changed state
    }
    protected override void UpdateState()
    {
        //Debug.Log("UPDATING STATE");
        skinnedMeshRenderer.material = currentState.material;
        skinnedMeshRenderer.sharedMesh = currentState.mesh;

        if (currentState.isLastPhase) SpawnInteractable();
    }
    public void SpawnInteractable()
    {
        if (spawnedInteractable != null) return;
        spawnedInteractable = Instantiate(interactablePrefab, transform.position, Quaternion.identity, transform);
        skinnedMeshRenderer.sharedMesh = null;
        skinnedMeshRenderer.material = null;
    }

    public override void Die()
    {
        base.Die();
        //meshFilter.mesh = deadState.mesh;
        if(skinnedMeshRenderer)
        {
            skinnedMeshRenderer.material.SetFloat("_UseMultiplyTexture", 1f);
        }
        if(spawnedInteractable != null)
        {
            spawnedInteractable.GetComponentInChildren<SkinnedMeshRenderer>().material.SetFloat("_UseMultiplyTexture", 1f);
        }
    }
}