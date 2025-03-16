using UnityEngine;

namespace FaRUtils.Systems.GridSystem
{
    [System.Serializable]
    public struct GridPoint2D
    {
        int x;
        int y;
        Vector3 position;

        public int X => x;
        public int Y => y;
        public Vector3 Position => position;

        public GridPoint2D(Vector2Int coordinate, Vector3 worldPos)
        {
            this.x = coordinate.x;
            this.y = coordinate.y;
            position = worldPos;
        }
    }

    [System.Serializable]
    public struct GridPoint3D
    {
        int x;
        int y;
        int z;
        Vector3 position;

        public int X => x;
        public int Y => y;
        public int Z => z;
        public Vector3 Position => position;

        public GridPoint3D(Vector3Int coordinate, Vector3 worldPos)
        {
            this.x = coordinate.x;
            this.y = coordinate.y;
            this.z = coordinate.z;
            position = worldPos;
        }
    }
}
