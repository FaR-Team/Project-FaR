using System.Collections.Generic;
using UnityEngine;
using Utils;
using FaRUtils.Systems.DateTime;

namespace FaRUtils.Systems.GridSystem
{
    public class GridDataManager : MonoBehaviour
    {
        private static GridDataManager _instance;
        public static GridDataManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GridDataManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("GridDataManager");
                        _instance = go.AddComponent<GridDataManager>();
                    }
                }
                return _instance;
            }
        }

        [SerializeField] private Transform boundariesParent;
        [SerializeField] private float tileSize = WorldGrid.GRID_SCALE;


        [SerializeField] private Transform restrictedZonesParent;
        [SerializeField] private Collider waterCollider;
        [SerializeField] private Transform waterZonesParent;

        [Header("Test")]
        [SerializeField] private bool showDebugGizmos = true;
        [SerializeField] private Color validCellColor = new Color(0f, 1f, 1f, 0.2f);
        [SerializeField] private Color occupiedCellColor = new Color(1f, 0f, 0f, 0.4f);
        [SerializeField] private Color cliffCellColor = Color.yellow;

        private List<Collider> restrictedColliders = new List<Collider>();
        private List<Collider> waterColliders = new List<Collider>();

        private static readonly Vector2Int[] Directions = new Vector2Int[]
        {
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(0, 1)
        };

        private readonly Dictionary<int, GridCell[]> _layers = new Dictionary<int, GridCell[]>();
        

        private Vector3Int minBounds;
        private Vector3Int maxBounds;
        private List<Collider> boundaryColliders = new List<Collider>();
        private int _width;
        private int _height;
        private bool _isInitialized = false;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            boundaryColliders.Clear();
            if (boundariesParent != null)
            {
                boundaryColliders.AddRange(boundariesParent.GetComponentsInChildren<Collider>(false));
            }

            restrictedColliders.Clear();
            if (restrictedZonesParent != null)
            {
                restrictedColliders.AddRange(restrictedZonesParent.GetComponentsInChildren<Collider>(false));
            }

            if (waterCollider == null && waterZonesParent == null)
            {
                GameObject waterGo = GameObject.Find("Water");
                if (waterGo != null)
                {
                    waterCollider = waterGo.GetComponent<Collider>();
                    if (waterCollider == null)
                    {
                        waterCollider = waterGo.GetComponentInChildren<Collider>();
                    }
                }
            }

            waterColliders.Clear();
            if (waterZonesParent != null)
            {
                waterColliders.AddRange(waterZonesParent.GetComponentsInChildren<Collider>(false));
            }

            if (boundaryColliders.Count > 0)
            {
                Bounds combinedBounds = new Bounds();
                bool hasBounds = false;
                foreach (var col in boundaryColliders)
                {
                    if (col == null) continue;
                    if (!hasBounds)
                    {
                        combinedBounds = col.bounds;
                        hasBounds = true;
                    }
                    else
                    {
                        combinedBounds.Encapsulate(col.bounds);
                    }
                }

                minBounds = WorldGrid.WorldToCell(combinedBounds.min, tileSize) - Vector3Int.one;
                maxBounds = WorldGrid.WorldToCell(combinedBounds.max, tileSize) + Vector3Int.one;
            }
            else
            {
                Debug.LogWarning("[GridDataManager] No boundary colliders found under boundariesParent! Grid boundaries are empty.");
                minBounds = Vector3Int.zero;
                maxBounds = Vector3Int.zero;
            }

            _width = maxBounds.x - minBounds.x + 1;
            _height = maxBounds.z - minBounds.z + 1;

            for (int y = minBounds.y; y <= maxBounds.y; y++)
            {
                GetOrCreateLayer(y);
            }

            CalculateEdgeCells();
        }

        private GridCell[] GetOrCreateLayer(int y)
        {
            if (!_layers.TryGetValue(y, out var layer))
            {
                layer = new GridCell[_width * _height];
                for (int z = 0; z < _height; z++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        int xCoord = minBounds.x + x;
                        int zCoord = minBounds.z + z;
                        Vector3Int coord = new Vector3Int(xCoord, y, zCoord);
                        Vector3 worldPos = WorldGrid.CellToWorld(coord, tileSize);

                        bool isInside = false;
                        if (boundaryColliders.Count > 0)
                        {
                            isInside = IsPointInColliders(worldPos, boundaryColliders);
                        }
                        else
                        {
                            isInside = true;
                        }

                        if (isInside)
                        {
                            GridCell cell = new GridCell(coord);
                            cell.IsRestrictedZoneDayOne = IsPointInColliders(worldPos, restrictedColliders);
                            layer[z * _width + x] = cell;
                        }
                        else
                        {
                            layer[z * _width + x] = null;
                        }
                    }
                }
                _layers[y] = layer;
            }
            return layer;
        }

        private void CalculateEdgeCells()
        {
            foreach (var layerPair in _layers)
            {
                var cells = layerPair.Value;
                if (cells == null) continue;

                for (int z = 0; z < _height; z++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        GridCell cell = cells[z * _width + x];
                        if (cell == null) continue;

                        cell.IsNearCliff = IsCellNearCliff(x, z, _width, _height, minBounds, layerPair.Key, cells, waterCollider, waterColliders);
                    }
                }
            }
        }

        private bool IsCellNearCliff(
            int x, int z, int width, int height, Vector3Int min, int layerY, 
            GridCell[] cells, Collider tempWaterCol, List<Collider> tempWaterCols)
        {
            foreach (var dir in Directions)
            {
                int nx = x + dir.x;
                int nz = z + dir.y;

                if (nx < 0 || nx >= width || nz < 0 || nz >= height || cells[nz * width + nx] == null)
                {
                    Vector3Int neighborCoord = new Vector3Int(min.x + nx, layerY, min.z + nz);
                    if (!IsPointInWater(neighborCoord, tempWaterCol, tempWaterCols))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsPointInColliders(Vector3 worldPos, List<Collider> colliders)
        {
            if (colliders == null || colliders.Count == 0) return false;
            foreach (var col in colliders)
            {
                if (col == null) continue;
                Bounds bounds = col.bounds;
                if (worldPos.x >= bounds.min.x - 0.1f && worldPos.x <= bounds.max.x + 0.1f &&
                    worldPos.z >= bounds.min.z - 0.1f && worldPos.z <= bounds.max.z + 0.1f &&
                    worldPos.y >= bounds.min.y - 1.0f && worldPos.y <= bounds.max.y + 1.0f)
                {
                    Vector3 closest = col.ClosestPoint(worldPos);
                    float distXZ = Vector2.Distance(new Vector2(closest.x, closest.z), new Vector2(worldPos.x, worldPos.z));
                    float distY = Mathf.Abs(closest.y - worldPos.y);
                    if (distXZ < 0.1f && distY < 1.0f)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsPointInWater(Vector3Int coord, Collider targetWaterCol, List<Collider> targetWaterCols)
        {
            Vector3 worldPos = WorldGrid.CellToWorld(coord, tileSize);
            if (targetWaterCol != null)
            {
                Bounds bounds = targetWaterCol.bounds;
                if (worldPos.x >= bounds.min.x - 0.1f && worldPos.x <= bounds.max.x + 0.1f &&
                    worldPos.z >= bounds.min.z - 0.1f && worldPos.z <= bounds.max.z + 0.1f &&
                    worldPos.y >= bounds.min.y - 1.0f && worldPos.y <= bounds.max.y + 1.0f)
                {
                    Vector3 closest = targetWaterCol.ClosestPoint(worldPos);
                    float distXZ = Vector2.Distance(new Vector2(closest.x, closest.z), new Vector2(worldPos.x, worldPos.z));
                    float distY = Mathf.Abs(closest.y - worldPos.y);
                    if (distXZ < 0.1f && distY < 1.0f)
                    {
                        return true;
                    }
                }
            }
            return IsPointInColliders(worldPos, targetWaterCols);
        }

        public GridCell GetCellAt(Vector3Int coord)
        {
            if (!_isInitialized) InitializeGrid();

            if (coord.x >= minBounds.x && coord.x <= maxBounds.x &&
                coord.z >= minBounds.z && coord.z <= maxBounds.z &&
                coord.y >= minBounds.y && coord.y <= maxBounds.y)
            {
                var layer = GetOrCreateLayer(coord.y);
                int xIdx = coord.x - minBounds.x;
                int zIdx = coord.z - minBounds.z;
                return layer[zIdx * _width + xIdx];
            }

            return null;
        }

        public void Register(IGridEntity entity)
        {
            Vector3Int pivot = entity.Coordinate;
            Vector3Int size = entity.FootprintSize;
            Vector3Int offset = entity.FootprintOffset;

            foreach (Vector3Int coord in new FootprintEnumerator(pivot, size, offset))
            {
                RegisterSingleCell(entity, coord);
            }
            entity.OnGridRegistered(pivot);
        }

        private void RegisterSingleCell(IGridEntity entity, Vector3Int coord)
        {
            GridCell cell = GetCellAt(coord);
            if (cell != null)
            {
                cell.AddOccupant(entity);

                if (entity is FaRUtils.Systems.Debris.Debris debris)
                {
                    cell.ActiveDebrisCategory = debris.Category;
                }

                if (TimeManager.Instance != null)
                {
                    cell.LastActiveDay = TimeManager.DateTime.TotalNumDays;
                }

                //this.Log($"Entity '{entity.EntityName}' registered at {coord} (Pivot: {entity.Coordinate}, Size: {entity.FootprintSize}, Offset: {entity.FootprintOffset})");
            }
        }

        public void Unregister(IGridEntity entity)
        {
            Unregister(entity, entity.Coordinate);
        }

        public void Unregister(IGridEntity entity, Vector3Int pivot)
        {
            Vector3Int size = entity.FootprintSize;
            Vector3Int offset = entity.FootprintOffset;

            foreach (Vector3Int coord in new FootprintEnumerator(pivot, size, offset))
            {
                UnregisterSingleCell(entity, coord);
            }
            entity.OnGridUnregistered();
        }

        private void UnregisterSingleCell(IGridEntity entity, Vector3Int coord)
        {
            GridCell cell = GetCellAt(coord);
            if (cell != null)
            {
                cell.RemoveOccupant(entity);

                if (entity is FaRUtils.Systems.Debris.Debris)
                {
                    cell.ActiveDebrisCategory = DebrisCategory.None;
                }

                if (TimeManager.Instance != null)
                {
                    cell.LastActiveDay = TimeManager.DateTime.TotalNumDays;
                }
            }
        }

        public T GetEntityAt<T>(Vector3Int coord) where T : class
        {
            GridCell cell = GetCellAt(coord);
            if (cell != null && cell.Occupants != null)
            {
                foreach (var occupant in cell.Occupants)
                {
                    if (occupant is T target) return target;
                }
            }
            return null;
        }

        public bool IsCellOccupied(Vector3Int coord)
        {
            GridCell cell = GetCellAt(coord);
            if (cell == null) return true;

            if (cell.Occupants != null)
            {
                foreach (var occupant in cell.Occupants)
                {
                    if (!occupant.CanOverlap) return true;
                }
            }
            return false;
        }

        public bool IsAreaOccupied(Vector3Int pivot, Vector3Int size, Vector3Int offset)
        {
            return IsAreaOccupiedIgnoring(pivot, size, offset, null);
        }

        public bool IsAreaOccupiedIgnoring(Vector3Int pivot, Vector3Int size, Vector3Int offset, IGridEntity ignoreEntity)
        {
            foreach (Vector3Int coord in new FootprintEnumerator(pivot, size, offset))
            {
                if (IsCellOccupiedIgnoring(coord, ignoreEntity)) return true;
            }
            return false;
        }

        public bool AnyEntityOfTypeInRange<T>(Vector3Int pivot, Vector3Int size, Vector3Int offset) where T : class
        {
            foreach (Vector3Int coord in new FootprintEnumerator(pivot, size, offset))
            {
                if (GetEntityAt<T>(coord) != null) return true;
            }
            return false;
        }

        private bool IsCellOccupiedIgnoring(Vector3Int coord, IGridEntity ignoreEntity)
        {
            GridCell cell = GetCellAt(coord);
            if (cell == null) return true;

            if (cell.Occupants != null)
            {
                foreach (var occupant in cell.Occupants)
                {
                    if (occupant == ignoreEntity) continue;
                    if (!occupant.CanOverlap) return true;
                }
            }
            return false;
        }

        public bool IsAreaOccupiedIgnoringDebris(Vector3Int pivot, Vector3Int size, Vector3Int offset)
        {
            foreach (Vector3Int coord in new FootprintEnumerator(pivot, size, offset))
            {
                if (coord == pivot)
                {
                    if (IsCellOccupied(coord)) return true;
                }
                else
                {
                    if (IsCellOccupiedIgnoringDebris(coord)) return true;
                }
            }
            return false;
        }

        private bool IsCellOccupiedIgnoringDebris(Vector3Int coord)
        {
            GridCell cell = GetCellAt(coord);
            if (cell == null) return true;

            if (cell.Occupants != null)
            {
                foreach (var occupant in cell.Occupants)
                {
                    if (occupant is FaRUtils.Systems.Debris.Debris) continue;
                    if (!occupant.CanOverlap) return true;
                }
            }
            return false;
        }

        public bool IsAreaOccupiedIgnoringDebrisAndEntity(Vector3Int pivot, Vector3Int size, Vector3Int offset, IGridEntity ignoreEntity)
        {
            foreach (Vector3Int coord in new FootprintEnumerator(pivot, size, offset))
            {
                if (coord == pivot)
                {
                    if (IsCellOccupiedIgnoring(coord, ignoreEntity)) return true;
                }
                else
                {
                    if (IsCellOccupiedIgnoringDebrisAndEntity(coord, ignoreEntity)) return true;
                }
            }
            return false;
        }

        private bool IsCellOccupiedIgnoringDebrisAndEntity(Vector3Int coord, IGridEntity ignoreEntity)
        {
            GridCell cell = GetCellAt(coord);
            if (cell == null) return true;

            if (cell.Occupants != null)
            {
                foreach (var occupant in cell.Occupants)
                {
                    if (occupant == ignoreEntity) continue;
                    if (occupant is FaRUtils.Systems.Debris.Debris) continue;
                    if (!occupant.CanOverlap) return true;
                }
            }
            return false;
        }

        public void RecordCellActivity(Vector3Int coord)
        {
            GridCell cell = GetCellAt(coord);
            if (cell != null && TimeManager.Instance != null)
            {
                cell.LastActiveDay = TimeManager.DateTime.TotalNumDays;
            }
        }

        public int GetDaysAbandoned(Vector3Int coord)
        {
            GridCell cell = GetCellAt(coord);
            if (cell == null) return 0;

            int currentDay = TimeManager.Instance != null ? TimeManager.DateTime.TotalNumDays : 1;
            return cell.GetDaysAbandoned(currentDay);
        }

        public bool IsNearCliff(Vector3Int coord)
        {
            GridCell cell = GetCellAt(coord);
            return cell != null && cell.IsNearCliff;
        }

        public bool IsRestrictedZoneDayOne(Vector3Int coord)
        {
            GridCell cell = GetCellAt(coord);
            return cell != null && cell.IsRestrictedZoneDayOne;
        }

        public bool HasDebrisNearby(Vector3Int center, int radius, DebrisCategory category)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int z = -radius; z <= radius; z++)
                {
                    Vector3Int neighborCoord = center + new Vector3Int(x, 0, z);
                    GridCell cell = GetCellAt(neighborCoord);
                    if (cell != null && cell.ActiveDebrisCategory == category)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Clear()
        {
            _layers.Clear();
            _isInitialized = false;
            InitializeGrid();
        }

        private Dictionary<int, GridCell[]> BuildPreviewLayers(
            Vector3Int min, Vector3Int max, int width, int height,
            List<Collider> boundaryCols, List<Collider> restrictedCols,
            Collider tempWaterCol, List<Collider> tempWaterCols)
        {
            var previewLayers = new Dictionary<int, GridCell[]>();
            for (int y = min.y; y <= max.y; y++)
            {
                var layer = new GridCell[width * height];
                for (int z = 0; z < height; z++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int xCoord = min.x + x;
                        int zCoord = min.z + z;
                        Vector3Int coord = new Vector3Int(xCoord, y, zCoord);
                        Vector3 worldPos = WorldGrid.CellToWorld(coord, tileSize);

                        bool isInside = false;
                        if (boundaryCols.Count > 0)
                        {
                            isInside = IsPointInColliders(worldPos, boundaryCols);
                        }
                        else
                        {
                            isInside = true;
                        }

                        if (isInside)
                        {
                            GridCell cell = new GridCell(coord);
                            cell.IsRestrictedZoneDayOne = IsPointInColliders(worldPos, restrictedCols);
                            layer[z * width + x] = cell;
                        }
                        else
                        {
                            layer[z * width + x] = null;
                        }
                    }
                }
                previewLayers[y] = layer;
            }

            foreach (var layerPair in previewLayers)
            {
                var cells = layerPair.Value;
                if (cells == null) continue;

                for (int z = 0; z < height; z++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        GridCell cell = cells[z * width + x];
                        if (cell == null) continue;

                        cell.IsNearCliff = IsCellNearCliff(x, z, width, height, min, layerPair.Key, cells, tempWaterCol, tempWaterCols);
                    }
                }
            }

            return previewLayers;
        }

        private void OnDrawGizmos()
        {
            if (!showDebugGizmos) return;

            bool showRestricted = true;
            if (Application.isPlaying && TimeManager.Instance != null)
            {
                var dt = TimeManager.DateTime;
                showRestricted = (dt.Date == 1 && (int)dt.Seasons == 0 && dt.Year == 1);
            }

            Vector3Int activeMinBounds = minBounds;
            Vector3Int activeMaxBounds = maxBounds;
            int activeWidth = _width;
            int activeHeight = _height;

            List<Collider> tempColliders = boundaryColliders;
            if (!Application.isPlaying && boundariesParent != null)
            {
                tempColliders = new List<Collider>(boundariesParent.GetComponentsInChildren<Collider>(false));
                if (tempColliders.Count > 0)
                {
                    Bounds combinedBounds = new Bounds();
                    bool hasBounds = false;
                    foreach (var col in tempColliders)
                    {
                        if (col == null) continue;
                        if (!hasBounds)
                        {
                            combinedBounds = col.bounds;
                            hasBounds = true;
                        }
                        else
                        {
                            combinedBounds.Encapsulate(col.bounds);
                        }
                    }
                    activeMinBounds = WorldGrid.WorldToCell(combinedBounds.min, tileSize) - Vector3Int.one;
                    activeMaxBounds = WorldGrid.WorldToCell(combinedBounds.max, tileSize) + Vector3Int.one;
                }
                else
                {
                    activeMinBounds = Vector3Int.zero;
                    activeMaxBounds = Vector3Int.zero;
                }
                activeWidth = activeMaxBounds.x - activeMinBounds.x + 1;
                activeHeight = activeMaxBounds.z - activeMinBounds.z + 1;
            }

            List<Collider> tempRestrictedColliders = new List<Collider>();
            if (!Application.isPlaying && restrictedZonesParent != null)
            {
                tempRestrictedColliders.AddRange(restrictedZonesParent.GetComponentsInChildren<Collider>(false));
            }
            else if (Application.isPlaying)
            {
                tempRestrictedColliders = restrictedColliders;
            }

            Collider tempWaterCollider = waterCollider;
            List<Collider> tempWaterColliders = new List<Collider>();
            if (!Application.isPlaying)
            {
                if (tempWaterCollider == null && waterZonesParent == null)
                {
                    GameObject waterGo = GameObject.Find("Water");
                    if (waterGo != null)
                    {
                        tempWaterCollider = waterGo.GetComponent<Collider>();
                        if (tempWaterCollider == null)
                        {
                            tempWaterCollider = waterGo.GetComponentInChildren<Collider>();
                        }
                    }
                }
                if (waterZonesParent != null)
                {
                    tempWaterColliders.AddRange(waterZonesParent.GetComponentsInChildren<Collider>(false));
                }
            }
            else
            {
                tempWaterColliders = waterColliders;
            }

            if (showRestricted && tempRestrictedColliders != null && tempRestrictedColliders.Count > 0)
            {
                Gizmos.color = new Color(1f, 0f, 0f, 0.15f);
                foreach (var col in tempRestrictedColliders)
                {
                    if (col == null) continue;
                    if (col is BoxCollider box)
                    {
                        Gizmos.matrix = col.transform.localToWorldMatrix;
                        Gizmos.DrawCube(box.center, box.size);
                        Gizmos.DrawWireCube(box.center, box.size);
                    }
                }
                Gizmos.matrix = Matrix4x4.identity;
            }

            Dictionary<int, GridCell[]> layersToDraw = null;
            if (Application.isPlaying)
            {
                layersToDraw = _layers;
            }
            else if (tempColliders != null && tempColliders.Count > 0)
            {
                layersToDraw = BuildPreviewLayers(activeMinBounds, activeMaxBounds, activeWidth, activeHeight, tempColliders, tempRestrictedColliders, tempWaterCollider, tempWaterColliders);
            }

            if (layersToDraw != null)
            {
                foreach (var layerPair in layersToDraw)
                {
                    var cells = layerPair.Value;
                    if (cells == null) continue;

                    foreach (var cell in cells)
                    {
                        if (cell == null) continue;

                        Vector3 cellPos = WorldGrid.CellToWorld(cell.Coordinate, tileSize);

                        RaycastHit[] hits = Physics.RaycastAll(cellPos + Vector3.up * 5f, Vector3.down, 10f);
                        float groundY = cellPos.y;
                        bool foundGround = false;
                        foreach (var hit in hits)
                        {
                            if (tempColliders.Contains(hit.collider)) continue;
                            if (tempRestrictedColliders.Contains(hit.collider)) continue;
                            if (!foundGround || hit.point.y > groundY)
                            {
                                groundY = hit.point.y;
                                foundGround = true;
                            }
                        }
                        if (foundGround)
                        {
                            cellPos.y = groundY + 0.02f;
                        }

                        bool isOccupied = cell.HasOccupants;
                        Gizmos.color = isOccupied ? occupiedCellColor : validCellColor;
                        Gizmos.DrawCube(cellPos, new Vector3(tileSize * 0.9f, 0.05f, tileSize * 0.9f));

                        if (cell.IsNearCliff)
                        {
                            Gizmos.color = cliffCellColor;
                            Gizmos.DrawCube(cellPos, new Vector3(tileSize * 0.7f, 0.08f, tileSize * 0.7f));
                        }

                        if (showRestricted && cell.IsRestrictedZoneDayOne)
                        {
                            Gizmos.color = occupiedCellColor;
                            float redSize = cell.IsNearCliff ? 0.4f : 0.7f;
                            float redHeight = cell.IsNearCliff ? 0.11f : 0.08f;
                            Gizmos.DrawCube(cellPos, new Vector3(tileSize * redSize, redHeight, tileSize * redSize));
                        }
                    }
                }
            }
        }
    }

    public struct FootprintEnumerator
    {
        private readonly Vector3Int pivot;
        private readonly Vector3Int size;
        private readonly Vector3Int offset;
        private int x, y, z;
        private Vector3Int current;

        public FootprintEnumerator(Vector3Int pivot, Vector3Int size, Vector3Int offset)
        {
            this.pivot = pivot;
            this.size = size;
            this.offset = offset;
            x = 0;
            y = 0;
            z = -1;
            current = Vector3Int.zero;
        }

        public Vector3Int Current => current;

        public bool MoveNext()
        {
            if (size.x <= 0 || size.y <= 0 || size.z <= 0) return false;

            z++;
            if (z >= size.z)
            {
                z = 0;
                y++;
                if (y >= size.y)
                {
                    y = 0;
                    x++;
                    if (x >= size.x)
                    {
                        return false;
                    }
                }
            }
            current = pivot + offset + new Vector3Int(x, y, z);
            return true;
        }

        public FootprintEnumerator GetEnumerator() => this;
    }
}
