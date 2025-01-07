using System;
using UnityEngine;
using Utils;

[RequireComponent(typeof(UniqueID))]
public class GrowingFruit : GrowingCrop //CrecimientoFruta TODO: Intentar unificar todas las frutas, y depende de si tienen tierra asignada o no sabemos que tipo es
{
    protected GrowingTreeAndPlant _parentPlant;
    protected override void Awake(){
        base.Awake();
        CheckGrowState();
        isFruit = true;
        
    }

    protected override void OnEnable()
    {
        _parentPlant.OnDayPassed += DayPassed;
        _parentPlant.OnDeath += Die;
    }
    
    protected override void OnDisable()
    {
        _parentPlant.OnDayPassed -= DayPassed;
        _parentPlant.OnDeath -= Die;
    }

    public void SetParentPlant(GrowingTreeAndPlant plant) => _parentPlant = plant;
    public override void OnHourChanged(int hour){} // Que las frutas escuchen al evento de la planta padre para el DayPassed, para que dependan de la misma

    protected override void DayPassed()
    {
        //base.DayPassed(); TODO: Separar la lógica y que esta clase ya no herede de crop
        
        daysPlanted++;
        CheckGrowState();
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