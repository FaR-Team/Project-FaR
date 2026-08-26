using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationOnInteract : MonoBehaviour
{
    [SerializeField] private InteractorBase interactor;
    [SerializeField] private bool disableInteractorWhileAnimating;
    
    private Animator _animator;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable() => interactor.OnInteractTry += InteractTryHandler;
    private void OnDisable() => interactor.OnInteractTry -= InteractTryHandler;

    public void AnimationStarted()
    {
        if (disableInteractorWhileAnimating) interactor.SetInteractorAnimating(true);
    }
    
    public void AnimationFinished()
    {
        if (disableInteractorWhileAnimating) interactor.SetInteractorAnimating(false);
    }

    private void InteractTryHandler(Vector3 position)
    {
        _animator.SetTrigger("Interact");
    }
}
