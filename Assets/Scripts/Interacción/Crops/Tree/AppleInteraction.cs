using System.Collections;
using UnityEngine;
using FaRUtils;
using System.Collections.Generic;
using System.Linq;

public class AppleInteraction : CropInteraction
{

    public TreeGrowing appleTree;
    [SerializeField]
    private Animator treeAnimator;

    public override void Awake()
    {
        base.Awake();

        appleTree = GetComponent<TreeGrowing>();
    }

    public override IEnumerator Wait()
    {
        treeAnimator.SetTrigger("Harvest");
        yield return new WaitForSeconds(0.5f);

        foreach (var fruit in appleTree.fruits)
        {
            fruit.GetComponent<FallingFruit>().FallFruit();
        }
        
        appleTree.StartReGrowTree();

        already = false;
    }
}
