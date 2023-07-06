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
    
    public override void OnHourChanged(int hour)
    {
        if (hour != 5) return;
        Dia++;
            
        CheckDayGrow();
        
        if (meshCollider.sharedMesh == meshes[meshes.Length])
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

    public virtual void PonerFruto(int minFruits = 10, int maxFruits = 15)
    {
        if (ReGrow == ReGrowTimes) return;

        RandInt = Random.Range(minFruits, maxFruits);

        for (int i = 0; i < RandInt; i++)
        {
            Transform Spawn = GetRandomSP();
            GameObject fruit = Instantiate(Prefab, Spawn.position, Spawn.rotation, Spawn);
            fruits.Add(fruit.transform.gameObject);
        }
        DiaM = 1;
        ReGrow++;
    }

public virtual IEnumerator BushCedeLaPresidencia() //LA CONCHA DE TU MADRE SATIA QUE NOMBRE DE MIERDA.
    {
       // Tierra.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(0.5f);
        //Destroy(Tierra.transform.parent.gameObject);
    }
}
