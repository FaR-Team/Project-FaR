using UnityEngine;

[RequireComponent(typeof(UniqueID))]
public class GrowingFruitsTree : GrowingCrop //CrecimientoFruta
{
    void Awake(){
        isFruit = true;
    }
    public override void OnHourChanged(int hour)
    {
        
        if (hour != 5) return;

        DiasPlantado++;
        CheckDayGrow();
    }
}