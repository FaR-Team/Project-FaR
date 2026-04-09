using System.Collections.Generic;
using UnityEngine;

namespace FaRUtils.Systems.GridSystem
{
    public static class WorldGrid
    {
        public const float GRID_SCALE = 2.1f;

        public static Vector3Int WorldToCell(Vector3 worldPos, float gridScale = GRID_SCALE)
        {
            return new Vector3Int(
                Mathf.FloorToInt(worldPos.x / gridScale),
                Mathf.FloorToInt(worldPos.y / gridScale),
                Mathf.FloorToInt(worldPos.z / gridScale)
            );
        }

        public static Vector3 CellToWorld(Vector3Int cell, float gridScale = GRID_SCALE)
        {
            return new Vector3(
                (cell.x + 0.5f) * gridScale,
                (cell.y * gridScale) + 0.1f,
                (cell.z + 0.5f) * gridScale
            );
        }

        public static Vector3 SnapToGrid(Vector3 worldPos, float gridScale = GRID_SCALE)
        {
            return CellToWorld(WorldToCell(worldPos, gridScale), gridScale);
        }

        public static Vector3 PositionXZFromWorldPoint2D(Vector3 worldPos, float gridScale = GRID_SCALE)
        {
            Vector3Int cell = WorldToCell(worldPos, gridScale);
            return new Vector3((cell.x + 0.5f) * gridScale, worldPos.y, (cell.z + 0.5f) * gridScale);
        }

        public static IEnumerable<Vector3> GetGridPositionsWithinRange3D(Vector3 worldPos, float gridScale, int range)
        {
            Vector3Int center = WorldToCell(worldPos, gridScale);
            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    for (int z = -range; z <= range; z++)
                    {
                        yield return CellToWorld(center + new Vector3Int(x, y, z), gridScale);
                    }
                }
            }
        }

        public static IEnumerable<Vector3> GetXZGridPositionsWithinRange2D(Vector3 worldPos, float gridScale, int range)
        {
             Vector3Int center = WorldToCell(worldPos, gridScale);
             for (int x = -range; x <= range; x++)
             {
                 for (int z = -range; z <= range; z++)
                 {
                     yield return CellToWorld(new Vector3Int(center.x + x, center.y, center.z + z), gridScale);
                 }
             }
        }

        public static Vector3 PositionFromWorldPoint3D(Vector3 worldPos, float gridScale = GRID_SCALE)
        {
            return SnapToGrid(worldPos, gridScale);
        }

        public static IEnumerable<Vector3> GetXYGridPositionsWithinRange2D(Vector3 worldPos, float gridScale, int range)
        {
            Vector3Int center = WorldToCell(worldPos, gridScale);
            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    yield return CellToWorld(new Vector3Int(center.x + x, center.y + y, center.z), gridScale);
                }
            }
        }

        public static IEnumerable<Vector3> GetYZGridPositionsWithinRange2D(Vector3 worldPos, float gridScale, int range)
        {
            Vector3Int center = WorldToCell(worldPos, gridScale);
            for (int y = -range; y <= range; y++)
            {
                for (int z = -range; z <= range; z++)
                {
                    yield return CellToWorld(new Vector3Int(center.x, center.y + y, center.z + z), gridScale);
                }
            }
        }

        public static GridPoint3D GetGridPoint3D(Vector3Int coord, Vector3 worldPos) => new GridPoint3D(coord, worldPos);
        public static GridPoint2D GetGridPoint2D(Vector2Int coord, Vector3 worldPos) => new GridPoint2D(coord, worldPos);
    }
}

