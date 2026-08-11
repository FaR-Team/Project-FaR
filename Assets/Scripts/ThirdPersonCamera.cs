using System;
using System.Collections;
using System.Collections.Generic;
using FaRUtils.FPSController;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public bool lookingAtTarget = false;
    public Transform lookTarget;
    public float lookAtSpeed = 5f;
    
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private FaRCharacterController _controller;
    private ThirdPersonInteractor _interactor;

    private void Awake()
    {
        _controller = GetComponentInParent<FaRCharacterController>();
        _interactor = GetComponentInChildren<ThirdPersonInteractor>(true);
    }

    void Update()
    {
        if (lookingAtTarget && lookTarget)
        {
            Vector3 dir = lookTarget.position - transform.position;
            dir.Normalize();
            
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 targetEuler = lookRotation.eulerAngles;

            float newPitch = Mathf.LerpAngle(pitch, targetEuler.x, Time.deltaTime * lookAtSpeed);
            float newYaw = Mathf.LerpAngle(yaw, targetEuler.y, Time.deltaTime * lookAtSpeed);
            
            SetRotation(newPitch, newYaw);
        }
    }

    public void ActivateCamera(Transform target)
    {
        // Update with FPS camera rotation
        SetRotation(_controller.FPSCamera.transform.eulerAngles);
        SetCameraTarget(target);
        _interactor.gameObject.SetActive(true);
    }
    
    public void DeactivateCamera()
    {
        SetCameraTarget(null);
        _interactor.gameObject.SetActive(false);
    }

    public void SetRotation(Vector3 eulerAngles)
    {
        pitch = eulerAngles.x;
        yaw = eulerAngles.y;
        transform.eulerAngles = eulerAngles;
    }
    public void SetRotation(float pitch, float yaw)
    {
        this.pitch = pitch;
        this.yaw = yaw;
        transform.eulerAngles = new Vector3(pitch, yaw, 0);
    }
    public void SetCameraTarget(Transform target)
    {
        lookingAtTarget = target != null;
        lookTarget = target;
    }

    public void EnableCameraInteractor(bool enable)
    {
        _interactor.SetCanInteract(enable);
    }

    private void OnDisable()
    {
        transform.localRotation = Quaternion.identity;
    }
}
