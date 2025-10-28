using UnityEngine;

[RequireComponent(typeof(ContainerPhysics))]
public class SimpleContainer : MonoBehaviour
{
    [Header("Configuración del Contenedor")]
    [SerializeField, Tooltip("Color del contenido")]
    private Color _contentColor = Color.yellow;
    
    [SerializeField, Tooltip("Renderer del contenido interno")]
    private Renderer _contentRenderer;
    
    [Header("Efectos de Derrame")]
    [SerializeField, Tooltip("Sistema de partículas para el derrame")]
    private ParticleSystem _spillParticleSystem;
    
    [SerializeField, Tooltip("Prefab simple para el derrame (fallback)")]
    private GameObject _spillPrefab;
    
    [Header("Configuración de Partículas")]
    [SerializeField, Tooltip("Intensidad base de emisión de partículas")]
    private float _baseEmissionRate = 10f;
    
    [SerializeField, Tooltip("Multiplicador de intensidad por cantidad derramada")]
    private float _emissionIntensityMultiplier = 50f;
    
    [SerializeField, Tooltip("Duración del efecto de derrame")]
    private float _spillEffectDuration = 0.5f;

    private ContainerPhysics _containerPhysics;
    private MaterialPropertyBlock _materialPropertyBlock;

    private void Awake()
    {
        _containerPhysics = GetComponent<ContainerPhysics>();
        
        if (_contentRenderer != null)
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
        }
    }

    private void Start()
    {
        _containerPhysics.OnContentChanged += OnContentChanged;
        _containerPhysics.OnContentSpilled += OnContentSpilled;
        _containerPhysics.OnContainerEmpty += OnContainerEmpty;
        
        SetupParticleSystem();
        UpdateVisualContent();
    }

    private void OnDestroy()
    {
        if (_containerPhysics != null)
        {
            _containerPhysics.OnContentChanged -= OnContentChanged;
            _containerPhysics.OnContentSpilled -= OnContentSpilled;
            _containerPhysics.OnContainerEmpty -= OnContainerEmpty;
        }
    }

    private void OnContentChanged(float newPercentage)
    {
        UpdateVisualContent();
    }

    private void OnContentSpilled(Vector3 spillPosition, float amount, Vector3 direction)
    {
        CreateSpillEffect(spillPosition, amount, direction);
    }

    private void OnContainerEmpty()
    {
        Debug.Log($"Contenedor {gameObject.name} completamente vacío!");
        
        if (_contentRenderer != null)
        {
            _contentRenderer.enabled = false;
        }
    }

    private void CreateSpillEffect(Vector3 position, float amount, Vector3 direction)
    {
        if (_spillParticleSystem != null)
        {
            CreateParticleSpillEffect(position, amount, direction);
        }
        else if (_spillPrefab != null)
        {
            GameObject spillEffect = Instantiate(_spillPrefab, position, Quaternion.identity);
            Destroy(spillEffect, 5f);
        }
        else
        {
            CreateSimpleSpillEffect(position);
        }
    }

    private void CreateParticleSpillEffect(Vector3 position, float amount, Vector3 direction)
    {
        if (_spillParticleSystem == null) return;

        _spillParticleSystem.transform.position = position;
        
        if (direction != Vector3.zero)
        {
            _spillParticleSystem.transform.rotation = Quaternion.LookRotation(direction);
        }
        else
        {
            _spillParticleSystem.transform.rotation = Quaternion.LookRotation(Vector3.down);
        }

        var main = _spillParticleSystem.main;
        main.startColor = _contentColor;
        main.startSpeed = Mathf.Lerp(1f, 3f, amount);

        var emission = _spillParticleSystem.emission;
        emission.rateOverTime = 0f;
        
        var shape = _spillParticleSystem.shape;
        shape.angle = Mathf.Lerp(5f, 25f, amount);
        
        var velocityOverLifetime = _spillParticleSystem.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
        
        Vector3 spillVelocity = direction * Mathf.Lerp(0.5f, 2f, amount);
        
        float minX = spillVelocity.x * 0.3f;
        float maxX = spillVelocity.x * 1.2f;
        float minY = spillVelocity.y * 0.3f;
        float maxY = spillVelocity.y * 1.2f;
        float minZ = spillVelocity.z * 0.3f;
        float maxZ = spillVelocity.z * 1.2f;
        
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(minX, maxX);
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(minY, maxY);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(minZ, maxZ);
        
        int particleCount = Mathf.RoundToInt(_baseEmissionRate + (amount * _emissionIntensityMultiplier));
        _spillParticleSystem.Emit(particleCount);
        
        if (!_spillParticleSystem.isPlaying)
        {
            _spillParticleSystem.Play();
        }
    }

    private System.Collections.IEnumerator StopSpillEffectAfterDelay()
    {
        yield return new WaitForSeconds(_spillEffectDuration);
        
        if (_spillParticleSystem != null && _spillParticleSystem.isPlaying)
        {
            _spillParticleSystem.Stop();
        }
    }

    private void CreateSimpleSpillEffect(Vector3 position)
    {
        GameObject spillObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        spillObj.name = "SimpleSpill";
        spillObj.transform.position = position;
        spillObj.transform.localScale = Vector3.one * 0.2f;
        
        Renderer renderer = spillObj.GetComponent<Renderer>();
        renderer.material.color = _contentColor;
        
        Destroy(spillObj, 10f);
    }

    private void SetupParticleSystem()
    {
        if (_spillParticleSystem == null) return;

        var main = _spillParticleSystem.main;
        main.startLifetime = 3f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 3f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.15f);
        main.startColor = _contentColor;
        main.maxParticles = 1000;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = 1.2f;

        var emission = _spillParticleSystem.emission;
        emission.enabled = true;
        emission.rateOverTime = 0f;

        var shape = _spillParticleSystem.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 25f;
        shape.radius = 0.05f;
        shape.length = 0.3f;

        var velocityOverLifetime = _spillParticleSystem.velocityOverLifetime;
        velocityOverLifetime.enabled = false;
        
        var forceOverLifetime = _spillParticleSystem.forceOverLifetime;
        forceOverLifetime.enabled = true;
        forceOverLifetime.y = -9.81f;
        
        var limitVelocityOverLifetime = _spillParticleSystem.limitVelocityOverLifetime;
        limitVelocityOverLifetime.enabled = true;
        limitVelocityOverLifetime.limit = 2f;
        limitVelocityOverLifetime.dampen = 0.95f;
        limitVelocityOverLifetime.space = ParticleSystemSimulationSpace.World;
        
        var inheritVelocity = _spillParticleSystem.inheritVelocity;
        inheritVelocity.enabled = true;
        inheritVelocity.mode = ParticleSystemInheritVelocityMode.Initial;
        inheritVelocity.curve = 0.3f;

        var collision = _spillParticleSystem.collision;
        collision.enabled = true;
        collision.type = ParticleSystemCollisionType.World;
        collision.mode = ParticleSystemCollisionMode.Collision3D;

        collision.dampen = 0.8f;
        collision.bounce = 0.02f;
        collision.lifetimeLoss = 0.2f;
        collision.minKillSpeed = 0.1f;

        collision.enableDynamicColliders = true;
        collision.radiusScale = 1.2f;

        _spillParticleSystem.Stop();
    }

    private void UpdateVisualContent()
    {
        if (_contentRenderer == null) return;
        
        _contentRenderer.enabled = !_containerPhysics.IsEmpty;
        
        if (_contentRenderer.enabled)
        {
            if (_materialPropertyBlock != null)
            {
                _materialPropertyBlock.SetColor("_Color", _contentColor);
                _materialPropertyBlock.SetFloat("_FillLevel", _containerPhysics.ContentPercentage);
                _contentRenderer.SetPropertyBlock(_materialPropertyBlock);
            }
            
            Vector3 scale = Vector3.one;
            scale.y = _containerPhysics.ContentPercentage;
            _contentRenderer.transform.localScale = scale;
        }
    }

    public void AddContent(float amount)
    {
        _containerPhysics.AddContent(amount);
    }

    public bool RemoveContent(float amount)
    {
        return _containerPhysics.RemoveContent(amount);
    }

    public float GetContentPercentage()
    {
        return _containerPhysics.ContentPercentage;
    }

    public bool IsEmpty()
    {
        return _containerPhysics.IsEmpty;
    }

    [ContextMenu("Create Particle System")]
    public void CreateParticleSystem()
    {
        if (_spillParticleSystem == null)
        {
            GameObject particleObject = new GameObject("SpillParticleSystem");
            particleObject.transform.SetParent(transform);
            particleObject.transform.localPosition = Vector3.zero;
            
            _spillParticleSystem = particleObject.AddComponent<ParticleSystem>();
            SetupParticleSystem();
            
            Debug.Log("Sistema de partículas creado automáticamente para el contenedor.");
        }
    }

    [ContextMenu("Setup Emission Source")]
    public void SetupEmissionSource()
    {
        _containerPhysics.AutoFindEmissionSource();
        
        BoxCollider emissionSource = _containerPhysics.GetEmissionSource();
        if (emissionSource != null)
        {
            Debug.Log($"Emission source configurado: {emissionSource.name}");
        }
        else
        {
            Debug.LogWarning("No se pudo configurar emission source. Asegúrate de tener un BoxCollider.");
        }
    }

    [ContextMenu("Reset Particle System")]
    public void ResetParticleSystem()
    {
        if (_spillParticleSystem != null)
        {
            _spillParticleSystem.Stop();
            _spillParticleSystem.Clear();
            SetupParticleSystem();
            Debug.Log("Sistema de partículas reiniciado.");
        }
    }

    [ContextMenu("Test Heavy Liquid (Oil/Honey)")]
    public void ConfigureHeavyLiquid()
    {
        if (_spillParticleSystem == null) return;
        
        var collision = _spillParticleSystem.collision;
        collision.dampen = 0.95f;
        collision.bounce = 0.01f;
        
        var limitVel = _spillParticleSystem.limitVelocityOverLifetime;
        limitVel.limit = 1f;
        limitVel.dampen = 0.98f;
        
        Debug.Log("Configurado para líquido espeso (aceite/miel)");
    }

    [ContextMenu("Test Light Liquid (Water)")]  
    public void ConfigureLightLiquid()
    {
        if (_spillParticleSystem == null) return;
        
        var collision = _spillParticleSystem.collision;
        collision.dampen = 0.6f;
        collision.bounce = 0.05f;
        
        var limitVel = _spillParticleSystem.limitVelocityOverLifetime;
        limitVel.limit = 3f;
        limitVel.dampen = 0.8f;
        
        Debug.Log("Configurado para líquido ligero (agua)");
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying && _containerPhysics != null)
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, 
                $"Contenido: {(_containerPhysics.ContentPercentage * 100):F0}%");
        }
    }
#endif
}