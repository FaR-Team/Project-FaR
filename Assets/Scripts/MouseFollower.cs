using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float zOffset = 2;
    private Quaternion _initialRotation;
    private void Awake()
    {
        _initialRotation = transform.rotation;
    }
    
    private void OnDisable()
    {
        transform.rotation = _initialRotation;
    }

    private void Update()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = zOffset;
        transform.right = (cam.ScreenToWorldPoint(mousePos) - transform.position).normalized;
    }
}