using UnityEngine;
using FaRUtils.Systems.DateTime;

[RequireComponent(typeof(UniqueID))]
public class GrowingFruitsBush : GrowingCrop //Crecimiento de la Fruta del arbusto
{
    protected override void Start(){
        base.Start();

        tierra = transform.parent.parent.parent.GetComponent<BushGrowing>().Tierra;
        CheckDayGrow();
    }
}