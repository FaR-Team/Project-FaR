using System.Collections.Generic;
using UnityEngine;
using Utils;

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

        private readonly Dictionary<Vector3Int, List<IGridEntity>> _cells = new Dictionary<Vector3Int, List<IGridEntity>>();

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Register(IGridEntity entity)
        {
            Vector3Int pivot = entity.Coordinate;
            Vector3Int size = entity.FootprintSize;
            Vector3Int offset = entity.FootprintOffset;

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        Vector3Int coord = pivot + offset + new Vector3Int(x, y, z);
                        RegisterSingleCell(entity, coord);
                    }
                }
            }
            entity.OnGridRegistered(pivot);
        }

        private void RegisterSingleCell(IGridEntity entity, Vector3Int coord)
        {
            if (!_cells.ContainsKey(coord))
            {
                _cells[coord] = new List<IGridEntity>();
            }

            if (!_cells[coord].Contains(entity))
            {
                _cells[coord].Add(entity);
                this.Log($"Entity '{entity.EntityName}' registered at {coord} (Pivot: {entity.Coordinate}, Size: {entity.FootprintSize}, Offset: {entity.FootprintOffset})");
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

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        Vector3Int coord = pivot + offset + new Vector3Int(x, y, z);
                        UnregisterSingleCell(entity, coord);
                    }
                }
            }
            entity.OnGridUnregistered();
        }

        private void UnregisterSingleCell(IGridEntity entity, Vector3Int coord)
        {
            if (_cells.TryGetValue(coord, out var list))
            {
                if (list.Remove(entity))
                {
                    // this.Log($"Entity '{entity.EntityName}' unregistered from {coord}");
                }

                if (list.Count == 0)
                {
                    _cells.Remove(coord);
                }
            }
        }


        public T GetEntityAt<T>(Vector3Int coord) where T : class
        {
            if (_cells.TryGetValue(coord, out var list))
            {
                foreach (var entity in list)
                {
                    if (entity is T target) return target;
                }
            }
            return null;
        }

        public bool IsCellOccupied(Vector3Int coord)
        {
            if (_cells.TryGetValue(coord, out var list))
            {
                foreach (var entity in list)
                {
                    if (!entity.CanOverlap) return true;
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
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        Vector3Int coord = pivot + offset + new Vector3Int(x, y, z);
                        if (IsCellOccupiedIgnoring(coord, ignoreEntity)) return true;
                    }
                }
            }
            return false;
        }

        public bool AnyEntityOfTypeInRange<T>(Vector3Int pivot, Vector3Int size, Vector3Int offset) where T : class
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        Vector3Int coord = pivot + offset + new Vector3Int(x, y, z);
                        if (GetEntityAt<T>(coord) != null) return true;
                    }
                }
            }
            return false;
        }

        private bool IsCellOccupiedIgnoring(Vector3Int coord, IGridEntity ignoreEntity)
        {
            if (_cells.TryGetValue(coord, out var list))
            {
                foreach (var entity in list)
                {
                    if (entity == ignoreEntity) continue;
                    if (!entity.CanOverlap) return true;
                }
            }
            return false;
        }
        
        public void Clear()
        {
            _cells.Clear();
        }
    }
}
