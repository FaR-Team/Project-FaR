using System.Collections;
using UnityEngine;

public class BushGrowing : GrowingTreeAndPlant //Crecimiento del arbusto
{
    public GameObject TierraTexture = null;

    protected override void Awake()
    {
        base.Awake();
        
        Tierra = transform.parent.gameObject.GetComponent<Dirt>();
        TierraTexture = transform.parent.GetChild(0).gameObject;
    }

    protected override void Start()
    {
        base.Start();

        CheckGrowState();
    }

    
    public void StartReGrowBush()
    {
        ResetSpawnPoints();
        daysWithoutHarvest = 0;
        _reGrowCounter++;
        fruits.Clear();
        gameObject.layer = 3;
        
        if(_reGrowCounter > ReGrowMaxTimes) Die();
    }

    public override IEnumerator BushCedeLaPresidencia()
    {
        TierraTexture.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
        DirtSpawnerPooling.DeSpawn(Tierra.gameObject);
    }
}