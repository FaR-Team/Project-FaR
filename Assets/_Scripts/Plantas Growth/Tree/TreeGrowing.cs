using System.Collections;
using UnityEngine;
using Utils;

public class TreeGrowing : GrowingTreeAndPlant //Crecimiento del árbol
{
    protected override void Start()
    {
        base.Start();
        CheckDayGrow();
    }
    
    public void StartReGrowTree()
    {
        ResetSpawnPoints();
        daysWithoutHarvest = 0;
        _reGrowCounter++;
        fruits.Clear();
        gameObject.layer = 3;
    }

    public override IEnumerator BushCedeLaPresidencia()
    {
        yield return new WaitForSeconds(0.5f);
    }

}
