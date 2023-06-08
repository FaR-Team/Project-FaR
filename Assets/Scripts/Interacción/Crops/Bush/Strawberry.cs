using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class Strawberry : FruitsInteraction
{
    public BushGrowing bushGrowing;

    public override void Awake()
    {
        base.Awake();

        bushGrowing = GetComponent<BushGrowing>();
       
    }
    private void Start()
    {
        already = bushGrowing._alreadyRe;
        ReGrow = bushGrowing.ReGrow;
        ReGrowMaxTime = bushGrowing.ReGrowTimes;
        DiaM = bushGrowing.DiaM;
        yaEligioCh = bushGrowing.yaeligioCh;
    }

    public override List<GameObject> Fruits()
    {
        return bushGrowing.fruits;
    }

    public override GameObject GetGameObject()
    {
        return bushGrowing.gameObject;
    }

    public override IEnumerator Enumerator()
    {
        return bushGrowing.BushCedeLaPresidencia();
    }
}
