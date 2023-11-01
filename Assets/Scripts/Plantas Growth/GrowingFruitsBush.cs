using UnityEngine;
using FaRUtils.Systems.DateTime;

[RequireComponent(typeof(UniqueID))]
public class GrowingFruitsBush : GrowingCrop //Crecimiento de la Fruta del arbusto
{
    public override void Start(){
        base.Start();

        tierra = transform.parent.parent.parent.GetComponent<BushGrowing>().Tierra;
        CheckDayGrow();
    }
}