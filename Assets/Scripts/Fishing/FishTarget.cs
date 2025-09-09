using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishTarget : MonoBehaviour
{
    [SerializeField] Collider col;
    private FishingSpot _spot;

    public void Setup(FishingSpot spot)
    {
        _spot = spot;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something collided with fish");
        if (other.TryGetComponent(out Spear spear))
        {
            Debug.Log("Fish caught by spear");
            spear.SetCatchVisual(_spot.FishData.ItemPrefab);
            _spot.CaughtFish();
        }
    }

    public void EnableInteraction(bool enable)
    {
        col.enabled = enable;
    }
}
