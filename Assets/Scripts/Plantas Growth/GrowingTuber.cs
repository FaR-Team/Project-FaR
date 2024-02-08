using System.Linq;
using UnityEngine;

[RequireComponent(typeof(UniqueID))]
public class GrowingTuber : GrowingBase { 

    private string id;
    public Dirt tierra;
    public GameObject interactablePrefab;
    public SkinnedMeshRenderer skinnedMeshRenderer ;

    void Awake()
    {
        tierra = transform.parent.GetComponent<Dirt>();
    }

    public override void Start()
    {
        base.Start();

        id = GetComponent<UniqueID>().ID;
    }
    public override void OnHourChanged(int hour)
    {
        if (!tierra._isWet || hour != 4) return;
        
        DiasPlantado++; 
        CheckDayGrow();
    }
    public override void CheckDayGrow() //SE FIJA LOS DIAS DEL CRECIMIENTO.
    {
        currentState = states.FirstOrDefault<GrowingState>(state => state.IsThisState(DiasPlantado));
        SetData();
    }
    protected override void SetData()
    {
        skinnedMeshRenderer.material = currentState.material;
        skinnedMeshRenderer.sharedMesh = currentState.mesh;

        if (currentState.isLastPhase) SetInteractable();
    }
    public override void SetInteractable()
    {
        Instantiate(interactablePrefab, transform.position, Quaternion.identity, transform);
        skinnedMeshRenderer.sharedMesh = null;
        skinnedMeshRenderer.material = null;
    }
}