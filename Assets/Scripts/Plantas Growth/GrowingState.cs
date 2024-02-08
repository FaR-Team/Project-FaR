using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Jueguito Granjil/Growing State")]
[Serializable]
public class GrowingState : ScriptableObject
{
    public Mesh mesh;
    public Material material;
    [Range(0, 20)]
    public int minimalDay, maximalDay;
    public bool isLastPhase;

    public bool IsThisState(int day)
    {
        if(isLastPhase)
        {
            return day >= minimalDay;
        }
        return day >= minimalDay && day <= maximalDay;
    }
}