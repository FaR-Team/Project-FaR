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

    private void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        GameInput.playerInputActions.Player.PrimaryUse.performed += TryInteract;
        UIController.instance.EnableCrosshairMovement(true);
    }

    private void OnDisable()
    {
        GameInput.playerInputActions.Player.PrimaryUse.performed -= TryInteract;
        UIController.instance.EnableCrosshairMovement(false);
    }

    private void TryInteract(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit, 5f, interactableLayer);

        if (hit.collider != null && hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
        {
            interactable.Interact(this, out bool interacted);
        }
    }
}
