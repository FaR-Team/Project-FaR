using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class ForceUpdateSkinnedMesh : MonoBehaviour
{
    [SerializeField]
    SkinnedMeshRenderer skinnedMesh;

    void Update()
    {
        skinnedMesh.enabled = false;
    }

    void OnRenderObject()
    {
        skinnedMesh.enabled = true;
    }
}