using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Random = UnityEngine.Random;

public class GrowingTreeAndPlant : GrowingBase
{
    [SerializeField] protected List<Transform> spawnPoints;
    public List<GameObject> fruits;
    public Dirt Tierra;

    [Header("Fruits Settings")]
    [SerializeField] protected int daysToGiveFruits = 1;
    [SerializeField] protected int minFruitsToSpawn;
    [SerializeField] protected int maxFruitsToSpawn;
    protected int daysWithoutFruitsCounter;
    protected int _reGrowCounter; //Veces que volvio a dar frutos.
    public int ReGrowMaxTimes; //Veces maxima que puede volver a dar frutos.

    [FormerlySerializedAs("Prefab")] public GameObject fruitPrefab;

    [HideInInspector] public List<Transform> SpawnPoints => spawnPoints;
    protected HashSet<Transform> availableSpawnPoints = new HashSet<Transform>();
    protected HashSet<Transform> usedSpawnPoints = new HashSet<Transform>();
    [HideInInspector] public int RandInt;
    [HideInInspector] public int ExpectedInt;

    public int ReGrowCounter => _reGrowCounter;
    public int DaysWithoutFruits => daysWithoutFruitsCounter;
    public HashSet<Transform> UsedSpawnPoints => usedSpawnPoints;

    protected override void Awake()
    {
        base.Awake();
        availableSpawnPoints = SpawnPoints.ToHashSet();
    }
    
    protected override void DayPassed()
    {
        if (Tierra != null) // If dirt is null, it means this plant does not need water
        {
            if (Tierra._isWet)
            {
                Tierra.DryDirt(5);
                daysDry = 0;
                daysPlanted++;
            }
            else daysDry++;

        }
        else daysPlanted++;
        
        if (!currentState.isLastPhase)
        {
            CheckDayGrow(); // Try update state anyway, if it's last phase, it'll check day grow down there
            
            var prevalidation = GrowthValidator.ValidateGrowthState(this);
            if(!prevalidation.IsValid)
            {
                GrowthValidator.HandleFailedValidation(this, prevalidation);
                return;
            }
            
            return;
        }

        if (fruits.Count == 0) // If it's fully grown and there are no fruits, spawn them when necessary
        {

            if (daysWithoutFruitsCounter >= daysToGiveFruits)
            {
                SpawnFruits(minFruitsToSpawn, maxFruitsToSpawn);
            }
            else
            {
                daysWithoutFruitsCounter++;
            }
        }
        
        if(fruits.Count > 0)
        {
            // If spawned fruits are ready to harvest
            if (fruits[0].GetComponent<GrowingCrop>().currentState.isLastPhase)
            {
                if (daysWithoutHarvest >= maxDaysWithoutHarvest)
                {
                    this.Log($"Not harvested for {daysWithoutHarvest} days, destroying!");
                    //TODO: DestroyTree();
                }
                
                daysWithoutHarvest++; // PARA USAR EN EL FUTURO CUANDO, QUE SE PUDRAN SI NO SE COSECHAN POR 3 DIAS, REINICIAR AL COSECHAR
                
                
                gameObject.layer = interactableLayerInt;
            }
            return;
        }
        
        CheckDayGrow();
        
        var validation = GrowthValidator.ValidateGrowthState(this);
        if(!validation.IsValid)
        {
            GrowthValidator.HandleFailedValidation(this, validation);
            return;
        }
    }

    public override void OnHourChanged(int hour)
    {
        if (hour != 5) return;
        DayPassed();
    }

    public Transform GetRandomSpawnPoint()
    {
        if (availableSpawnPoints.Count == 0) return null;
        
        Transform point = availableSpawnPoints.ElementAt(Random.Range(0, availableSpawnPoints.Count));

        availableSpawnPoints.Remove(point);
        usedSpawnPoints.Add(point);

        return point;
    }

    public virtual void SpawnFruits(int minFruits = 10, int maxFruits = 15)
    {
        if (_reGrowCounter == ReGrowMaxTimes) return;
        
        daysWithoutFruitsCounter = 0;

        RandInt = Random.Range(minFruits, maxFruits);

        for (int i = 0; i < RandInt; i++)
        {
            Transform spawnPoint = GetRandomSpawnPoint();
            if (spawnPoint == null) return;
            
            GameObject fruit = Instantiate(fruitPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
            fruits.Add(fruit.transform.gameObject);
        }
    }

    protected virtual void ResetSpawnPoints()
    {
        availableSpawnPoints = spawnPoints.ToHashSet();
        usedSpawnPoints.Clear();
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
    
    protected override void UpdateState()
    {
        meshFilter.mesh = currentState.mesh;
        meshRenderer.material = currentState.material;
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = currentState.mesh;
        }
        if (currentState.isLastPhase && fruits.Count > 0) SetInteractable();
        
        // Notify subscribers about the state change
        GrowthEventManager.Instance.NotifyGrowthStateChanged(this, currentState);
    }

    public void SpawnLoadedFruits(HashSet<Transform> usedPositions)
    {
        foreach (var pos in usedPositions)
        {
            GameObject fruit = Instantiate(fruitPrefab, pos.position, pos.rotation, pos); // TODO: Cargar estado de frutas, como las manzanas tienen 1 no es necesario
            fruits.Add(fruit.transform.gameObject);
        }
    }
    
    public override void LoadData(PlantData data)
    {
        if (data is not TreeBushData plantData)
        {
            this.LogError("Wasn't able to cast SaveData to a TreeBushData");
            return;
        }
        
        daysPlanted = plantData.daysPlanted;
        daysWithoutHarvest = plantData.daysWithoutHarvest;
        daysWithoutFruitsCounter = plantData.daysWithoutFruitsCounter;
        _reGrowCounter = plantData.reGrowCounter;
        PopulateUsedSpawnpointsByPositions(plantData.usedSpawnPointsPos);
        currentState = plantData.growingState;
        transform.position = plantData.position;
        daysDry = plantData.daysDry;
        
        if(usedSpawnPoints.Count > 0) SpawnLoadedFruits(usedSpawnPoints);
        
        UpdateState();
    }

    /// <summary>
    /// Used when Loading tree data, since Transform List can't be serialized properly
    /// </summary>
    /// <param name="positions"></param>
    void PopulateUsedSpawnpointsByPositions(List<Vector3> positions)
    {
        for (int i = 0; i < positions.Count; i++)
        {
            var spawnpoint = availableSpawnPoints.FirstOrDefault(t => t.localPosition == positions[i]);
            if (spawnpoint == null)
            {
                Debug.LogError("Error when loading UsedSpawnpoints, not found in Available Spawnpoints");
                continue;
            }
            
            availableSpawnPoints.Remove(spawnpoint);
            usedSpawnPoints.Add(spawnpoint);
        }
    }
}
