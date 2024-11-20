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

    protected override void CatchUpMissedGrowth()
    {
        this.Log("Catching up missed growth...");
        if (hasCaughtUp) return;
        
        // Calculate days between current time and last save
        var currentTime = TimeManager.DateTime;
        var lastSaveTime = TimeManager.Instance.GetLastTimeInScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        var daysPassed = currentTime.TotalNumDays - lastSaveTime.TotalNumDays; // TODO: No va a funcionar bien, no se fija que hayan pasado las 6
        
        this.Log($"Days passed: {daysPassed}");
        this.Log($"Current time: {currentTime.Date}");
        this.Log($"Last save time: {lastSaveTime.Date}");

        for (int i = 0; i < daysPassed; i++)
        {
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
                
            var validation = GrowthValidator.ValidateGrowthState(this);
            if(!validation.IsValid)
            {
                GrowthValidator.HandleFailedValidation(this, validation);
                return;
            }
        }

        hasCaughtUp = true;
        CheckDayGrow();
    }

    public override void OnHourChanged(int hour)
    {
        if (!tierra._isWet || hour != 4) return;

        daysPlanted++;
        CheckDayGrow();
    }
    public override void CheckDayGrow() //SE FIJA LOS DIAS DEL CRECIMIENTO.
    {
        GrowingState lastState = currentState;
        currentState = states.FirstOrDefault<GrowingState>(state => state.IsThisState(daysPlanted));

        if (currentState != lastState) UpdateState(); // Only change mesh data if changed state
    }
    protected override void UpdateState()
    {
        Debug.Log("UPDATING STATE");
        skinnedMeshRenderer.material = currentState.material;
        skinnedMeshRenderer.sharedMesh = currentState.mesh;

        if (currentState.isLastPhase) SetInteractable();
    }
    public override void SetInteractable()
    {
        Instantiate(interactablePrefab, transform.position, Quaternion.identity, transform);
        skinnedMeshRenderer.sharedMesh = null;
        skinnedMeshRenderer.material = null;
    }
}