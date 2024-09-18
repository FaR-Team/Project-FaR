using UnityEngine;

[RequireComponent(typeof(UniqueID))]
public class GrowingCrop : GrowingBase
{
    private string id;
    public Dirt tierra;

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
}
