using System.Collections;
using UnityEngine;
using FaRUtils;
using System.Collections.Generic;
using System.Linq;

public class AppleInteraction : CropInteraction
{

    public TreeGrowing appleTree;

    public override void Awake()
    {
        base.Awake();

        appleTree = GetComponent<TreeGrowing>();
    }
    public override List<GameObject> Fruits()
    {
        return appleTree.fruits.Select(f => f.gameObject).ToList();
    }

    public override IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (GameObject fruit in Fruits())
        {
            //GetComponent<CropExplodeBush>().Chau(fruit);
            fruit.GetComponent<FallingFruit>().FallFruit();
        }
        
        appleTree.StartReGrowTree();

        DoEnumeratorIfMaxRegrows();

        already = false;
    }
}
