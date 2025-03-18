using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayAndSphereManager : MonoBehaviour
{
   public static RaycastHit hit;

    public static void DoRaycast(Ray ray, out RaycastHit hit, float maxDistance, LayerMask layerMask = default)
    {
        Physics.Raycast(ray, out hit, maxDistance, layerMask);
#if UNITY_EDITOR
        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.green, 0.01f);
#endif
    }


    public static Collider[] colliders;

    public static void DoOverlapSphere(Vector3 position, float radius, LayerMask layers)
    {
        colliders = Physics.OverlapSphere(position, radius, layers);
    }

    private static Collider[] m_NonAllocColliders;
    private static int m_LastMaxColliders = 0;

    public static int DoOverlapSphereNonAlloc(Vector3 position, float radius, int maxColliders, LayerMask layers)
    {
        if (m_NonAllocColliders == null || m_LastMaxColliders != maxColliders)
        {
            m_NonAllocColliders = new Collider[maxColliders];
            m_LastMaxColliders = maxColliders;
        }

        return Physics.OverlapSphereNonAlloc(position, radius, m_NonAllocColliders, layers);
    }

    public static Ray RayCameraScreenPoint()
    {
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }
   
}
