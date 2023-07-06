using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class Strawberry : FruitsInteraction
{
    public BushGrowing bushGrowing;

    public override void Awake()
    {
        base.Awake();

        bushGrowing = GetComponent<BushGrowing>();
       
    }

    public override IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (GameObject fruit in Fruits())
        {
            this.GetComponent<CropExplodeBush>().Chau(fruit);
        }

        ReGrow++;
        DiaM = 0;
        GetGameObject().layer = 0;
        if (ReGrow == ReGrowMaxTime)
        {
            StartCoroutine(Enumerator());
        }
        yaEligioCh = true;
        already = true;
    }

    public override List<GameObject> Fruits()
    {
        return bushGrowing.fruits;
    }

    public override GameObject GetGameObject()
    {
        return bushGrowing.gameObject;
    }

    public override IEnumerator Enumerator()
    {
        return bushGrowing.BushCedeLaPresidencia();
    }
}
