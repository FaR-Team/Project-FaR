using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using FaRUtils.Systems.GridSystem;
using FaRUtils.Systems.DateTime;

namespace FaRUtils.Systems.Debris
{
    public class DebrisManager : MonoBehaviour
    {
        public static DebrisManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private GameObject[] debrisPrefabs;
        
        [Header("Spawning Area")]
        [SerializeField] private LayerMask spawnableLayer;
        [SerializeField] private Vector3 searchAreaCenter;
        [SerializeField] private Vector3 searchAreaSize = new Vector3(20, 0, 20);

        [Header("Settings - (First time)")]
        [SerializeField] private int initialDebrisCount = 20;

        [Header("Settings - (Every day)")]
        [SerializeField] private int dailyDebrisSpawnCount = 5;

        [Header("Rules Settings")]
        [SerializeField] private float woodBaseWeight = 1f;
        [SerializeField] private float rockBaseWeight = 1f;
        [SerializeField] private float plantBaseWeight = 1f;
        [SerializeField] private float mushroomBaseWeight = 1f;
        [SerializeField] private int litterCheckRadius = 2;
        [SerializeField] private float sameLitterTypeBonusWeight = 3f;
        [SerializeField] private float mushroomNearCliffBonusWeight = 10f;

        private Dictionary<GameObject, DebrisCategory> prefabCategories = new Dictionary<GameObject, DebrisCategory>();
        public static AllDebrisData debrisData;
        private static bool firstLoad = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (CatchUpBroadcaster.Instance != null)
            {
                CatchUpBroadcaster.Instance.OnCatchUpBroadcast += HandleCatchUp;
            }
        }

        private void Start()
        {
            if (firstLoad)
            {
                LoadDebris(true);
            }
            else
            {
                LoadDebris(false);
                firstLoad = true;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }

            if (CatchUpBroadcaster.Instance != null)
            {
                CatchUpBroadcaster.Instance.OnCatchUpBroadcast -= HandleCatchUp;
            }
        }

        public Task LoadDebris(bool temporary)
        {
            try
            {
                debrisData = LoadAllData.GetData<AllDebrisData>(temporary);

                if (!temporary)
                {
                    DebrisSaver.instance.LoadScenesData(debrisData.scenesDataList);
                }

                var sceneData = debrisData.GetSceneDataFromName(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                var debrisDataList = sceneData.datas;

                ClearActiveDebris();

                if (debrisDataList != null)
                {
                    foreach (var data in debrisDataList)
                    {
                        if (data.prefabIndex >= 0 && data.prefabIndex < debrisPrefabs.Length)
                        {
                            GameObject prefab = debrisPrefabs[data.prefabIndex];
                            GameObject instance = Instantiate(prefab, data.position, data.rotation, transform);
                            
                            Debris debris = instance.GetComponent<Debris>();
                            if (debris == null) debris = instance.AddComponent<Debris>();

                            UniqueID uid = instance.GetComponent<UniqueID>();
                            if (uid == null) uid = instance.AddComponent<UniqueID>();

                            SaveDebrisData saveDebris = instance.GetComponent<SaveDebrisData>();
                            if (saveDebris == null) saveDebris = instance.AddComponent<SaveDebrisData>();
                            saveDebris.prefabIndex = data.prefabIndex;

                            GridDataManager.Instance.Register(debris);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[DebrisManager] Failed to load debris (Reason: {e.Message}). Spawning initial debris if day one.");
                FaRUtils.Systems.DateTime.DateTime dt = TimeManager.DateTime;
                if (dt.Date == 1 && (int)dt.Seasons == 0 && dt.Year == 1)
                {
                    SpawnDebris(initialDebrisCount);
                }
            }
            return Task.CompletedTask;
        }

        private void ClearActiveDebris()
        {
            var activeDebris = GetComponentsInChildren<Debris>(true);
            foreach (var debris in activeDebris)
            {
                if (debris.transform.parent == transform)
                {
                    debris.transform.SetParent(null);
                    Destroy(debris.gameObject);
                }
            }
        }

        private void HandleCatchUp(int daysPassed)
        {
            Debug.Log($"DebrisManager: HandleCatchUp called with {daysPassed} daysPassed.");
            if (daysPassed > 0)
            {
                Debug.Log($"DebrisManager: Catching up {daysPassed} days of debris.");
                SpawnDebris(dailyDebrisSpawnCount * daysPassed);
            }
        }

        public void SpawnDebris(int count)
        {
            if (debrisPrefabs == null || debrisPrefabs.Length == 0) return;

            Bounds bounds = GetSpawningBounds();
            
            int spawnedCount = 0;
            int attempts = 0;
            int maxAttempts = count * 10;

            while (spawnedCount < count && attempts < maxAttempts)
            {
                attempts++;
                
                Vector3 randomPos = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    bounds.center.y,
                    Random.Range(bounds.min.z, bounds.max.z)
                );

                Vector3 snappedPos = WorldGrid.SnapToGrid(randomPos);
                Vector3Int cellCoord = WorldGrid.WorldToCell(snappedPos);

                var dt = TimeManager.DateTime;
                bool isDayOne = (dt.Date == 1 && (int)dt.Seasons == 0 && dt.Year == 1);
                if (isDayOne && GridDataManager.Instance.IsRestrictedZoneDayOne(cellCoord))
                {
                    continue;
                }

                if (!GridDataManager.Instance.IsCellOccupied(cellCoord) && IsGroundValid(snappedPos))
                {
                    SpawnSingleDebris(cellCoord);
                    spawnedCount++;
                }
            }

            if (spawnedCount > 0)
            {
                Debug.Log($"DebrisManager: Spawned {spawnedCount} debris items.");
            }
        }

        private bool IsGroundValid(Vector3 pos)
        {
            return Physics.Raycast(pos + Vector3.up * 2f, Vector3.down, 5f, spawnableLayer);
        }

        private Bounds GetSpawningBounds()
        {
            return new Bounds(searchAreaCenter, searchAreaSize);
        }

        private void SpawnSingleDebris(Vector3Int cellCoord)
        {
            GameObject prefab = SelectDebrisPrefabForCell(cellCoord);
            if (prefab == null) return;

            Vector3 worldPos = WorldGrid.CellToWorld(cellCoord);
            
            Quaternion randomRot = Quaternion.Euler(0, Random.Range(0, 4) * 90, 0);
            GameObject instance = Instantiate(prefab, worldPos, randomRot, transform);
            
            Debris debris = instance.GetComponent<Debris>();
            if (debris == null) debris = instance.AddComponent<Debris>();

            UniqueID uid = instance.GetComponent<UniqueID>();
            if (uid == null) uid = instance.AddComponent<UniqueID>();

            SaveDebrisData saveDebris = instance.GetComponent<SaveDebrisData>();
            if (saveDebris == null) saveDebris = instance.AddComponent<SaveDebrisData>();
            
            int prefabIndex = System.Array.IndexOf(debrisPrefabs, prefab);
            saveDebris.prefabIndex = prefabIndex;

            GridDataManager.Instance.Register(debris);
        }

        private void InitializePrefabCategories()
        {
            prefabCategories.Clear();
            foreach (var prefab in debrisPrefabs)
            {
                if (prefab == null) continue;
                Debris debris = prefab.GetComponent<Debris>();
                DebrisCategory cat = debris != null ? debris.Category : DebrisCategory.None;
                prefabCategories[prefab] = cat;
            }
        }

        private GameObject SelectDebrisPrefabForCell(Vector3Int cellCoord)
        {
            if (debrisPrefabs == null || debrisPrefabs.Length == 0) return null;

            if (prefabCategories.Count != debrisPrefabs.Length)
            {
                InitializePrefabCategories();
            }

            Dictionary<DebrisCategory, int> nearbyDebrisCounts = new Dictionary<DebrisCategory, int>();
            bool isNearCliff = GridDataManager.Instance.IsNearCliff(cellCoord);

            for (int x = -litterCheckRadius; x <= litterCheckRadius; x++)
            {
                for (int z = -litterCheckRadius; z <= litterCheckRadius; z++)
                {
                    if (x == 0 && z == 0) continue;
                    
                    Vector3Int neighborCoord = cellCoord + new Vector3Int(x, 0, z);
                    GridCell cell = GridDataManager.Instance.GetCellAt(neighborCoord);
                    if (cell != null && cell.ActiveDebrisCategory != DebrisCategory.None)
                    {
                        if (!nearbyDebrisCounts.ContainsKey(cell.ActiveDebrisCategory))
                        {
                            nearbyDebrisCounts[cell.ActiveDebrisCategory] = 0;
                        }
                        nearbyDebrisCounts[cell.ActiveDebrisCategory]++;
                    }
                }
            }

            float[] weights = new float[debrisPrefabs.Length];
            float totalWeight = 0f;

            for (int i = 0; i < debrisPrefabs.Length; i++)
            {
                GameObject prefab = debrisPrefabs[i];
                if (prefab == null) continue;

                DebrisCategory cat = prefabCategories.TryGetValue(prefab, out var c) ? c : DebrisCategory.None;
                
                float weight = GetBaseWeight(cat);

                if (cat != DebrisCategory.None && nearbyDebrisCounts.TryGetValue(cat, out int count))
                {
                    weight += count * sameLitterTypeBonusWeight;
                }

                if (cat == DebrisCategory.Mushroom)
                {
                    if (isNearCliff)
                    {
                        weight += mushroomNearCliffBonusWeight;
                    }
                    else
                    {
                        weight = 0f;
                    }
                }

                weights[i] = weight;
                totalWeight += weight;
            }

            if (totalWeight <= 0f)
            {
                return debrisPrefabs[Random.Range(0, debrisPrefabs.Length)];
            }

            float randomValue = Random.Range(0f, totalWeight);
            float currentSum = 0f;
            for (int i = 0; i < debrisPrefabs.Length; i++)
            {
                currentSum += weights[i];
                if (randomValue <= currentSum)
                {
                    return debrisPrefabs[i];
                }
            }

            return debrisPrefabs[debrisPrefabs.Length - 1];
        }

        private float GetBaseWeight(DebrisCategory category)
        {
            switch (category)
            {
                case DebrisCategory.Wood:
                    return woodBaseWeight;
                case DebrisCategory.Rock:
                    return rockBaseWeight;
                case DebrisCategory.Plant:
                    return plantBaseWeight;
                case DebrisCategory.Mushroom:
                    return mushroomBaseWeight;
                default:
                    return 0f;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 1, 1, 0.2f);
            Bounds b = GetSpawningBounds();
            Gizmos.DrawCube(b.center, b.size);
            Gizmos.DrawWireCube(b.center, b.size);
        }
    }
}