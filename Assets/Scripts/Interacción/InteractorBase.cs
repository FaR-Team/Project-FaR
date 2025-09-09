using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractorBase : MonoBehaviour
{
    public event Action<Vector3> OnRaycastHit;
    public event Action OnInteractTry;

    protected bool isInteractorAnimating;
    protected bool _canInteract;

    protected virtual void RaycastHitEvent(Vector3 vector3)
    {
        OnRaycastHit?.Invoke(vector3);
    }

    protected virtual void InteractTryEvent()
    {
        OnInteractTry?.Invoke();
    }

    public void SetInteractorAnimating(bool animating)
    {
        isInteractorAnimating = animating;
    }
}
