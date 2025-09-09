using UnityEngine;
public class PhysicsObjectProperties : MonoBehaviour
{
    [Header("Propiedades del Centro de Masa")]
    [SerializeField, Tooltip("Centro de masa local personalizado")]
    private Vector3 _customCenterOfMass = Vector3.zero;
    
    [SerializeField, Tooltip("Usar centro de masa personalizado")]
    private bool _useCustomCenterOfMass = false;
    
    [Header("Propiedades de Masa")]
    [SerializeField, Tooltip("Masa del objeto en kg")]
    private float _mass = 1f;
    
    [SerializeField, Tooltip("Densidad del material (usado para calcular masa automáticamente)")]
    private float _density = 1f;
    
    [SerializeField, Tooltip("Usar cálculo automático de masa basado en densidad")]
    private bool _useAutomaticMass = false;

    [Header("Propiedades de Interacción")]
    [SerializeField, Tooltip("Factor de dificultad adicional para levantar este objeto")]
    [Range(0.1f, 5f)]
    private float _liftDifficultyMultiplier = 1f;
    
    [SerializeField, Tooltip("Resistencia al viento/movimiento")]
    [Range(0f, 10f)]
    private float _dragCoefficient = 0.5f;

    private Rigidbody _rigidbody;
    private Vector3 _originalCenterOfMass;
    private float _originalMass;
    private float _originalDrag;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.LogError($"PhysicsObjectProperties requiere un Rigidbody en {gameObject.name}", this);
            return;
        }

        // Guardar valores originales
        _originalCenterOfMass = _rigidbody.centerOfMass;
        _originalMass = _rigidbody.mass;
        _originalDrag = _rigidbody.drag;
    }

    private void Start()
    {
        ApplyPhysicsProperties();
    }

    private void OnValidate()
    {
        if (Application.isPlaying && _rigidbody != null)
        {
            ApplyPhysicsProperties();
        }
    }

    /// <summary>
    /// Aplica las propiedades físicas configuradas al rigidbody
    /// </summary>
    private void ApplyPhysicsProperties()
    {
        if (_rigidbody == null) return;

        // Configurar centro de masa
        if (_useCustomCenterOfMass)
        {
            _rigidbody.centerOfMass = _customCenterOfMass;
        }
        else
        {
            _rigidbody.centerOfMass = _originalCenterOfMass;
        }

        // Configurar masa
        if (_useAutomaticMass)
        {
            CalculateAutomaticMass();
        }
        else
        {
            _rigidbody.mass = _mass;
        }

        // Configurar resistencia al aire
        _rigidbody.drag = _dragCoefficient;
    }

    /// <summary>
    /// Calcula la masa automáticamente basada en la densidad y el volumen aproximado
    /// </summary>
    private void CalculateAutomaticMass()
    {
        if (_rigidbody == null) return;

        // Obtener el volumen aproximado del objeto usando su collider
        float volume = GetApproximateVolume();
        _rigidbody.mass = volume * _density;
    }

    /// <summary>
    /// Obtiene el volumen aproximado del objeto basado en sus colliders
    /// </summary>
    private float GetApproximateVolume()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        float totalVolume = 0f;

        foreach (Collider col in colliders)
        {
            if (col.isTrigger) continue;

            if (col is BoxCollider box)
            {
                Vector3 size = box.size;
                totalVolume += size.x * size.y * size.z;
            }
            else if (col is SphereCollider sphere)
            {
                float radius = sphere.radius;
                totalVolume += (4f / 3f) * Mathf.PI * radius * radius * radius;
            }
            else if (col is CapsuleCollider capsule)
            {
                float radius = capsule.radius;
                float height = capsule.height;
                float cylinderVolume = Mathf.PI * radius * radius * (height - 2 * radius);
                float sphereVolume = (4f / 3f) * Mathf.PI * radius * radius * radius;
                totalVolume += cylinderVolume + sphereVolume;
            }
            else
            {
                // Para otros tipos de colliders, usar bounds como aproximación
                Bounds bounds = col.bounds;
                totalVolume += bounds.size.x * bounds.size.y * bounds.size.z;
            }
        }

        return Mathf.Max(totalVolume, 0.1f); // Mínimo volumen para evitar división por cero
    }

    /// <summary>
    /// Obtiene el factor de dificultad para levantar este objeto
    /// </summary>
    public float GetLiftDifficultyMultiplier()
    {
        return _liftDifficultyMultiplier;
    }

    /// <summary>
    /// Resetea las propiedades físicas a sus valores originales
    /// </summary>
    public void ResetToOriginalProperties()
    {
        if (_rigidbody == null) return;

        _rigidbody.centerOfMass = _originalCenterOfMass;
        _rigidbody.mass = _originalMass;
        _rigidbody.drag = _originalDrag;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_rigidbody == null) return;

        // Dibujar centro de masa
        Gizmos.color = _useCustomCenterOfMass ? Color.red : Color.green;
        Vector3 worldCenterOfMass = transform.TransformPoint(_rigidbody.centerOfMass);
        Gizmos.DrawWireSphere(worldCenterOfMass, 0.1f);

        // Dibujar información de masa
        UnityEditor.Handles.Label(worldCenterOfMass + Vector3.up * 0.5f, 
            $"Masa: {_rigidbody.mass:F2}kg\nDificultad: {_liftDifficultyMultiplier:F1}x");
    }

    private void OnDrawGizmosSelected()
    {
        if (_rigidbody == null) return;

        // Dibujar centro de masa más prominente cuando está seleccionado
        Gizmos.color = Color.yellow;
        Vector3 worldCenterOfMass = transform.TransformPoint(_rigidbody.centerOfMass);
        Gizmos.DrawSphere(worldCenterOfMass, 0.2f);

        // Dibujar ejes desde el centro de masa
        Gizmos.color = Color.red;
        Gizmos.DrawRay(worldCenterOfMass, transform.right * 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(worldCenterOfMass, transform.up * 0.5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(worldCenterOfMass, transform.forward * 0.5f);
    }
#endif
}
