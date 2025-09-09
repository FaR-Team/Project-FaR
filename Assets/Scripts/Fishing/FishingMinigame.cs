using System;
using System.Collections;
using System.Collections.Generic;
using FaRUtils.FPSController;
using UnityEngine;

public class FishingMinigame : MonoBehaviour, IMinigame
{
    [SerializeField] private float endDelay;
    private FishingSpot _currentSpot;
    public MinigameTools Tool => MinigameTools.Spear;
    public FishingSpot CurrentSpot => _currentSpot;
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
        StartCoroutine(EndCoroutine());
    }

    private void OnDisable()
    {
        if (_currentSpot) _currentSpot.OnFishingFinished -= EndMinigame;
    }

    IEnumerator EndCoroutine()
    {
        FaRCharacterController.instance.DisableMinigameInput();
        _currentSpot.OnFishingFinished -= EndMinigame;
        _currentSpot.EnableFishInteraction(false);
        _currentSpot = null;
        yield return new WaitForSeconds(endDelay);
        OnMinigameFinished?.Invoke();
    }
}

public interface IMinigame
{
    event Action OnMinigameFinished;
    void MinigameStarted();
    void EndMinigame();
    
    public MinigameTools Tool { get; }
}

public enum MinigameTools
{
    Spear
}
