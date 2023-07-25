using UnityEngine;
using FaRUtils.Systems.DateTime;
using System.Linq;

public class GrowingBase : MonoBehaviour
{
    
    [Tooltip("acá poné las meshes")]
    public Mesh[] meshes; 
    public Material[] materials;

    protected int interactableLayerInt = 7;

    public int DiasPlantado; //Dias que pasaron desde que se plantó.

    //[SerializeField] private CropSaveData cropSaveData;
    //private string id;

    public int[] DayForChangeOfPhase;

    public bool isFruit;

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

        DateTime.OnHourChanged.AddListener(OnHourChanged);

        DiasPlantado = 0;
    }
    
    public virtual void OnHourChanged(int hour) {}

    public virtual void CheckDayGrow() //SE FIJA LOS DIAS DEL CRECIMIENTO.
    {
        foreach (int i in DayForChangeOfPhase)
        { 
            if (DiasPlantado != i) continue;

            int valueToGet = System.Array.IndexOf(DayForChangeOfPhase, i);
            if (meshCollider != null)
            {
                meshCollider.sharedMesh = meshes[valueToGet];
            }
            meshFilter.mesh = meshes[valueToGet];
            meshRenderer.material = materials[valueToGet];
            return;
        }
    }

    public void SetInteractable(int i)
    {
        if (IsLastPhase(i) && !isFruit)
        {
            gameObject.layer = interactableLayerInt; //layer interactuable.
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
    public bool IsLastStage()
    {
        return (DiasPlantado >= meshes.Length);
    }
}