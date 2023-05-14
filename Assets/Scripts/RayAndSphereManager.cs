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
    }//TODO: cambiar las OverlapSphere por este codigo.
   
}
