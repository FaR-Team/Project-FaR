using System;
using UnityEngine;
using FaRUtils.FPSController;

public class FPSControllerBridge : MonoBehaviour
{
    private FaRCharacterController _fpsController;

    private void Start()
    {
        _fpsController = FindObjectOfType<FaRCharacterController>();
        if(_fpsController == null)
        {
            Debug.LogError($"{nameof(FPSControllerBridge)} is missing {nameof(FaRCharacterController)}", this);
            return;
        }

        var gun = FindObjectOfType<PhysicsGunInteractionBehavior>();

        if (gun != null && _fpsController != null)
        {
            gun.OnRotation.AddListener(OnRotation);
        }
    }

    private void OnRotation(bool rotation)
    {
        //_fpsController.LockRotation = rotation;
    }
}
