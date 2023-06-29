using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance {get; private set; }

    private void Awake() {
        if (Instance == null || Instance != this)
        {
            Instance = this;
        }
    }

    [FormerlySerializedAs("AreaHarvestLevel")] public int areaHarvestLevel = 0;

    public static bool hasPants;
    public static bool hasShirt;
}
