using System.Collections;
using UnityEngine;

public class TreeGrowing : GrowingTreeAndPlant //Crecimiento del árbol
{
    protected override void Start()
    {
        base.Start();
        CheckDayGrow();
    }

    public override void OnHourChanged(int hour) //TODO: Que esto sea más escalable.
    {
        if (hour != 5) return;
        
        daysPlanted++;
        CheckDayGrow();
        
        if (!currentState.isLastPhase) return;

        if (fruits.Count == 0) // If it's fully grown and there are no fruits, spawn them when necessary
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
            // If spawned fruits are ready to harvest
            if (fruits[0].GetComponent<GrowingFruitsTree>().currentState.isLastPhase)
            {
                if (daysWithoutHarvest >= maxDaysWithoutHarvest)
                {
                    Debug.Log($"Not harvested for {daysWithoutHarvest} days, destroying!");
                    //TODO: DestroyTree();
                }
                
                daysWithoutHarvest++; // PARA USAR EN EL FUTURO CUANDO, QUE SE PUDRAN SI NO SE COSECHAN POR 3 DIAS, REINICIAR AL COSECHAR
                
                
                gameObject.layer = interactableLayerInt;
            }
            return;
        }
       
    }

    public void StartReGrowTree()
    {
        ResetSpawnPoints();
        daysWithoutHarvest = 0;
        _reGrowCounter++;
        fruits.Clear();
        gameObject.layer = 3;
    }

    public override IEnumerator BushCedeLaPresidencia()
    {
        yield return new WaitForSeconds(0.5f);
    }

}
