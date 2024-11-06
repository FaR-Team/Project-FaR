using System;
using FaRUtils.FPSController;
using UnityEngine;

public class LocationHandler : MonoBehaviour
{
    [SerializeField] bool sleepingSpawnpoint;
    [SerializeField] private Transform fromFarmSpawnLoc;
    [SerializeField] private Transform fromHouseSpawnLoc;
    [SerializeField] private Transform fromTownSpawnLoc;
    [SerializeField] private Locations thisLocation;
    void Start()
    {
        if (SleepHandler.Instance._isSleeping != sleepingSpawnpoint) return; // To TP player to house while sleeping

        switch (FaRCharacterController.instance.CurrentLocation)
        {
            case Locations.Farm:
                if (fromFarmSpawnLoc == null)
                {
                    Debug.LogError("Farm spawn Location not set");
                    FaRCharacterController.instance.Teleport(transform); // TP to LocationHandler if not set, use as default position
                    break;
                }
                
                FaRCharacterController.instance.Teleport(fromFarmSpawnLoc);
                break;
            case Locations.House:
                if (fromHouseSpawnLoc == null)
                {
                    Debug.LogError("House spawn Location not set");
                    FaRCharacterController.instance.Teleport(transform);
                    break;
                }
                
                FaRCharacterController.instance.Teleport(fromHouseSpawnLoc);
                break;
            case Locations.Town:
                if (fromTownSpawnLoc == null)
                {
                    Debug.LogError("Town spawn Location not set");
                    FaRCharacterController.instance.Teleport(transform);
                    break;
                }
                
                FaRCharacterController.instance.Teleport(fromTownSpawnLoc);
                break;
            default:
                FaRCharacterController.instance.Teleport(transform);
                throw new ArgumentOutOfRangeException();
        }
        
        FaRCharacterController.instance.SetLocation(thisLocation); // Update Player's Location for next TP, also can be used somewhere else
    }
}

public enum Locations
{
    Farm,
    House,
    Town
}