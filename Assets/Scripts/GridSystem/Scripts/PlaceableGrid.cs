using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FaRUtils.Systems.GridSystem
{
    public class PlaceableGrid : MonoBehaviour
    {
        [Header("=== Grid Settings ===")]
        [Min(1)]
        [SerializeField] private int gridWidth = 1;
        [Min(1)]
        [SerializeField] private int gridHeight = 1;
        [Min(0.001f)]
        [SerializeField] private float tileSize = 1f;

        [SerializeField] private bool alignToWorldGrid;

        [Header("=== Gizmos ===")]
        public bool DisplayGizmos;
        public Color gizmoGridColor, gizmoPointColor;
        public float gizmoPointSize = 1f;

        private Dictionary<Vector2Int, GridPoint2D> grid;
        public Dictionary<Vector2Int, GridPoint2D> Grid => grid;

        private int halfWidth => gridWidth / 2;
        private int halfHeight => gridHeight / 2;

        public UnityAction OnGridGenerated;

        private void OnValidate()
        {
            transform.position = alignToWorldGrid ? WorldGrid.PositionFromWorldPoint3D(transform.position, tileSize) : transform.position; 
        }

        private void Start()
        {
            if (alignToWorldGrid) SnapToGrid();
            GenerateGrid();
        }

        [ContextMenu("Snap To World Grid")] // Click the three dots on the script to perform this function.
        void SnapToGrid()
        {
            if (alignToWorldGrid)
            {
                transform.position = WorldGrid.PositionFromWorldPoint3D(transform.position, tileSize);
            }
        }

        public void GenerateGrid()
        {
            if (grid == null) grid = new Dictionary<Vector2Int, GridPoint2D>();
            else ClearGrid();

            foreach (var point in EvaluateGridPoints())
            {
                grid.Add(new Vector2Int(point.X, point.Y), point);
            }

            OnGridGenerated?.Invoke();
        }

        public void ClearGrid()
        {
            foreach (Transform child in transform.Cast<Transform>().ToArray())
            {
                child.parent = null;
                DestroyImmediate(child.gameObject);
            }

            if (grid != null) grid.Clear();
        }

        private void OnDrawGizmos()
        {
            if (!DisplayGizmos) return;

            foreach (var point in EvaluateGridPoints())
            {
                Gizmos.color = gizmoPointColor;
                Gizmos.DrawSphere(point.Position, gizmoPointSize);
                Gizmos.color = gizmoGridColor;
                Gizmos.DrawWireCube(point.Position, new Vector3(1, 0.1f, 1) * tileSize);
            }
        }

        IEnumerable<GridPoint2D> EvaluateGridPoints()
        {
            for (int x = -halfWidth; x <= halfWidth; x++)
            {
                for (int y = -halfHeight; y <= halfHeight; y++)
                {
                    Vector3 worldPosition = new Vector3(x * tileSize, 0, y * tileSize);
                    yield return WorldGrid.GetGridPoint2D(new Vector2Int(x, y), transform.position + worldPosition);
                }
            }
        }

        public Vector2Int GridPointCoordinateFromWorldPoint(PlaceableGrid grid, Vector3 worldPos)
        {
            int x = Mathf.FloorToInt(grid.transform.position.x - worldPos.x);
            int y = Mathf.FloorToInt(grid.transform.position.y - worldPos.y);
            return new Vector2Int(x, y);
        }

        public Vector3 WorldPointFromCoordinate(PlaceableGrid grid, Vector2Int coord)
        {
            if (grid.Grid.TryGetValue(coord, out GridPoint2D value))
            {
                return value.Position;
            }
            else
            {
                Debug.LogError($"Tried to get a world point from coord: {coord} but wasn't found in dictionary - returning Vector3.zero");
                return Vector3.zero;
            }
        }
    }
}


