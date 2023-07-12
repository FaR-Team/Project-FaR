using System.Collections;
using UnityEngine;
using FaRUtils;
using System.Collections.Generic;

public class AppleInteraction : CropInteraction
{

    public AppleTreeGrowing appleTree;

    public override void Awake()
    {
        base.Awake();

        appleTree = GetComponent<AppleTreeGrowing>();
    }
    public override List<GameObject> Fruits()
    {
        return appleTree.fruits;
    }
}
