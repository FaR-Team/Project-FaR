using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GrowingTreeAndPlant : GrowingBase
{

    [Header("Días para cambiar de fase")]
    
    [SerializeField] protected List<Transform> spawnPoints;
    public List<GameObject> fruits;
    public Dirt Tierra;
        
    public int ReGrow; //Veces que volvio a dar frutos.
    public int ReGrowMaxTimes; //Veces maxima que puede volver a dar frutos.

    public GameObject Prefab;

    [HideInInspector] public List<Transform> SpawnPoints => spawnPoints;
    private HashSet<Transform> availableSpawnPoints = new HashSet<Transform>();
    [HideInInspector] public int RandInt;
    [HideInInspector] public int ExpectedInt;
    public int horasQuePasaronSinFrutas;
    public int horasQueDebenPasarSinFrutas;

    protected override void Start()
    {
        base.Start();
        availableSpawnPoints = SpawnPoints.ToHashSet();
    }

    public override void OnHourChanged(int hour)
    {
        if (hour != 5) return;
        DiasPlantado++;
            
        CheckDayGrow();
        
        if (currentState.isLastPhase) SpawnFruits();
        
    }

    public Transform GetRandomSpawnPoint()
    {
        Transform point = availableSpawnPoints.ElementAt(Random.Range(0, availableSpawnPoints.Count));

        availableSpawnPoints.Remove(point);

        return point;
    }

    public virtual void SpawnFruits(int minFruits = 10, int maxFruits = 15)
    {
        if (ReGrow == ReGrowMaxTimes) return;

        RandInt = Random.Range(minFruits, maxFruits);

        for (int i = 0; i < RandInt; i++)
        {
            Transform spawnPoint = GetRandomSpawnPoint();
            GameObject fruit = Instantiate(Prefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
            fruits.Add(fruit.transform.gameObject);
        }

        ReGrow++;
    }

    protected virtual void ResetSpawnPoints()
    {
        availableSpawnPoints = spawnPoints.ToHashSet();
    }

    public void DestroyThisBush()
    {
        StartCoroutine(BushCedeLaPresidencia());
    }
    public virtual IEnumerator BushCedeLaPresidencia() //LA CONCHA DE TU MADRE SATIA QUE NOMBRE DE MIERDA.
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(transform.parent.gameObject);
    }
}
