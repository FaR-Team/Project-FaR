using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractorBase : MonoBehaviour
{
    public event Action<Vector3> OnRaycastHit;
    public event Action<Vector3> OnInteractTry;

    protected bool isInteractorAnimating;
    protected bool _canInteract;

    protected Vector3 _currentHitPosition;

    protected virtual void RaycastHitEvent(Vector3 position)
    {
        _currentHitPosition = position;
        OnRaycastHit?.Invoke(position);
    }

    protected virtual void InteractTryEvent()
    {
        OnInteractTry?.Invoke(_currentHitPosition);
    }

    public void SetInteractorAnimating(bool animating)
    {
        isInteractorAnimating = animating;
    }
}
