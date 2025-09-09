using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TelekineticRayRenderer : MonoBehaviour
{
    [Header("Configuración del Rayo")]
    [SerializeField] private Material rayMaterial;
    [SerializeField] private AnimationCurve rayWidthCurve = AnimationCurve.Linear(0f, 0.1f, 1f, 0.05f);
    [SerializeField] private float baseRayWidth = 0.1f;
    [SerializeField] private int raySegments = 20;
    
    [Header("Colores del Rayo")]
    [SerializeField] private Color startColor = new Color(0.2f, 0.8f, 1f, 1f);
    [SerializeField] private Color endColor = new Color(0.8f, 0.2f, 1f, 0.8f);
    [SerializeField] private Color grabbingColor = new Color(1f, 0.4f, 0.2f, 1f);
    
    [Header("Efectos Dinámicos")]
    [SerializeField] private float noiseAmplitude = 0.02f;
    [SerializeField] private float noiseFrequency = 5f;
    [SerializeField] private float animationSpeed = 3f;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private bool enableElectricalEffect = true;
    
    [Header("Partículas")]
    [SerializeField] private ParticleSystem startParticles;
    [SerializeField] private ParticleSystem endParticles;
    [SerializeField] private ParticleSystem connectionParticles;
    
    private LineRenderer lineRenderer;
    private TelekinesisController telekinesisController;
    private Vector3[] rayPoints;
    private float animationTime;
    private bool isActive = false;
    
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        telekinesisController = GetComponent<TelekinesisController>();
        
        SetupLineRenderer();
        rayPoints = new Vector3[raySegments];
    }
    
    private void SetupLineRenderer()
    {
        lineRenderer.material = rayMaterial;
        lineRenderer.positionCount = raySegments;
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = baseRayWidth;
        lineRenderer.endWidth = baseRayWidth * 0.5f;
        lineRenderer.widthCurve = rayWidthCurve;
        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;
        
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.alignment = LineAlignment.View;
        
        lineRenderer.enabled = false;
    }
    
    private void Update()
    {
        animationTime += Time.deltaTime * animationSpeed;
        
        bool shouldShowRay = telekinesisController.HasGrabbedObject;
        
        if (shouldShowRay != isActive)
        {
            isActive = shouldShowRay;
            lineRenderer.enabled = isActive;
            
            if (startParticles != null)
            {
                if (isActive) startParticles.Play();
                else startParticles.Stop();
            }
            
            if (endParticles != null)
            {
                if (isActive) endParticles.Play();
                else endParticles.Stop();
            }
            
            if (connectionParticles != null)
            {
                if (isActive) connectionParticles.Play();
                else connectionParticles.Stop();
            }
        }
        
        if (isActive)
        {
            UpdateRayVisuals();
            UpdateParticlePositions();
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
            float noiseX = Mathf.PerlinNoise(animationTime + i * 0.1f, 0f) - 0.5f;
            float noiseY = Mathf.PerlinNoise(0f, animationTime + i * 0.1f) - 0.5f;
            float noiseZ = Mathf.PerlinNoise(animationTime * 0.7f, i * 0.1f) - 0.5f;
            
            Vector3 noise = new Vector3(noiseX, noiseY, noiseZ) * noiseAmplitude;
            
            float distanceFromCenter = Mathf.Abs(0.5f - (float)i / rayPoints.Length) * 2f;
            float noiseFactor = 1f - distanceFromCenter;
            
            rayPoints[i] += noise * noiseFactor;
        }
    }
    
    private void UpdateRayColors()
    {
        bool isGrabbing = telekinesisController.HasGrabbedObject;
        
        Color currentStartColor = isGrabbing ? grabbingColor : startColor;
        Color currentEndColor = isGrabbing ? grabbingColor * 0.7f : endColor;
        
        float pulse = (Mathf.Sin(animationTime * pulseSpeed) + 1f) * 0.5f;
        float intensity = 0.7f + pulse * 0.3f;
        
        lineRenderer.startColor = currentStartColor * intensity;
        lineRenderer.endColor = currentEndColor * intensity;
    }
    
    private void UpdateRayWidth()
    {
        float pulse = (Mathf.Sin(animationTime * pulseSpeed * 1.5f) + 1f) * 0.5f;
        float widthMultiplier = 0.8f + pulse * 0.2f;
        
        float stabilityFactor = telekinesisController.CurrentStability;
        float instabilityWidth = 1f + (1f - stabilityFactor) * 0.5f;
        
        lineRenderer.startWidth = baseRayWidth * widthMultiplier * instabilityWidth;
        lineRenderer.endWidth = baseRayWidth * 0.5f * widthMultiplier;
    }
    
    private void UpdateParticlePositions()
    {
        if (startParticles != null)
        {
            startParticles.transform.position = telekinesisController.StartPoint;
        }
        
        if (endParticles != null && telekinesisController.EndPoint != Vector3.zero)
        {
            endParticles.transform.position = telekinesisController.EndPoint;
        }
        
        if (connectionParticles != null && telekinesisController.MidPoint != Vector3.zero)
        {
            connectionParticles.transform.position = telekinesisController.MidPoint;
        }
    }
    
    public void SetRayActive(bool active)
    {
        isActive = active;
        lineRenderer.enabled = active;
        
        if (!active)
        {
            if (startParticles != null) startParticles.Stop();
            if (endParticles != null) endParticles.Stop();
            if (connectionParticles != null) connectionParticles.Stop();
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
