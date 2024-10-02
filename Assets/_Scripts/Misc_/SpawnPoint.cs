using FaRUtils.FPSController;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] bool sleepingSpawnpoint;
    void Start()
    {
        if (SleepHandler.Instance._isSleeping != sleepingSpawnpoint) return;

        FaRCharacterController.instance.Teleport(transform);
    }
}
