using System.Linq;
using UnityEngine;

[RequireComponent(typeof(UniqueID))]
public class GrowingTuber : GrowingBase
{

    private string id;
    public Dirt tierra;
    public GameObject interactablePrefab;
    public SkinnedMeshRenderer skinnedMeshRenderer;

    protected override void Awake()
    {
        base.Awake();
        tierra = transform.parent.GetComponent<Dirt>();
    }

    protected override void Start()
    {
        base.Start();

        id = GetComponent<UniqueID>().ID;
    }
    public override void OnHourChanged(int hour)
    {
        if (!tierra._isWet || hour != 4) return;

        daysPlanted++;
        CheckDayGrow();
    }
    public override void CheckDayGrow() //SE FIJA LOS DIAS DEL CRECIMIENTO.
    {
        GrowingState lastState = currentState;
        currentState = states.FirstOrDefault<GrowingState>(state => state.IsThisState(daysPlanted));

        if (currentState != lastState) UpdateState(); // Only change mesh data if changed state
    }
    protected override void UpdateState()
    {
        Debug.Log("UPDATING STATE");
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