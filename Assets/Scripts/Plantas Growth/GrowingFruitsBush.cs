using UnityEngine;

[RequireComponent(typeof(UniqueID))]
public class GrowingFruitsBush : GrowingCrop //CrecimientoFruta
{
    public Dirt tierra;

    public override void Start(){
        base.Start();
        tierra = transform.parent.parent.parent.GetComponent<BushGrowing>().Tierra;
    }
    
    public override void Update()
    {
        if (ClockManager.TimeText() == "05:00 AM" && !yacrecio && tierra._isWet)
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