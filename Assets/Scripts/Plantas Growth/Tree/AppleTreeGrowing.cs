using UnityEngine;
using Random = UnityEngine.Random;

public class AppleTreeGrowing : GrowingTreeAndPlant
{
    public float[] scales;
    public override void Start()
    {
        base.Start();
                
        meshFilter.mesh = meshes[0];
        meshCollider.sharedMesh = meshes[0];

        SetThisGameObjectScale(0);

        ExpectedInt = 3;
    }

    private void SetThisGameObjectScale(int scaleValue)
    {
        transform.localScale = new Vector3(scales[scaleValue], scales[scaleValue], scales[scaleValue]);
    }
    public override void Update()
    {
        if (ClockManager.TimeText() == "05:00 AM" && yacrecio == false)
        {
            if (Dia < 2)
            {
                Dia++;
            }

            yacrecio = true;
            CheckDayGrow();

            if (yaeligioCh == true)
            {
                yaeligio = false;
                yaeligioCh = false;
                _alreadyRe = false;
            }

            if (Dia is 2)
            {
                DiaM += 1;
                if (DiaM == ExpectedInt)
                {
                    this.gameObject.layer = 7;
                }
            }
        }

        if (ClockManager.TimeText() == "06:00 AM" && yacrecio == true)
        {
            yacrecio = false;
        }

        if (Dia is 2 && yaeligio == false)
        {
            PonerFruto();
        }
    }
    public void PonerFruto()
    {
        if (yaeligio != false || ReGrow == ReGrowTimes) return;

        RandInt = Random.Range(10, 15);

        for (int i = 0; i < RandInt; i++)
        {
            Transform Spawn = GetRandomSP();
            GameObject fruit = Instantiate(Prefab, Spawn.position, Spawn.rotation, Spawn);

            fruits.Add(fruit.transform.GetChild(2).gameObject);
        }
        DiaM = 1;
        yaeligio = true;
        ReGrow++;
    }

    public override void CheckDayGrow()
    {
        if (!yacrecio) return;

        SetThisGameObjectScale(Dia);
    }
}
