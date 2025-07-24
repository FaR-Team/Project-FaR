using System;
using System.Collections;
using System.Collections.Generic;
using FaRUtils.FPSController;
using UnityEngine;

public class FishingMinigame : MonoBehaviour, IMinigame
{
    private FishingSpot _currentSpot;
    
    public event Action OnMinigameFinished;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartMinigame(FishingSpot spot)
    {
        _currentSpot = spot;
        MinigameStarted();
    }

    public void MinigameStarted()
    {
        _currentSpot.EnableFishInteraction(true);
        FaRCharacterController.instance.EnableThirdPerson(true);
        FaRCharacterController.instance.SetMinigame(this);
    }

    public void EndMinigame()
    {
        _currentSpot.EnableFishInteraction(false);
        
        _currentSpot = null;
        OnMinigameFinished?.Invoke();
    }
}

public interface IMinigame
{
    event Action OnMinigameFinished;
    void MinigameStarted();
    void EndMinigame();
}
