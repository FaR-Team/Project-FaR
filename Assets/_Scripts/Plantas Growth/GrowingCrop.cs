using FaRUtils.Systems.DateTime;
using UnityEngine;
using Utils;

[RequireComponent(typeof(UniqueID))]
public class GrowingCrop : GrowingBase
{
    private string id;
    public Dirt tierra;

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
        this.Log("BANANA");
        
        var gameState = GameStateLoader.gameStateData;
        if (gameState == null) return;

        // Calculate days between current time and last save
        var currentTime = TimeManager.DateTime;
        var lastSaveTime = gameState.LastSaveDateTime;
        var daysPassed = currentTime.TotalNumDays - lastSaveTime.TotalNumDays;
        
        this.Log($"Days passed: {daysPassed}");
        this.Log($"Current time: {currentTime.Date}");
        this.Log($"Last save time: {lastSaveTime.Date}");

        for (int i = 0; i < daysPassed; i++)
        {
            if (tierra._isWet)
            {
                tierra.DryDirt(5);
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
}
