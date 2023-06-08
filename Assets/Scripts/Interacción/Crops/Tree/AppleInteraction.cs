using System.Collections;
using UnityEngine;
using cakeslice;
using System.Collections.Generic;

public class AppleInteraction : FruitsInteraction
{

    public AppleTreeGrowing appleTree;

    public override void Awake()
    {
        base.Awake();

        appleTree = GetComponent<AppleTreeGrowing>();
    }
    private void Start()
    {
        already = appleTree._alreadyRe;
        ReGrow = appleTree.ReGrow;
        ReGrowMaxTime = appleTree.ReGrowTimes;
        DiaM = appleTree.DiaM;
        yaEligioCh = appleTree.yaeligioCh;
    }
    public override List<GameObject> Fruits()
    {
        return appleTree.fruits;
    }

    public override GameObject GetGameObject()
    {
        return appleTree.gameObject;
    }

    public override IEnumerator Enumerator()
    {
        return appleTree.BushCedeLaPresidencia();
    }

}
