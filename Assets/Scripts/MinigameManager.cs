using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager instance;

    [SerializeField] FishingMinigame fishingMinigame;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else instance = this;
    }

    public void StartFishingMinigame(FishingSpot usedSpot)
    {
        fishingMinigame.StartMinigame(usedSpot);
        
        //usedSpot.OnFishingFinished += EndFishingMinigame;
    }
    
}

public enum Minigames
{
    Fishing
}
