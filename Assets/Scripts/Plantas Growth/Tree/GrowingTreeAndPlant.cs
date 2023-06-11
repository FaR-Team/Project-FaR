using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GrowingTreeAndPlant : GrowingBase
{
    [Header("Misc.")]

    public int DiaM; //Asumo que debe ser los dias que estuvo maduro?? no entiendo.

    [Header("Días para cambiar de fase")]

    public List<Transform> spawnPoints;
    public List<GameObject> fruits;

    public int ReGrow; //Veces que volvio a dar frutos.
    public int ReGrowTimes; //Veces maxima que puede volver a dar frutos.

    public GameObject Prefab;

    [HideInInspector] public List<Transform> SpawnPointsAvailable => spawnPoints;
    [HideInInspector] public int RandInt;
    [HideInInspector] public int ExpectedInt;
    [HideInInspector] public bool _alreadyRe = false;
    [HideInInspector] public bool yaeligio = false;
    [HideInInspector] public bool yaeligioCh = false;

    public virtual void Update()
    {
        if (ClockManager.TimeText() == "05:00 AM" && yacrecio == false)
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
        }

        if (ClockManager.TimeText() == "06:00 AM" && yacrecio == true)
        {
            yacrecio = false;
        }

        if (meshCollider.sharedMesh == meshes[meshes.Length] && yaeligio == false)
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

public virtual IEnumerator BushCedeLaPresidencia() //LA CONCHA DE TU MADRE SATIA QUE NOMBRE DE MIERDA.
    {
       // Tierra.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(0.5f);
        //Destroy(Tierra.transform.parent.gameObject);
    }
}
