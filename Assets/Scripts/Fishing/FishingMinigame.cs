using System;
using System.Collections;
using System.Collections.Generic;
using FaRUtils.FPSController;
using UnityEngine;

public class FishingMinigame : MonoBehaviour, IMinigame
{
    private FishingSpot _currentSpot;

    public FishingSpot CurrentSpot;
    public event Action OnMinigameFinished;

    public void StartMinigame(FishingSpot spot)
    {
        _currentSpot = spot;
        _currentSpot.OnFishingFinished += EndMinigame;
        MinigameStarted();
    }

    public void MinigameStarted()
    {
        _currentSpot.EnableFishInteraction(true);
        FaRCharacterController.instance.EnableThirdPerson(true, _currentSpot.transform);
        FaRCharacterController.instance.SetMinigame(this);
    }

    public void EndMinigame()
    {
        _currentSpot.OnFishingFinished -= EndMinigame;
        _currentSpot.EnableFishInteraction(false);
        _currentSpot = null;
        OnMinigameFinished?.Invoke();
    }

    private void OnDisable()
    {
        if (_currentSpot) _currentSpot.OnFishingFinished -= EndMinigame;
    }
}

public interface IMinigame
{
    event Action OnMinigameFinished;
    void MinigameStarted();
    void EndMinigame();
}
