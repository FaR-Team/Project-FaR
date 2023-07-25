using System.Collections;
using UnityEngine;

public class TreeGrowing : GrowingTreeAndPlant //Crecimiento del árbol
{
    public override void OnHourChanged(int hour) //TODO: Que esto sea más escalable.
    {
        if (hour == 5)
        {
            DiasPlantado++;
            CheckDayGrow();
        }

        if (!IsLastStage()) return;

        if (fruits.Count > 0)
        {
            if (fruits[0].GetComponent<GrowingFruitsTree>().IsLastStage()) gameObject.layer = interactableLayerInt;
            return;
        }

        horasQuePasaronSinFrutas++;

        if (horasQuePasaronSinFrutas > horasQueDebenPasarSinFrutas)
        {
            PonerFrutos(15, 20);
            horasQuePasaronSinFrutas = 0;
        }
    }

    public void StartReGrowTree()
    {
        ReGrow++;
        horasQuePasaronSinFrutas = 0;
        fruits.Clear();
        gameObject.layer = 3;
    }

    public override IEnumerator BushCedeLaPresidencia()
    {
        yield return new WaitForSeconds(0.5f);
    }

}
