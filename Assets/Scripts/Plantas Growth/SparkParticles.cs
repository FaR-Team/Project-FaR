using UnityEngine;
using System.Collections;

public class SparkParticles : MonoBehaviour
{
    public Transform BillboardTarget;
    ParticleSystem _particleSystem;
    ParticleSystem.Particle[] _particles = new ParticleSystem.Particle[25];

    void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();

        if (BillboardTarget == null)
            BillboardTarget = Camera.main.transform;
    }

    void FixParticleRotations()
    {
        int count = _particleSystem.GetParticles(_particles);

        for (int i = 0; i < count; i++)
        {
            Vector3 dir = _particles[i].velocity.normalized;
            float rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            _particles[i].rotation = rot;
        }

        _particleSystem.SetParticles(_particles, _particleSystem.particleCount);
    }

    public void Play()
    {
        transform.LookAt(BillboardTarget);
        _particleSystem.Play();
        StartCoroutine(LateFix());
    }

    IEnumerator LateFix()
    {
        yield return null;
        FixParticleRotations();
    }
}