using System.Collections.Generic;
using UnityEngine;

namespace FaRUtils.Systems.GridSystem
{
    public static class WorldGrid
    {
        /// <summary>
        /// Get a grid aligned position from a given coordinate.
        /// </summary>
        /// <param name="coordinate">The grid coordinate</param>
        /// <param name="gridScale">The size of grid cells, as a Vector3 to allow for nonuniform grid dimensions</param>
        /// <returns>Scaled coordinate as a Vector3Int</returns>
        public static Vector3 PositionFromCoordinate3D(Vector3Int coordinate, Vector3 gridScale)
        {
            var x = Mathf.Round(coordinate.x * gridScale.x);
            var y = Mathf.Round(coordinate.y * gridScale.y);
            var z = Mathf.Round(coordinate.z * gridScale.z);

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Get a grid aligned position from a given coordinate.
        /// </summary>
        /// <param name="coordinate">The grid coordinate</param>
        /// <param name="gridScale">The size of grid cells, as a float to allow for uniform grid dimensions</param>
        /// <returns>Scaled coordinate as a Vector3Int</returns>
        public static Vector3 PositionFromCoordinate3D(Vector3Int coordinate, float gridScale)
        {
            return PositionFromCoordinate3D(coordinate, new Vector3(gridScale, gridScale, gridScale));
        }

        /// <summary>
        /// Get a 3D coordinate from a world point.
        /// </summary>
        /// <param name="worldPos">The position from which to generate a coordinate</param>
        /// <param name="gridScale">The size of grid cells, as a Vector3 to allow for nonuniform grid dimensions</param>
        /// <returns>3D Coordinate from a world point</returns>
        public static Vector3Int CoordinateFromWorldPoint3D(Vector3 worldPos, Vector3 gridScale)
        {
            var x = Mathf.RoundToInt(worldPos.x / gridScale.x);
            var y = Mathf.RoundToInt(worldPos.y / gridScale.y);
            var z = Mathf.RoundToInt(worldPos.z / gridScale.z);

            return new Vector3Int(x, y, z);
        }

        /// <summary>
        /// Get a 3D coordinate from a world point.
        /// </summary>
        /// <param name="worldPos">The position from which to generate a coordinate</param>
        /// <param name="gridScale">The size of grid cells, as a float to allow for uniform grid dimensions</param>
        /// <returns>3D Coordinate from a world point</returns>
        public static Vector3Int CoordinateFromWorldPoint3D(Vector3 worldPos, float gridScale)
        {
            return CoordinateFromWorldPoint3D(worldPos, new Vector3(gridScale, gridScale, gridScale));
        }

        /// <summary>
        /// Given a world position, this returns back a grid aligned position.
        /// </summary>
        /// <param name="worldPos">The position from which to generate a coordinate</param>
        /// <param name="gridScale">The size of grid cells, as a Vector3 to allow for nonuniform grid dimensions</param>
        /// <returns>Grid aligned Vector3 position</returns>
        public static Vector3 PositionFromWorldPoint3D(Vector3 worldPos, Vector3 gridScale)
        {
            var x = Mathf.Round(worldPos.x / gridScale.x) * gridScale.x;
            var y = Mathf.Round(worldPos.y / gridScale.y) * gridScale.y;
            var z = Mathf.Round(worldPos.z / gridScale.y) * gridScale.z;

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Given a world position, this returns back a grid aligned position.
        /// </summary>
        /// <param name="worldPos">The position from which to generate a coordinate</param>
        /// <param name="gridScale">The size of grid cells, as a float to allow for uniform grid dimensions</param>
        /// <returns>Grid aligned Vector3 position</returns>
        public static Vector3 PositionFromWorldPoint3D(Vector3 worldPos, float gridScale)
        {
            return PositionFromWorldPoint3D(worldPos, new Vector3(gridScale, gridScale, gridScale));
        }

        public static Vector3 PositionXZFromWorldPoint2D(Vector3 worldPos, float gridScale)
        {
            var pos = PositionFromWorldPoint3D(worldPos, new Vector3(gridScale, gridScale, gridScale));
            return new Vector3(pos.x, worldPos.y, pos.z);
        }

        /// <summary>
        /// Generate a 3D Grid Point
        /// </summary>
        /// <param name="coord">The coordinate</param>
        /// <param name="worldPos">The world position</param>
        /// <returns>3D Grid Point</returns>
        public static GridPoint3D GetGridPoint3D(Vector3Int coord, Vector3 worldPos)
        {
            return new GridPoint3D(coord, worldPos);
        }

        /// <summary>
        /// Generate a 2D Grid Point
        /// </summary>
        /// <param name="coord">The coordinate</param>
        /// <param name="worldPos">The world position</param>
        /// <returns>2D Grid Point</returns>
        public static GridPoint2D GetGridPoint2D(Vector2Int coord, Vector3 worldPos)
        {
            return new GridPoint2D(coord, worldPos);
        }

        /// <summary>
        /// Get grid positions in all dimensions in range of a world position taking into account the grid scale.
        /// </summary>
        /// <param name="worldPos">The world position to search from</param>
        /// <param name="gridScale">The scale of the grid</param>
        /// <param name="range">The range in each dimension you would like search.</param>
        /// <returns>Returns IEnumerable Vector3 which allows you to loop through them.</returns>
        public static IEnumerable<Vector3> GetGridPositionsWithinRange3D(Vector3 worldPos, float gridScale, int range)
        {
            for (int x = -range; x < range + 1; x++)
            {
                for (int y = -range; y < range + 1; y++)
                {
                    for (int z = -range; z < range + 1; z++)
                    {
                        var offsetPos = new Vector3(worldPos.x + (x * gridScale), worldPos.y + (y * gridScale), worldPos.z + (z * gridScale));
                        yield return PositionFromWorldPoint3D(offsetPos, gridScale);
                    }
                }
            }
        }

        /// <summary>
        /// Get grid positions in XZ dimensions in range of a world position taking into account the grid scale.
        /// </summary>
        /// <param name="worldPos">The world position to search from</param>
        /// <param name="gridScale">The scale of the grid</param>
        /// <param name="range">The range in each dimension you would like search.</param>
        /// <returns>Returns IEnumerable Vector3 which allows you to loop through them.</returns>
        public static IEnumerable<Vector3> GetXZGridPositionsWithinRange2D(Vector3 worldPos, float gridScale, int range)
        {
            for (int x = -range; x < range + 1; x++)
            {
                for (int z = -range; z < range + 1; z++)
                {
                    var offsetPos = new Vector3(worldPos.x + (x * gridScale), worldPos.y, worldPos.z + (z * gridScale));
                    yield return PositionFromWorldPoint3D(offsetPos, gridScale);
                }
            }
        }

        public static IEnumerable<Vector3> GetXYGridPositionsWithinRange2D(Vector3 worldPos, float gridScale, int range)
        {
            for (int x = -range; x < range + 1; x++)
            {
                for (int y = -range; y < range + 1; y++)
                {
                    var offsetPos = new Vector3(worldPos.x + (x * gridScale), worldPos.y + (y * gridScale), worldPos.z);
                    yield return PositionFromWorldPoint3D(offsetPos, gridScale);
                }
            }
        }

        public static IEnumerable<Vector3> GetYZGridPositionsWithinRange2D(Vector3 worldPos, float gridScale, int range)
        {
            for (int y = -range; y < range + 1; y++)
            {
                for (int z = -range; z < range + 1; z++)
                {
                    var offsetPos = new Vector3(worldPos.x, worldPos.y + (y * gridScale), worldPos.z + (z * gridScale));
                    yield return PositionFromWorldPoint3D(offsetPos, gridScale);
                }
            }
        }
    }
}

