using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class FishingSpot : MonoBehaviour, IInteractable
{
    [SerializeField] private FishTarget fish;
    [SerializeField] private FishingMissCollider missCol;
    [SerializeField] private InteractionPromptUI prompt;
    [SerializeField] private Collider mainCollider;

    private FishDataSO _fishData;
    private int _misses;
    
    private FishSpawner _spawner;
    public Transform InteractionTarget => transform;
    public InteractionPromptUI InteractionPrompt => prompt;

    public event Action OnFishingFinished;
    //public event Action<Transform> 

    private void Start()
    {
        fish.Setup(this);
        missCol.Setup(this);
    }

    public void Setup(FishDataSO fishData, FishSpawner spawner)
    {
        _fishData = fishData;
        this._spawner = spawner;
    }

    public void Interact(InteractorBase interactor, out bool interactSuccessful)
    {
        MinigameManager.instance.StartFishingMinigame(this);
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
        
        //minigame.EndMinigame();
        _spawner.FreeSpot(this);
        Debug.Log("Caught fish");
        OnFishingFinished?.Invoke();
        Destroy(gameObject);
    }
    public void MissedFish()
    {
        Debug.Log("Missed fish");
        _misses++;

        if (_misses > 2)
        {
            OnFishingFinished?.Invoke();
            _spawner.FreeSpot(this);
            Destroy(gameObject);
        }
    }
    
    public void EnableMainCollider(bool enable) => mainCollider.enabled = enable;
}
