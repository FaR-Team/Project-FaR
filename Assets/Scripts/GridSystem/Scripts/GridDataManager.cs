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
            Vector3Int coord = entity.Coordinate;
            if (!_cells.ContainsKey(coord))
            {
                _cells[coord] = new List<IGridEntity>();
            }

            if (!_cells[coord].Contains(entity))
            {
                _cells[coord].Add(entity);
                this.Log($"Entity '{entity.EntityName}' registered at {coord}");
                entity.OnGridRegistered(coord);
            }
        }

        public void Unregister(IGridEntity entity)
        {
            Unregister(entity, entity.Coordinate);
        }

        public void Unregister(IGridEntity entity, Vector3Int coord)
        {
            if (_cells.TryGetValue(coord, out var list))
            {
                if (list.Remove(entity))
                {
                    this.Log($"Entity '{entity.EntityName}' unregistered from {coord}");
                    entity.OnGridUnregistered();
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

        public List<IGridEntity> GetEntitiesAt(Vector3Int coord)
        {
            if (_cells.TryGetValue(coord, out var list))
            {
                return new List<IGridEntity>(list);
            }
            return new List<IGridEntity>();
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
        
        public void Clear()
        {
            _cells.Clear();
        }
    }
}
