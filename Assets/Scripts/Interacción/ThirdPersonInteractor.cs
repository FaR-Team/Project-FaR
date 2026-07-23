using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class ThirdPersonInteractor : InteractorBase
{
    [SerializeField] private LayerMask interactableLayer;
    private Camera _cam;
    private CursorLockMode _previousLockMode;
    private IInteractable _interactable;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        _canInteract = true; // By default, when interacting, set CanInteract to true
        GameInput.playerInputActions.Player.PrimaryUse.performed += TryInteract;
        UIController.instance.EnableCrosshairMovement(true);
        _previousLockMode = Cursor.lockState;
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnDisable()
    {
        GameInput.playerInputActions.Player.PrimaryUse.performed -= TryInteract;
        UIController.instance.EnableCrosshairMovement(false);
        Cursor.lockState = _previousLockMode;
    }

    private void Update()
    {
        if (!_canInteract) return;
                
        Ray ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Physics.Raycast(ray, out RaycastHit hit, 8f, interactableLayer);

        if (hit.collider == null || !hit.collider.gameObject.TryGetComponent(out IInteractable interactable)) return;
            
        RaycastHitEvent(hit.point);
        _interactable = interactable != _interactable ? interactable : _interactable;
    }

    private void TryInteract(InputAction.CallbackContext ctx)
    {
        if (!_canInteract || !ctx.performed || _interactable == null || isInteractorAnimating) return;
        
        InteractTryEvent();
        _interactable.Interact(this, out bool interacted);
    }

    public void SetCanInteract(bool canInteract)
    {
        _canInteract = canInteract;
    }
}
