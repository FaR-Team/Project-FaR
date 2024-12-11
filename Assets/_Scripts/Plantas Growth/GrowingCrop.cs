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

    protected override void DayPassed()
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
        
        CheckDayGrow();
    }
    
    public override void OnHourChanged(int hour)
    {
        if (hour != 4) return;
        
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
        
        CheckDayGrow();
    }
}
