using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FaRUtils.Systems.DateTime;
using UniRx;
using System;
using Random = UnityEngine.Random;

public class GrowingInPhases : MonoBehaviour
{

    [Header("Misc.")]

    public GameObject Reloj;

    public int Dia; //Dias que pasaron desde que se plantó.
    public int DiaM; //Asumo que debe ser los dias que estuvo maduro?? no entiendo.
    public bool yacrecio = false;

    [Header("Días para cambiar de fase")]

    public int[] DayIntsForChangeOfPhase;

    public List<Transform> spawnPoints;

    //public GameObject Tierra = null;


    public List<GameObject> fruits;


    public int ReGrow; //Veces que volvio a dar frutos.
    public int ReGrowTimes; //Veces maxima que puede volver a dar frutos.

    public GameObject Prefab;

    public Mesh[] meshs;
    public Material[] materials;

    [HideInInspector] public List<Transform> SpawnPointsAvailable => spawnPoints;

    [HideInInspector] public int RandInt;
    [HideInInspector] public int ExpectedInt;
    [HideInInspector] public bool _alreadyRe = false;
    [HideInInspector] public bool yaeligio = false;
    [HideInInspector] public bool yaeligioCh = false;

    [HideInInspector] public MeshFilter meshFilter;
    [HideInInspector] public MeshCollider meshCollider;
    [HideInInspector] public MeshRenderer meshRenderer;

    public virtual void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        meshRenderer= GetComponent<MeshRenderer>();
        Reloj = GameObject.Find("Reloj");
        Dia = 0;
        yacrecio = false;
        
        //Tierra = this.transform.root.gameObject;

        /*meshFilter.mesh = meshs[0];
        meshCollider.sharedMesh = meshs[0];
        meshRenderer.material = materials[0];*/
    }

    public virtual void Update()
    {
        if (Reloj.GetComponent<ClockManager>().Time.text == "05:00 AM" && yacrecio == false)
        {
            Dia++;
            yacrecio = true;
            CheckDayGrow();
            if (yaeligioCh == true)
            {
                yaeligio = false;
                yaeligioCh = false;
                _alreadyRe = false;
            }
            /*
            if(FaseFinal.activeInHierarchy)
            {
                DiaM += 1;
                if(DiaM == ExpectedInt)
                {
                    FaseFinal.layer = 7;
                }
            }*/
        }

        if (Reloj.GetComponent<ClockManager>().Time.text == "06:00 AM" && yacrecio == true)
        {
            yacrecio = false;
        }

        if (meshCollider.sharedMesh == meshs[meshs.Length] && yaeligio == false)
        {
            PonerFruto();
        }
    }

    public Transform GetRandomSP()
    {
        var randomSpawnPoint = Random.Range(1, SpawnPointsAvailable.Count);
        Transform transform = SpawnPointsAvailable[randomSpawnPoint];

        SpawnPointsAvailable.Remove(transform);

        return transform.transform;
    }

    public virtual void PonerFruto()
    {
        if (yaeligio != false || ReGrow == ReGrowTimes) return;


        RandInt = Random.Range(10, 15);

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

    public virtual void CheckDayGrow()
    {
        /*
        #region IfSpagetti

        if (!yacrecio) return;


        if(Dia == Int1 && yacrecio == true)
        {
            Fase1.SetActive(false);
            Fase2.SetActive(true);
        }

        if (Fase3 == null)
        {
            return;
        }
        
        if(Dia == Int2 && yacrecio == true)
        {
            Destroy(Fase1);
            Fase2.SetActive(false);
            Fase3.SetActive(true);
        }
        
        if (Fase4 == null)
        {
            return;
        }

        if(Dia == Int3 && yacrecio == true)
        {
            Destroy(Fase2);
            Fase3.SetActive(false);
            Fase4.SetActive(true);
        }

        if (Fase5 == null)
        {
            return;
        } //TODO: Preguntarle a Sasha cómo se dice frutilla en Swahili.
        
        if(Dia == Int4 && yacrecio == true)
        {
            Destroy(Fase3);
            Fase4.SetActive(false);
            Fase5.SetActive(false);
        }

        if (Fase6 == null)
        {
            return;
        }

        if(Dia == Int5 && yacrecio == true)
        {
            Destroy(Fase4);
            Fase5.SetActive(false);
            Fase6.SetActive(false);
        }

        if (Fase7 == null)
        {
            return;
        }

        if(Dia == Int6 && yacrecio == true)
        {
            Destroy(Fase5);
            Fase6.SetActive(false);
            Fase7.SetActive(false);
        }

        if (Fase8 == null)
        {
            return;
        }

        if(Dia == Int7 && yacrecio == true)
        {
            Destroy(Fase6);
            Fase7.SetActive(false);
            Fase8.SetActive(false);
        }

        if (Fase9 == null)
        {
            return;
        }

        if(Dia == Int8 && yacrecio == true)
        {
            Destroy(Fase7);
            Fase8.SetActive(false);
            Fase9.SetActive(false);
        }

        if (Fase10 == null)
        {
            return;
        }

        if(Dia == Int9 && yacrecio == true)
        {
            Destroy(Fase8);
            Fase9.SetActive(false);
            Fase10.SetActive(false);
        }
        #endregion */
    }

    public virtual IEnumerator BushCedeLaPresidencia() //LA CONCHA DE TU MADRE SATIA QUE NOMBRE DE MIERDA.
    {
       // Tierra.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(0.5f);
        //Destroy(Tierra.transform.parent.gameObject);

    }
}
