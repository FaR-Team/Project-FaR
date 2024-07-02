using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FaRUtils.FPSController;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] bool sleepingSpawnpoint;
    void Start()
    {
        if (SleepHandler.Instance._isSleeping != sleepingSpawnpoint) return;
        
        var player = GameObject.FindObjectOfType<FaRCharacterController>();
        
        player.Teleport(transform);
    }
}
