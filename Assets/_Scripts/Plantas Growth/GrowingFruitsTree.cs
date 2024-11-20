using UnityEngine;

[RequireComponent(typeof(UniqueID))]
public class GrowingFruitsTree : GrowingCrop //CrecimientoFruta
{
    protected override void Awake(){
        base.Awake();
        CheckDayGrow();
        isFruit = true;
        
    }
    public override void OnHourChanged(int hour)
    {
        if (hour != 4) return;

        daysPlanted++;
        CheckDayGrow();
    }

    protected override void DayPassed()
    {
        daysPlanted++;
    }
}