using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StrawberryBushInteraction : CropInteraction
{
    public BushGrowing bushGrowing;

    public override void Awake()
    {
        base.Awake();

        bushGrowing = GetComponent<BushGrowing>();
       
    }

    public override IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (GameObject fruit in Fruits())
        {
            //GetComponent<CropExplodeBush>().Chau(fruit);
            fruit.GetComponent<FallingFruit>().FallFruit();
        }
        
        bushGrowing.StartReGrowBush();

        already = false;
    }

    public override List<GameObject> Fruits()
    {
        return bushGrowing.fruits.Select(f => f.gameObject).ToList();
    }
}
