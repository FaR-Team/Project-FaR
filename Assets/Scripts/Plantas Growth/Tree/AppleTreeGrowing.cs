using UnityEngine;

public class AppleTreeGrowing : GrowingTreeAndPlant
{
    public float[] scales;
    int DiaMaduro;
    protected override void Start()
    {
        base.Start();

        

        ExpectedInt = 3;
    }

    public override void OnHourChanged(int hour)
    {
        if (hour != 5) return;
        
        if (daysPlanted < 2) daysPlanted++;

        CheckDayGrow();

        if (daysPlanted is 2)
        {
            DiaMaduro += 1;
            if (DiaMaduro == ExpectedInt)
            {
                this.gameObject.layer = 7;
            }
        }
    }    
}
