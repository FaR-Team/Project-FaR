using UnityEngine;

[RequireComponent(typeof(UniqueID))]
public class GrowingFruitsTree : GrowingCrop //CrecimientoFruta
{
    public override void Start(){
        base.Start();
    }
    
    public override void Update()
    {
        if (ClockManager.TimeText() == "05:00 AM" && !yacrecio)
        {
            Dia += 1;
            yacrecio = true;
            CheckDayGrow();
        }

        if (ClockManager.TimeText() == "06:00 AM" && yacrecio)
        {
            yacrecio = false;
        }
    }

}