using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{
    [SerializeField] private Transform catchPosition;
    [SerializeField] private Collider col;
    [SerializeField] private MouseFollower pivot;
    
    [Header("Interaction")]
    [SerializeField] private InteractorBase interactor;
    [SerializeField] private bool disableInteractorWhileAnimating;
    
    [Header("Animation")]
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float catchAnimationDuration = 0.5f;
    [SerializeField] private float activateColliderTreshold = .2f;
    [SerializeField] private float deactivateColliderTreshold = .79f;
    [SerializeField] private float progressCheckMissTreshold = .9f;
    
    private Vector3 _initialLocalPosition;
    private bool _animating;
    private float _animationTimer;
    private bool _caughtSomething;
    private bool _checkedMiss;
    private GameObject _caughtObject;
    private float _animStartXPosition;
    private float _animTargetXPosition;
    private float _curveValue;
    public event Action OnMiss;

    private void Awake()
    {
        _initialLocalPosition = transform.localPosition;
    }

    private void OnEnable()
    {
        interactor.OnInteractTry += InteractTryHandler;
        transform.localPosition = _initialLocalPosition;
    }

    private void OnDisable()
    {
        interactor.OnInteractTry -= InteractTryHandler;
        ResetSpear();
    }
    private void Update()
    {
        if (_animating)
        {
            _animationTimer += Time.deltaTime;

            float t = Mathf.Clamp01(_animationTimer / catchAnimationDuration);
            _curveValue = curve.Evaluate(t);
            //Debug.Log("LERPING, t = " + t);
            
            // If animation curve pass certain treshold and didn't catch anything, set as Missed
            if(!_checkedMiss && t > progressCheckMissTreshold) 
                CheckMissed();
            
            col.enabled =  t >= activateColliderTreshold && t <= deactivateColliderTreshold;

            Vector3 pos = transform.localPosition;
            pos.x = Mathf.Lerp(_animStartXPosition, _animTargetXPosition, _curveValue);
            transform.localPosition = pos;

            if (_animationTimer >= catchAnimationDuration)
            {
                // Set to last curve position just in case
                pos.x = Mathf.Lerp(_animStartXPosition, _animTargetXPosition, curve.Evaluate(1));
                FinishAnimation();
            }

        }
    }

    public void SetCatchVisual(GameObject fishObject)
    {
        _caughtObject = Instantiate(fishObject,  catchPosition.position, catchPosition.rotation, catchPosition);
        _caughtSomething = true;
    }

    public void CheckMissed()
    {
        _checkedMiss = true;
        
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
        _checkedMiss = false;
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
    
    private void InteractTryHandler(Vector3 hitPosition)
    {
        _animStartXPosition = transform.localPosition.x;
        
        Vector3 targetLocalPosition = transform.InverseTransformPoint(hitPosition);
        float distance = targetLocalPosition.x - catchPosition.localPosition.x;
        
        _animTargetXPosition = _animStartXPosition + distance;
        
        AnimateToPosition();
    }

    void AnimateToPosition()
    {
        _animating = true;
        _animationTimer = 0;
        
        if (disableInteractorWhileAnimating) interactor.SetInteractorAnimating(true);
    }

    void FinishAnimation()
    {
        _checkedMiss = false;
        _animating = false;
        _animationTimer = 0;
        if (disableInteractorWhileAnimating) interactor.SetInteractorAnimating(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 pos = _initialLocalPosition;
        Gizmos.DrawWireSphere(transform.parent.position + transform.parent.TransformPoint(pos), 0.2f);
        Gizmos.color = Color.blue;
        pos.x = _animTargetXPosition;
        Gizmos.DrawWireSphere(transform.parent.position + transform.parent.TransformPoint(pos), 0.2f);
    }
}
