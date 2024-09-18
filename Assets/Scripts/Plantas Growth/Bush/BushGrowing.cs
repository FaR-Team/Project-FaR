using System.Collections;
using UnityEngine;

public class BushGrowing : GrowingTreeAndPlant //Crecimiento del arbusto
{
    public GameObject TierraTexture = null;

    protected override void Start()
    {
        base.Start();

        Tierra = transform.parent.gameObject.GetComponent<Dirt>();
        TierraTexture = transform.parent.GetChild(0).gameObject;
        CheckDayGrow();
    }

    public override void OnHourChanged(int hour) //TODO: Que esto sea más escalable. Chequear solo si pasa el día, no cada hora 
    {
        if (hour != 5) return;

        if (Tierra._isWet)
        {
            daysPlanted++;
            daysDry = 0;
        }
        else
        {
            daysDry++;
            if (daysDry > maxDaysDry)
            {
                // TODO: die if dry for more than max days
            }
        }

        CheckDayGrow();

        if (!currentState.isLastPhase) return;


        if (fruits.Count == 0) // If it's fully grown and there are no fruits, spawn them
        {
            if (daysWithoutFruitsCounter >= daysToGiveFruits)
            {
                SpawnFruits(minFruitsToSpawn, maxFruitsToSpawn);
            }
            else
            {
                daysWithoutFruitsCounter++;
            }
        }
        
        if(fruits.Count > 0)
        {
            if (fruits[0].GetComponent<GrowingFruitsBush>().currentState.isLastPhase)
            {
                if (daysWithoutHarvest >= maxDaysWithoutHarvest)
                {
                    //TODO: Die 
                }
                
                daysWithoutHarvest++;
                
                gameObject.layer = interactableLayerInt;
            }
            return;
        }
    }

    public void StartReGrowBush()
    {
        ResetSpawnPoints();
        daysWithoutHarvest = 0;
        _reGrowCounter++;
        fruits.Clear();
        gameObject.layer = 3;
    }

    public override IEnumerator BushCedeLaPresidencia()
    {
        TierraTexture.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
        DirtSpawnerPooling.DeSpawn(Tierra.gameObject);
    }
}