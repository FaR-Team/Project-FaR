using UnityEngine;

public class HorizontalBillboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            Vector3 targetPosition = mainCamera.transform.position;
            targetPosition.y = transform.position.y;

            transform.LookAt(targetPosition);
        }
    }
}