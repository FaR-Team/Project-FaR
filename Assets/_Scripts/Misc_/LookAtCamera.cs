using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private enum Mode
    {
        LookAt,
        LookAtMouse,
        LookAtInverted,
        CameraForward,
        CameraForwardInverted,
    }

    [SerializeField] private Mode mode;

    private void LateUpdate()
    {
        switch (mode) {
            case Mode.LookAt:
                transform.LookAt(Camera.main.transform);
                break;
            case Mode.LookAtInverted:
                Vector3 dirFromCamera = transform.position - Camera.main.transform.position;
                    transform.LookAt(transform.position + dirFromCamera);
                break;
            case Mode.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;
            case Mode.CameraForwardInverted:
                transform.forward = -Camera.main.transform.forward;
                break;
            case Mode.LookAtMouse:
                Vector3 mouse = Input.mousePosition;
                Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(
                                                    mouse.x, 
                                                    mouse.y,
                                                    transform.position.y));
                Vector3 forward = mouseWorld - transform.position;
                transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
                break;
        }
    }
}
