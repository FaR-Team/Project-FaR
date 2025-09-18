using UnityEngine;

[ExecuteAlways]
public class FogWallManager : MonoBehaviour
{
    [Tooltip("Up to 4 fog wall Transforms (use Cube scaled as needed). Leave empty entries null.")]
    public Transform[] fogWalls = new Transform[4];

    [Tooltip("Fog color")]
    public Color fogColor = Color.white;

    [Range(0f, 1f)] public float fadeStart = 0f; // normalized 0..1
    [Range(0f, 1f)] public float fadeEnd = 1f;   // normalized 0..1

    [Tooltip("If true, update matrices every frame. Set false if all fog walls are static.")]
    public bool updateEveryFrame = true;

    void OnEnable()
    {
        UpdateGlobalFog();
    }

    void Update()
    {
        if (updateEveryFrame)
            UpdateGlobalFog();
    }

    void UpdateGlobalFog()
    {
        var mats = new Matrix4x4[4];
        for (int i = 0; i < 4; i++)
        {
            if (i < fogWalls.Length && fogWalls[i] != null)
                mats[i] = fogWalls[i].worldToLocalMatrix;
            else
                mats[i] = Matrix4x4.identity;
        }

        Shader.SetGlobalMatrixArray("_FogWorldToLocal", mats);
        Shader.SetGlobalColor("_FogColor", fogColor);
        Shader.SetGlobalFloat("_FadeStart", fadeStart);
        Shader.SetGlobalFloat("_FadeEnd", fadeEnd);
    }

    // If walls are static and you don't want Update calls:
    [ContextMenu("Bake Fog Globals (Set once)")]
    public void BakeOnce()
    {
        bool prev = updateEveryFrame;
        updateEveryFrame = false;
        UpdateGlobalFog();
        updateEveryFrame = prev;
    }
}
