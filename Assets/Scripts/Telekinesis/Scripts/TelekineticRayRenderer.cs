using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TelekineticRayRenderer : MonoBehaviour
{
    [Header("Configuración del Rayo")]
    [SerializeField] private Material rayMaterial;
    [SerializeField] private AnimationCurve rayWidthCurve = AnimationCurve.Linear(0f, 0.1f, 1f, 0.05f);
    [SerializeField] private float baseRayWidth = 0.1f;
    [SerializeField] private int raySegments = 50;
    [SerializeField] private bool useWorldSpace = true;
    [SerializeField] private LineTextureMode textureMode = LineTextureMode.Stretch;
    
    [Header("Colores del Rayo")]
    [SerializeField] private Color startColor = new Color(0.9f, 0.3f, 1f, 1f);
    [SerializeField] private Color endColor = new Color(1f, 0.4f, 0.8f, 0.9f);
    [SerializeField] private Color grabbingColor = new Color(0.8f, 0.1f, 1f, 1f);
    
    [Header("Efectos Dinámicos")]
    [SerializeField] private float noiseAmplitude = 0.03f;
    [SerializeField] private float animationSpeed = 4f;
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private bool enableElectricalEffect = true;
    
    [Header("Partículas del Rayo")]
    [SerializeField] private ParticleSystem rayParticles;
    [SerializeField] private int rayParticleCount = 40;
    [SerializeField] private float rayParticleSize = 0.015f;
    [SerializeField] private float rayParticleFloatSpeed = 1.5f;
    [SerializeField] private Color cubeStartColor = new Color(1f, 0.8f, 1f, 0.8f);
    [SerializeField] private Color cubeEndColor = new Color(0.8f, 0.3f, 1f, 0.3f);
    [SerializeField] private bool enableRayParticles = true;
    
    private LineRenderer lineRenderer;
    private TelekinesisController telekinesisController;
    private Vector3[] rayPoints;
    private float animationTime;
    private bool isActive = false;
    
    private ParticleSystem.Particle[] cubeParticles;
    private ParticleSystem.Particle[] rayParticlesArray;
    private float[] particleProgress;
    
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        telekinesisController = GetComponent<TelekinesisController>();
        
        SetupLineRenderer();
        rayPoints = new Vector3[raySegments];
        
        InitializeRayParticles();
    }
    
    private void SetupLineRenderer()
    {
        lineRenderer.material = rayMaterial;
        lineRenderer.positionCount = raySegments;
        lineRenderer.useWorldSpace = useWorldSpace;
        lineRenderer.startWidth = baseRayWidth;
        lineRenderer.endWidth = baseRayWidth * 0.5f;
        lineRenderer.widthCurve = rayWidthCurve;
        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;
        
        lineRenderer.textureMode = textureMode;
        lineRenderer.alignment = LineAlignment.View;
        lineRenderer.generateLightingData = false;
        lineRenderer.allowOcclusionWhenDynamic = false;
        
        if (rayMaterial != null)
        {
            lineRenderer.sharedMaterial = rayMaterial;
        }
        
        lineRenderer.enabled = false;
    }
    
    private void Update()
    {
        animationTime += Time.deltaTime * animationSpeed;
        
        if (telekinesisController == null)
            telekinesisController = GetComponent<TelekinesisController>();
            
        bool shouldShowRay = telekinesisController != null && (telekinesisController.HasGrabbedObject || telekinesisController.IsPerformingNoEnergyFeedback);
        
        if (shouldShowRay != isActive)
        {
            isActive = shouldShowRay;
            lineRenderer.enabled = isActive;
            
            if (rayParticles != null && enableRayParticles)
            {
                if (isActive) rayParticles.Play();
                else rayParticles.Stop();
            }
        }
        
        if (isActive)
        {
            UpdateRayVisuals();
            UpdateCubeParticles();
        }
    }
    
    private void UpdateRayVisuals()
    {
        Vector3 startPoint = telekinesisController.StartPoint;
        Vector3 midPoint = telekinesisController.MidPoint;
        Vector3 endPoint = telekinesisController.EndPoint;
        
        if (endPoint == Vector3.zero || midPoint == Vector3.zero)
        {
            lineRenderer.enabled = false;
            return;
        }
        
        lineRenderer.enabled = true;
        
        GenerateRayPoints(startPoint, midPoint, endPoint);
        
        if (enableElectricalEffect)
        {
            ApplyElectricalNoise();
        }
        
        lineRenderer.SetPositions(rayPoints);
        
        UpdateRayColors();
        
        UpdateRayWidth();
    }
    
    private void GenerateRayPoints(Vector3 start, Vector3 mid, Vector3 end)
    {
        for (int i = 0; i < raySegments; i++)
        {
            float t = (float)i / (raySegments - 1);
            
            Vector3 point = CalculateBezierPoint(t, start, mid, end);
            rayPoints[i] = point;
        }
        
        SmoothRayPoints();
    }
    
    private void SmoothRayPoints()
    {
        if (rayPoints.Length < 3) return;
        
        Vector3[] smoothedPoints = new Vector3[rayPoints.Length];
        smoothedPoints[0] = rayPoints[0];
        smoothedPoints[rayPoints.Length - 1] = rayPoints[rayPoints.Length - 1];
        
        for (int i = 1; i < rayPoints.Length - 1; i++)
        {
            Vector3 prev = rayPoints[i - 1];
            Vector3 current = rayPoints[i];
            Vector3 next = rayPoints[i + 1];
            
            smoothedPoints[i] = (prev * 0.2f + current * 0.6f + next * 0.2f);
        }
        
        for (int i = 1; i < rayPoints.Length - 1; i++)
        {
            rayPoints[i] = Vector3.Lerp(rayPoints[i], smoothedPoints[i], 0.5f);
        }
    }
    
    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        
        return uu * p0 + 2 * u * t * p1 + tt * p2;
    }
    
    private void ApplyElectricalNoise()
    {
        for (int i = 1; i < rayPoints.Length - 1; i++)
        {
            float t = (float)i / (rayPoints.Length - 1);
            
            float noiseX = Mathf.PerlinNoise(t * 2f + animationTime * 0.3f, animationTime * 0.2f) - 0.5f;
            float noiseY = Mathf.PerlinNoise(t * 2f + animationTime * 0.25f, animationTime * 0.3f + 100f) - 0.5f;
            float noiseZ = Mathf.PerlinNoise(t * 2f + animationTime * 0.2f, animationTime * 0.25f + 200f) - 0.5f;
            
            Vector3 noise = new Vector3(noiseX, noiseY, noiseZ) * noiseAmplitude;
            
            float waveEffect = Mathf.Sin(animationTime * 1.5f + t * Mathf.PI * 2f) * 0.005f;
            noise.y += waveEffect;
            
            float distanceFromCenter = Mathf.Abs(0.5f - t) * 2f;
            float noiseFactor = (1f - distanceFromCenter) * 0.7f; 

            float sparkleChance = Mathf.PerlinNoise(t * 3f + animationTime * 2f, animationTime * 1.8f + t);
            if (sparkleChance > 0.9f)
            {
                noise *= 1.2f;
            }
            
            rayPoints[i] += noise * noiseFactor;
        }
    }
    
    private void UpdateRayColors()
    {
        bool isGrabbing = telekinesisController.HasGrabbedObject;
        
        Color currentStartColor = isGrabbing ? grabbingColor : startColor;
        Color currentEndColor = isGrabbing ? grabbingColor * 0.8f : endColor;
        
        float pulse = (Mathf.Sin(animationTime * pulseSpeed) + 1f) * 0.5f;
        float sparkle = (Mathf.Sin(animationTime * pulseSpeed * 2.5f) + 1f) * 0.5f;
        float intensity = 0.8f + pulse * 0.4f + sparkle * 0.2f;

        float magicShimmer = (Mathf.Sin(animationTime * pulseSpeed * 1.7f) + 1f) * 0.5f;
        Color magicTint = new Color(1f, 0.7f, 1f, 1f);
        
        Color finalStartColor = Color.Lerp(currentStartColor, magicTint, magicShimmer * 0.3f);
        Color finalEndColor = Color.Lerp(currentEndColor, magicTint, magicShimmer * 0.2f);
        
        lineRenderer.startColor = finalStartColor * intensity;
        lineRenderer.endColor = finalEndColor * intensity;
    }
    
    private void UpdateRayWidth()
    {
        float pulse = (Mathf.Sin(animationTime * pulseSpeed * 1.5f) + 1f) * 0.5f;
        float magicPulse = (Mathf.Sin(animationTime * pulseSpeed * 2.3f) + 1f) * 0.5f;
        float widthMultiplier = 0.8f + pulse * 0.2f + magicPulse * 0.1f;
        
        float stabilityFactor = telekinesisController.CurrentStability;
        float instabilityWidth = 1f + (1f - stabilityFactor) * 0.3f;
        
        float breathe = (Mathf.Sin(animationTime * 0.8f) + 1f) * 0.5f;
        float breatheMultiplier = 0.95f + breathe * 0.1f;
        
        float finalStartWidth = baseRayWidth * widthMultiplier * instabilityWidth * breatheMultiplier;
        float finalEndWidth = baseRayWidth * 0.4f * widthMultiplier * breatheMultiplier;
        
        lineRenderer.startWidth = Mathf.Lerp(lineRenderer.startWidth, finalStartWidth, Time.deltaTime * 5f);
        lineRenderer.endWidth = Mathf.Lerp(lineRenderer.endWidth, finalEndWidth, Time.deltaTime * 5f);
    }

    private void InitializeRayParticles()
    {
        if (!enableRayParticles || rayParticles == null) return;
        rayParticlesArray = new ParticleSystem.Particle[rayParticleCount];
        particleProgress = new float[rayParticleCount];
        var main = rayParticles.main;
        main.startLifetime = 2f;
        main.startSpeed = 0f;
        main.startSize = rayParticleSize;
        main.maxParticles = rayParticleCount;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        var shape = rayParticles.shape;
        shape.enabled = false;
        var emission = rayParticles.emission;
        emission.enabled = false;
        var rotationOverLifetime = rayParticles.rotationOverLifetime;
        rotationOverLifetime.enabled = true;
        rotationOverLifetime.z = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
        var sizeOverLifetime = rayParticles.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 0.2f);
        sizeCurve.AddKey(0.1f, 1f);
        sizeCurve.AddKey(0.9f, 1f);
        sizeCurve.AddKey(1f, 0.2f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);
        for (int i = 0; i < rayParticleCount; i++)
        {
            particleProgress[i] = Random.Range(0f, 1f);
        }
    }
    
    private void UpdateCubeParticles()
    {
        if (!enableRayParticles || !isActive || rayParticles == null || rayPoints == null || rayPoints.Length < 2)
            return;

        if (!rayParticles.isPlaying)
            rayParticles.Play();

        if (rayParticlesArray == null || rayParticlesArray.Length != rayParticleCount)
            rayParticlesArray = new ParticleSystem.Particle[rayParticleCount];

        int visibleParticles = 0;
        for (int i = 0; i < rayParticleCount; i++)
        {
            float t = (float)i / (rayParticleCount - 1);
            Vector3 basePos = GetPositionAlongRay(t);
            float floatPhase = animationTime * rayParticleFloatSpeed + i * 0.5f;
            Vector3 floatOffset = new Vector3(
                Mathf.Sin(floatPhase + i) * 0.025f,
                Mathf.Cos(floatPhase * 1.2f + i * 0.7f) * 0.025f,
                Mathf.Sin(floatPhase * 0.7f + i * 1.3f) * 0.025f
            );
            rayParticlesArray[i].position = basePos + floatOffset;
            rayParticlesArray[i].rotation = floatPhase * 30f;
            rayParticlesArray[i].startSize = rayParticleSize;
            rayParticlesArray[i].remainingLifetime = 1f;
            rayParticlesArray[i].startLifetime = 1f;
            visibleParticles++;
        }
        rayParticles.SetParticles(rayParticlesArray, rayParticleCount);
    }
    
    private Vector3 GetPositionAlongRay(float t)
    {
        if (rayPoints == null || rayPoints.Length < 2) return Vector3.zero;
        
        t = Mathf.Clamp01(t);
        float exactIndex = t * (rayPoints.Length - 1);
        int index = Mathf.FloorToInt(exactIndex);
        float remainder = exactIndex - index;
        
        if (index >= rayPoints.Length - 1)
        {
            return rayPoints[rayPoints.Length - 1];
        }
        
        return Vector3.Lerp(rayPoints[index], rayPoints[index + 1], remainder);
    }
    
    public void SetRayActive(bool active)
    {
        isActive = active;
        lineRenderer.enabled = active;
        
        if (!active)
        {
            if (rayParticles != null) rayParticles.Stop();
        }
        else
        {
            if (rayParticles != null && enableRayParticles) rayParticles.Play();
        }
    }
    
    public void SetRayIntensity(float intensity)
    {
        intensity = Mathf.Clamp01(intensity);
        
        Color tempStart = lineRenderer.startColor;
        Color tempEnd = lineRenderer.endColor;
        
        tempStart.a = intensity;
        tempEnd.a = intensity * 0.8f;
        
        lineRenderer.startColor = tempStart;
        lineRenderer.endColor = tempEnd;
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();
        
        if (Application.isPlaying)
            SetupLineRenderer();
    }
#endif
}
