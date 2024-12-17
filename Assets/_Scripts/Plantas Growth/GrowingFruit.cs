using UnityEngine;
using Utils;

[RequireComponent(typeof(UniqueID))]
public class GrowingFruit : GrowingCrop //CrecimientoFruta TODO: Intentar unificar todas las frutas, y depende de si tienen tierra asignada o no sabemos que tipo es
{
    protected override void Awake(){
        base.Awake();
        CheckDayGrow();
        isFruit = true;
        
    }
    public override void OnHourChanged(int hour)
    {
        if (hour != 4) return;

        DayPassed();
    }

    protected override void DayPassed()
    {
        daysPlanted++;
        CheckDayGrow();
    }

    public override void LoadData(PlantData data)
    {
        if (data is not FruitData fruitData)
        {
            this.LogError("Wasn't able to cast SaveData to a TreeBushData");
            return;
        }
        
        daysPlanted = fruitData.daysPlanted;
        currentState = fruitData.growingState;
        
        UpdateState();
    }

    public FruitData GetData()
    {
        return new FruitData(this);
    }
}