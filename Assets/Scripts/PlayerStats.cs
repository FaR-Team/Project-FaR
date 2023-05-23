using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance {get; private set; }

    private void Awake() {
        if (Instance == null || Instance != this)
        {
            Instance = this;
        }
    }
    public int AreaHarvestLevel = 0;
}
