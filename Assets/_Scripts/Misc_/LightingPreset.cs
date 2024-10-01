using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Lighting Preset", menuName = "Jueguito Granjil/LightingPreset", order = 1)]
public class LightingPreset : ScriptableObject
{
    public Gradient ColorAmbiente;
    public Gradient ColorDireccional;
    public Gradient ColorNiebla;
}
