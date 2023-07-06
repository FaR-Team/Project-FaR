using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GrowingInPhases : MonoBehaviour
{

    [Header("Misc.")]

    public GameObject Reloj;

    public int Dia; //Dias que pasaron desde que se plantó.
    public int DiaM; //Dias que pasaron desde que maduró.
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
    }

    public Transform GetRandomSP()
    {
        var randomSpawnPoint = Random.Range(1, SpawnPointsAvailable.Count);
        Transform transform = SpawnPointsAvailable[randomSpawnPoint];

        SpawnPointsAvailable.Remove(transform);

        return transform.transform;
    }

    public virtual void CheckDayGrow() { }

    public virtual IEnumerator BushCedeLaPresidencia() //LA CONCHA DE TU MADRE SATIA QUE NOMBRE DE MIERDA.
    {
       // Tierra.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(0.5f);
        //Destroy(Tierra.transform.parent.gameObject);

    }
}
