using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class FishingSpot : MonoBehaviour, IInteractable
{
    [SerializeField] private FishTarget fish;
    [SerializeField] private FishingInteractCollider interactCol;
    [SerializeField] private InteractionPromptUI prompt;
    [SerializeField] private Collider mainCollider;

    private FishItemData _fishData;
    private int _misses;
    
    private FishSpawner _spawner;
    public Transform InteractionTarget => transform;
    public InteractionPromptUI InteractionPrompt => prompt;

    public FishItemData FishData => _fishData;
    public event Action OnFishingFinished;

    private Spear _playerSpear;

    private void Start()
    {
        fish.Setup(this);
        interactCol.Setup(this);
    }

    public void Setup(FishItemData fishData, FishSpawner spawner)
    {
        _fishData = fishData;
        this._spawner = spawner;
    }

    public void Interact(InteractorBase interactor, out bool interactSuccessful)
    {
        // Get player Spear/Minigame Tool
        _playerSpear = interactor.GetComponentInParent<PlayerInventoryHolder>()?.GetSpear();

        if (_playerSpear)
        {
            MinigameManager.instance.StartFishingMinigame(this);
            _playerSpear.OnMiss += MissedFish;
            interactSuccessful = true;
        }
        else
        {
            Debug.LogError("Spear not found, unable to start fishing minigame");
            interactSuccessful = false;
        }
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
        interactCol.gameObject.SetActive(enable);
        EnableMainCollider(!enable);
    }

    public void CaughtFish()
    {
        // TODO: Dar FishDataSO como Item o como sea al player, mejorar transiciones de camara, efectitos y etc (?
        
        //minigame.EndMinigame();
        _playerSpear.OnMiss -= MissedFish;
        _spawner.FreeSpot(this);
        Debug.Log("Caught fish");
        PlayerInventoryHolder.instance?.AddToInventory(_fishData, 1);
        OnFishingFinished?.Invoke();
        Destroy(gameObject);
    }
    public void MissedFish()
    {
        Debug.Log("Missed fish");
        _misses++;

        if (_misses > 2)
        {
            _playerSpear.OnMiss -= MissedFish;
            OnFishingFinished?.Invoke();
            _spawner.FreeSpot(this);
            Destroy(gameObject);
        }
    }
    
    public void EnableMainCollider(bool enable) => mainCollider.enabled = enable;
}
