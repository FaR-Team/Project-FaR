using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{
    [SerializeField] private Transform catchPosition;
    [SerializeField] private Collider col;
    [SerializeField] private MouseFollower pivot;
    private bool _caughtSomething;
    private GameObject _caughtObject;

    public event Action OnMiss;
    
    public void SetCatchVisual(GameObject fishObject)
    {
        _caughtObject = Instantiate(fishObject,  catchPosition.position, catchPosition.rotation, catchPosition);
        _caughtSomething = true;
    }

    public void CheckMissed()
    {
        if (!_caughtSomething)
        {
            // If didn't catch anything, call Missed 
            OnMiss?.Invoke();
        }
        
        _caughtSomething = false;
    }
    public void ResetSpear()
    {
        Destroy(_caughtObject);
        _caughtObject = null;
        _caughtSomething = false;
        EnablePivot(true);
    }

    public void EnableCollider() => col.enabled = true;

    public void DisableCollider()
    {
        //Debug.Break();
        col.enabled = false;
    }

    public void EnablePivot(bool enable)
    {
        pivot.EnableFollow(enable);
    }
    private void OnDisable()
    {
        ResetSpear();
    }
}
