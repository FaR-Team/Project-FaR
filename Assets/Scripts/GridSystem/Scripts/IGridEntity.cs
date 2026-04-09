using UnityEngine;

namespace FaRUtils.Systems.GridSystem
{
    public interface IGridEntity
    {
        Vector3Int Coordinate { get; }
        Vector3Int FootprintSize { get; }
        Vector3Int FootprintOffset { get; }

        bool CanOverlap { get; }
        string EntityName { get; }

        void OnGridRegistered(Vector3Int coord);
        void OnGridUnregistered();
    }
}
