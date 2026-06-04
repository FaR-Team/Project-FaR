using UnityEngine;
using FaRUtils.Systems.GridSystem;

namespace FaRUtils.Systems.Debris
{
    public class Debris : MonoBehaviour, IGridEntity
    {
        [SerializeField] private Vector3Int footprintSize = Vector3Int.one;
        [SerializeField] private Vector3Int footprintOffset = Vector3Int.zero;
        [SerializeField] private bool canOverlap = false;
        [SerializeField] private DebrisCategory category = DebrisCategory.None;

        public DebrisCategory Category => category;

        private Vector3Int _registeredCoord;

        public Vector3Int Coordinate => WorldGrid.WorldToCell(transform.position);
        public Vector3Int FootprintSize => footprintSize;
        public Vector3Int FootprintOffset => footprintOffset;
        public bool CanOverlap => canOverlap;
        public string EntityName => gameObject.name;

        public void OnGridRegistered(Vector3Int coord)
        {
            _registeredCoord = coord;
        }

        public void OnGridUnregistered()
        {
        }

        private void OnDisable()
        {
            if (GridDataManager.Instance != null)
            {
                GridDataManager.Instance.Unregister(this, _registeredCoord);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Vector3 center = WorldGrid.CellToWorld(Coordinate) + (Vector3)footprintOffset * WorldGrid.GRID_SCALE;
            Vector3 size = (Vector3)footprintSize * WorldGrid.GRID_SCALE;
            Gizmos.DrawWireCube(center, size);
        }
    }
}
