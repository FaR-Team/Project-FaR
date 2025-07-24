using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingSpot : MonoBehaviour, IInteractable
{
    [SerializeField] FishingMinigame minigame;
    [SerializeField] private FishTarget fish;
    [SerializeField] private FishingMissCollider missCol;
    [SerializeField] private InteractionPromptUI prompt;

    private int _misses;
    public Transform InteractionTarget => transform;

    public InteractionPromptUI InteractionPrompt => prompt;

    private void Start()
    {
        fish.Setup(this);
        missCol.Setup(this);
    }

    public void Interact(InteractorBase interactor, out bool interactSuccessful)
    {
        minigame.StartMinigame(this);
        interactSuccessful = true;
    }

    public void InteractOut()
    {
        throw new System.NotImplementedException();
    }

    public void EndInteraction()
    {
        
    }

    public void EnableFishInteraction(bool enable)
    {
        fish.EnableInteraction(enable);
        missCol.gameObject.SetActive(enable);
    }

    public void CaughtFish()
    {
        transform.localScale = new Vector3(3, 3, 3);
        minigame.EndMinigame();
    }
    public void MissedFish()
    {
        _misses++;

        if (_misses > 2)
        {
            minigame.EndMinigame();
            Destroy(gameObject);
        }
    }
}
