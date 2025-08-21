using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public bool lookingAtTarget = false;
    public Transform lookTarget;
    public float lookAtSpeed = 5f;
    
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    void Start()
    {
        
    }
    
    void Update()
    {
        if (lookingAtTarget)
        {
            Vector3 dir = lookTarget.position - transform.position;
            dir.Normalize();
            
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 targetEuler = lookRotation.eulerAngles;

            yaw = Mathf.LerpAngle(yaw, targetEuler.y, Time.deltaTime * lookAtSpeed);
            pitch = Mathf.LerpAngle(pitch, targetEuler.x, Time.deltaTime * lookAtSpeed);
            
            transform.localEulerAngles = new Vector3(pitch, yaw, 0);
            transform.localEulerAngles = new Vector3(pitch, 0, 0);
        }
    }
    
    public void SetCameraTarget(Transform target)
    {
        lookingAtTarget = target != null;
        lookTarget = target;
    }

    private void OnDisable()
    {
        transform.localRotation = Quaternion.identity;
    }
}
