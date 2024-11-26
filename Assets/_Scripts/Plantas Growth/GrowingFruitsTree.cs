using UnityEngine;

[RequireComponent(typeof(UniqueID))]
public class GrowingFruitsTree : GrowingCrop //CrecimientoFruta TODO: Intentar unificar todas las frutas, y depende de si tienen tierra asignada o no sabemos que tipo es
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