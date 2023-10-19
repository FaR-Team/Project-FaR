using UnityEngine;

[RequireComponent(typeof(UniqueID))]
public class GrowingCrop : GrowingBase
{
    private string id;
    public Dirt tierra;

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
    public override void CheckDayGrow()
    {
        foreach (int i in DayForChangeOfPhase)
        {
            if (DiasPlantado != i) continue;

            SetInteractable(i);

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
}
