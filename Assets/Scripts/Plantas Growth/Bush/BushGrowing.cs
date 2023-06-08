using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FaRUtils.Systems.DateTime;
using System;
using Random = UnityEngine.Random;

public class BushGrowing : GrowingInPhases
{
    public GameObject Tierra = null;


    public override void Start()
    {
        base.Start();

        Tierra = transform.root.GetChild(0).gameObject;
    }

    public override void Update()
    {
        if (Reloj.GetComponent<ClockManager>().Time.text == "05:00 AM" && yacrecio == false)
        {
            if (Dia < meshs.Length)
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

            if (Dia == meshs.Length)
            {
                DiaM += 1;
                if (DiaM == ExpectedInt)
                {
                    gameObject.layer = 7;
                }
            }
        }

        if (Reloj.GetComponent<ClockManager>().Time.text == "06:00 AM" && yacrecio == true)
        {
            yacrecio = false;
        }

        if (Dia == meshs.Length && yaeligio == false)
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
        int valueToGet = Array.IndexOf(DayIntsForChangeOfPhase, Dia);

        if (valueToGet is -1) return;

        meshCollider.sharedMesh = meshs[valueToGet];
        meshFilter.mesh = meshs[valueToGet];
        meshRenderer.material = materials[valueToGet];
    }

    public override IEnumerator BushCedeLaPresidencia()
    {
        Tierra.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(0.5f);
        Destroy(Tierra.transform.parent.gameObject);
    }
}
