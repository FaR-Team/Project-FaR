using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemDebugger : MonoBehaviour
{
    [Tooltip("Si está activado, escribe info cada frame en la consola (puede ser verboso).")]
    public bool logEveryFrame = false;

    [Tooltip("Número máximo de partículas a muestrear para el cálculo promedio.")]
    public int sampleLimit = 50;

    ParticleSystem _ps;

    void Awake()
    {
        _ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (_ps == null) return;

        var main = _ps.main;

        float gravityModifier = 1f;
        bool gravityIsConstant = main.gravityModifier.mode == ParticleSystemCurveMode.Constant;
        if (gravityIsConstant)
        {
            gravityModifier = main.gravityModifier.constant;
        }

        ParticleSystem.Particle[] arr = new ParticleSystem.Particle[_ps.main.maxParticles];
        int c = _ps.GetParticles(arr);
        int sampleCount = Mathf.Min(c, sampleLimit);

        Vector3 avgVel = Vector3.zero;
        for (int i = 0; i < sampleCount; i++)
        {
            avgVel += arr[i].velocity;
        }
        if (sampleCount > 0) avgVel /= sampleCount;

        Vector3 expectedGravityPerSec = Physics.gravity * gravityModifier;

        if (logEveryFrame)
        {
            Debug.Log($"[PS Debug] particles={c}, sample={sampleCount}, avgVel=({avgVel.x:F2},{avgVel.y:F2},{avgVel.z:F2}), gravityModifierMode={(main.gravityModifier.mode)}, gravityModifier={(gravityIsConstant? gravityModifier.ToString("F2") : "(non-const) ")}, expectedGravityPerSec=({expectedGravityPerSec.x:F2},{expectedGravityPerSec.y:F2},{expectedGravityPerSec.z:F2})");
        }
    }

    [ContextMenu("Log ParticleSystem Quick Info")]
    public void LogQuickInfo()
    {
        if (_ps == null) _ps = GetComponent<ParticleSystem>();

        var main = _ps.main;
        string simSpace = main.simulationSpace.ToString();
        string scaling = main.scalingMode.ToString();
        bool gravityIsConstant = main.gravityModifier.mode == ParticleSystemCurveMode.Constant;
        float gravityModifier = gravityIsConstant ? main.gravityModifier.constant : float.NaN;

        Debug.Log($"[PS Info] name={_ps.gameObject.name}, simulationSpace={simSpace}, scalingMode={scaling}, startSpeed={main.startSpeed.constant}, gravityModifierMode={main.gravityModifier.mode}, gravityModifier={(gravityIsConstant? gravityModifier.ToString("F2") : "(non-const)")}, maxParticles={main.maxParticles}");
    }

    [ContextMenu("Run One Sample Log")]
    public void RunOneSampleLog()
    {
        if (_ps == null) _ps = GetComponent<ParticleSystem>();

        ParticleSystem.Particle[] arr = new ParticleSystem.Particle[_ps.main.maxParticles];
        int c = _ps.GetParticles(arr);
        int sampleCount = Mathf.Min(c, sampleLimit);

        Vector3 avgVel = Vector3.zero;
        for (int i = 0; i < sampleCount; i++) avgVel += arr[i].velocity;
        if (sampleCount > 0) avgVel /= sampleCount;

        Debug.Log($"[PS Sample] particles={c}, sample={sampleCount}, avgVel=({avgVel.x:F2},{avgVel.y:F2},{avgVel.z:F2})");
    }
}
