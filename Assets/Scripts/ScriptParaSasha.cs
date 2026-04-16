using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptParaSasha : MonoBehaviour
{
    [ContextMenu("Boton magico? a tu responsabilidad")]
    public void Magia()
    {
        RoundRecursive(transform);
    }

    void RoundRecursive(Transform current)
    {
        var pos = current.localPosition;
        pos.x = RoundToOneDecimal(pos.x);
        pos.y = RoundToOneDecimal(pos.y);
        pos.z = RoundToOneDecimal(pos.z);
        current.localPosition = pos;

        for (int i = 0; i < current.childCount; i++)
        {
            RoundRecursive(current.GetChild(i));
        }
    }

    float RoundToOneDecimal(float value)
    {
        return (float)Math.Round(value, 1);
    }
}