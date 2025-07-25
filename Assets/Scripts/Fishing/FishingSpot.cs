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
    [SerializeField] private Collider mainCollider;

    private FishDataSO _fishData;
    private int _misses;
    public Transform InteractionTarget => transform;

    public InteractionPromptUI InteractionPrompt => prompt;

    private void Start()
    {
        fish.Setup(this);
        missCol.Setup(this);
    }

    public void Setup(FishDataSO fishData)
    {
        _fishData = fishData;
    }

    public void Interact(InteractorBase interactor, out bool interactSuccessful)
    {
        minigame.StartMinigame(this);
        interactSuccessful = true;
    }

    public void InteractOut()
    {
        //throw new System.NotImplementedException();
    }

    public void EndInteraction()
    {
        if (prompt != null)
        {
            prompt.Close();
        }
    }

    public void EnableFishInteraction(bool enable)
    {
        fish.EnableInteraction(enable);
        missCol.gameObject.SetActive(enable);
        EnableMainCollider(!enable);
    }

    public void CaughtFish()
    {
        // TODO: Dar FishDataSO como Item o como sea al player, mejorar transiciones de camara, efectitos y etc (?
        
        Destroy(gameObject);
        minigame.EndMinigame();
        Debug.Log("Caught fish");
    }
    public void MissedFish()
    {
        Debug.Log("Missed fish");
        _misses++;

        if (_misses > 2)
        {
            minigame.EndMinigame();
            Destroy(gameObject);
        }
    }
    
    public void EnableMainCollider(bool enable) => mainCollider.enabled = enable;
}
