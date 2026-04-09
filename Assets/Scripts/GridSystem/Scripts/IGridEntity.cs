using UnityEngine;

namespace FaRUtils.Systems.GridSystem
{
    public interface IGridEntity
    {
        Vector3Int Coordinate { get; }

        bool CanOverlap { get; }

        string EntityName { get; }

        void OnGridRegistered(Vector3Int coord);

        void OnGridUnregistered();
    }
}
