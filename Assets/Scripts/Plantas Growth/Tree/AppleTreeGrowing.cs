using UnityEngine;
using Random = UnityEngine.Random;

public class AppleTreeGrowing : GrowingTreeAndPlant
{
    public float[] scales;
    int DiaM;
    public override void Start()
    {
        base.Start();

        meshFilter.mesh = meshes[0];
        meshCollider.sharedMesh = meshes[0];

        SetThisGameObjectScale(0);

        ExpectedInt = 3;
    }

    public override void OnHourChanged(int hour)
    {
        if (hour != 5) return;
        
        if (DiasPlantado < 2) DiasPlantado++;

        CheckDayGrow();

        if (DiasPlantado is 2)
        {
            DiaM += 1;
            if (DiaM == ExpectedInt)
            {
                this.gameObject.layer = 7;
            }
        }
    }

    public override void CheckDayGrow()
    {
        SetThisGameObjectScale(DiasPlantado);
    }

    private void SetThisGameObjectScale(int scaleValue)
    {
        transform.localScale = new Vector3(scales[scaleValue], scales[scaleValue], scales[scaleValue]);
    }
}
