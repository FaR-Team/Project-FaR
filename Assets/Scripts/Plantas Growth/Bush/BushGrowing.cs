using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FaRUtils.Systems.DateTime;
using System;
using Random = UnityEngine.Random;

public class BushGrowing : GrowingTreeAndPlant
{
    public GameObject Tierra = null;


    public override void Start()
    {
        base.Start();

        Tierra = transform.root.GetChild(0).gameObject;
    }

    public override void Update()
    {
        if (ClockManager.TimeText() == "05:00 AM" && yacrecio == false)
        {
            if (Dia < meshes.Length)
            {
                Dia++;
            }
            yacrecio = true;
            CheckDayGrow();
            if (yaeligioCh == true)
            {
                yaeligio = false;
                yaeligioCh = false;
                _alreadyRe = false;
            }

            if (Dia == meshes.Length)
            {
                DiaM += 1;
                if (DiaM == ExpectedInt)
                {
                    gameObject.layer = 7;
                }
            }
        }

        if (ClockManager.TimeText() == "06:00 AM" && yacrecio == true)
        {
            yacrecio = false;
        }

        if (Dia == meshes.Length && yaeligio == false)
        {
            PonerFruto();
        }
    } 

    public override void PonerFruto()
    {
        if (yaeligio != false || ReGrow == ReGrowTimes) return;

        RandInt = Random.Range(3, 5);

        for (int i = 0; i < RandInt; i++)
        {
            Transform Spawn = GetRandomSP();
            GameObject fruit = Instantiate(Prefab, Spawn.position, Spawn.rotation, Spawn);

            fruits.Add(fruit.transform.GetChild(2).gameObject);
        }
        DiaM = 1;
        yaeligio = true;
        ReGrow++;
    }

    public override void CheckDayGrow()
    {
        if (!yacrecio) return;
        
        foreach(int i in DayForChangeOfPhase)
        {
            if (Dia == i) {
                int valueToGet = Array.IndexOf(DayForChangeOfPhase, i);
                if (meshCollider != null)
                {
                    meshCollider.sharedMesh = meshes[valueToGet];
                }
                meshFilter.mesh = meshes[valueToGet];
                meshRenderer.material = materials[valueToGet];
                return;
            }
        }
    }

    public override IEnumerator BushCedeLaPresidencia()
    {
        Tierra.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(0.5f);
        Destroy(Tierra.transform.parent.gameObject);
    }
}
