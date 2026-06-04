using System.Collections.Generic;
using UnityEngine;

namespace FaRUtils.Systems.GridSystem
{
    public class GridCell
    {
        public Vector3Int Coordinate { get; }
        
        public List<IGridEntity> Occupants { get; private set; }

        public DebrisCategory ActiveDebrisCategory { get; set; } = DebrisCategory.None;
        public int LastActiveDay { get; set; } = 1;
        public bool IsNearCliff { get; set; } = false;
        public bool IsRestrictedZoneDayOne { get; set; } = false;

        public GridCell(Vector3Int coordinate)
        {
            Coordinate = coordinate;
        }

        public bool HasOccupants => Occupants != null && Occupants.Count > 0;

        public int GetDaysAbandoned(int currentDay)
        {
            if (HasOccupants) return 0;
            return Mathf.Max(0, currentDay - LastActiveDay);
        }

        public void AddOccupant(IGridEntity entity)
        {
            if (Occupants == null)
            {
                Occupants = new List<IGridEntity>();
            }
            if (!Occupants.Contains(entity))
            {
                Occupants.Add(entity);
            }
        }

        public bool RemoveOccupant(IGridEntity entity)
        {
            if (Occupants == null) return false;
            bool removed = Occupants.Remove(entity);
            if (Occupants.Count == 0)
            {
                Occupants = null;
            }
            return removed;
        }
    }
}
