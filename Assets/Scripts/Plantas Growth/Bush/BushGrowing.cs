using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using FaRUtils.Systems.DateTime;
using System.Linq;

public class BushGrowing : GrowingTreeAndPlant //Crecimiento del arbusto
{
    public Dirt Tierra = null;
    public GameObject TierraTexture = null;

    public override void Start()
    {
        base.Start();

        Tierra = transform.parent.gameObject.GetComponent<Dirt>();
        TierraTexture = transform.parent.GetChild(0).gameObject;
    }
    public override void OnHourChanged(int hour) //TO DO: Que esto sea más escalable.
    {
        if (hour == 5)
        {
            if (Tierra._isWet) DiasPlantado++;

            CheckDayGrow();
        }

        if (!IsLastStage()) return;

        if (fruits.Count > 0)
        {
            if (fruits[0].GetComponent<GrowingFruitsBush>().IsLastStage()) gameObject.layer = interactableLayerInt;
            return;
        }

        horasQuePasaronSinFrutas++;

        if (horasQuePasaronSinFrutas > horasQueDebenPasarSinFrutas)
        {
            PonerFrutos(1, 5);
            horasQuePasaronSinFrutas = 0;
        }
    }

    public void StartReGrowBush()
    {
        ReGrow++;
        horasQuePasaronSinFrutas = 0;
        fruits.Clear();
        gameObject.layer = 3;
    }

    public override IEnumerator BushCedeLaPresidencia()
    {
        TierraTexture.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(0.5f);
        Tierra.gameObject.SetActive(false);
    }

}
