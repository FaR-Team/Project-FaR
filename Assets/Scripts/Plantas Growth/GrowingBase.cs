using UnityEngine;
using System;

public class GrowingBase : MonoBehaviour
{
    public Mesh[] meshes;
    public Material[] materials;

    public int Dia; //Dias que pasaron desde que se plantó.

    [HideInInspector] public bool yacrecio;

    //[SerializeField] private CropSaveData cropSaveData;
    //private string id;

    public int[] DayForChangeOfPhase;

    [HideInInspector] public MeshFilter meshFilter;
    [HideInInspector] public MeshCollider meshCollider;
    [HideInInspector] public MeshRenderer meshRenderer;

    public virtual void Start()
    {
        if (TryGetComponent<MeshFilter>(out MeshFilter filter))
        {
            meshFilter = filter;
            meshFilter.mesh = meshes[0];
        }
        if (TryGetComponent<MeshCollider>(out MeshCollider col))
        {
            meshCollider = col;
            meshCollider.sharedMesh = meshes[0];
        }
        if (TryGetComponent<MeshRenderer>(out MeshRenderer renderer))
        {
            meshRenderer = renderer;
            meshRenderer.material = materials[0];
        }

        Dia = 0;
        yacrecio = false;
    }

    public virtual void CheckDayGrow()
    {
        if (!yacrecio) return;

        foreach (int i in DayForChangeOfPhase)
        { 
            if (Dia != i) continue;

            if (IsLastPhase(i))
            {
                gameObject.layer = 7;
            }
            int valueToGet = Array.IndexOf(DayForChangeOfPhase, i);
            if (meshCollider != null)
            {
                meshCollider.sharedMesh = meshes[valueToGet];
            }
            meshFilter.mesh = meshes[valueToGet];
            meshRenderer.material = materials[valueToGet];
            return;
        }
    }
    bool IsLastPhase(int numero)
    {
        if (DayForChangeOfPhase.Length == 0)
        {
            return false;
        }

        int ultimoElemento = DayForChangeOfPhase[DayForChangeOfPhase.Length - 1];
        return numero == ultimoElemento;
    }
}