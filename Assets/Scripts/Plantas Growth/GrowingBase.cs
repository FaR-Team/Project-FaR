using UnityEngine;
using System.Linq;
using FaRUtils.Systems.DateTime;

public class GrowingBase : MonoBehaviour
{
    protected int interactableLayerInt = 7;

    public int DiasPlantado; //Dias que pasaron desde que se plantó.

    public bool isFruit;

    [SerializeField] protected GrowingState[] states;
    public GrowingState currentState;

    [HideInInspector] public MeshFilter meshFilter;
    [HideInInspector] public MeshCollider meshCollider;
    [HideInInspector] public MeshRenderer meshRenderer;

    public virtual void Start()
    {
        if (TryGetComponent<MeshFilter>(out MeshFilter filter))
        {
            meshFilter = filter;
        }
        if (TryGetComponent<MeshCollider>(out MeshCollider col))
        {
            meshCollider = col;
        }
        if (TryGetComponent<MeshRenderer>(out MeshRenderer renderer))
        {
            meshRenderer = renderer;
        }

        DateTime.OnHourChanged.AddListener(OnHourChanged);
    }

    public virtual void OnHourChanged(int hour) { }

    public virtual void CheckDayGrow() //SE FIJA LOS DIAS DEL CRECIMIENTO.
    {
        currentState = states.FirstOrDefault<GrowingState>(state => state.IsThisState(DiasPlantado));
        SetData();
    }

    protected virtual void SetData()
    {
        meshFilter.mesh = currentState.mesh;
        meshRenderer.material = currentState.material;
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = currentState.mesh;
        }
        if (currentState.isLastPhase) SetInteractable();
    }

    public virtual void SetInteractable()
    {
        if (isFruit) return;

        gameObject.layer = interactableLayerInt; //layer interactuable.
    }

    public void LoadData(CropSaveData cropSaveData)
    {
        DiasPlantado = cropSaveData.DiasPlantado;
        currentState = cropSaveData.GrowingState;
        SetData();
    }
}
