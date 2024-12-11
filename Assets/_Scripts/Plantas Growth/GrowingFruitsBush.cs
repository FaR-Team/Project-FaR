using UnityEngine;
using FaRUtils.Systems.DateTime;

[RequireComponent(typeof(UniqueID))]
public class GrowingFruitsBush : GrowingCrop //Crecimiento de la Fruta del arbusto
{
    protected override void Start(){
        base.Start();

        tierra = transform.parent.parent.parent.GetComponent<BushGrowing>().Tierra; // wtf
        CheckDayGrow();
    }
    
    public override void OnHourChanged(int hour)
    {
        if (hour != 4) return;

        daysPlanted++;
        CheckDayGrow();
        
        var validation = GrowthValidator.ValidateGrowthState(this);
        if(!validation.IsValid)
        {
            GrowthValidator.HandleFailedValidation(this, validation);
            return;
        }
    }

    protected override void DayPassed()
    {
        daysPlanted++;
        CheckDayGrow();
        
        var validation = GrowthValidator.ValidateGrowthState(this);
        if(!validation.IsValid)
        {
            GrowthValidator.HandleFailedValidation(this, validation);
            return;
        }
    }

    public override void LoadData(PlantData plantData)
    {
        base.LoadData(plantData);
    }
}