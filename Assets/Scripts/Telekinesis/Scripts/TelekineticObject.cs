using UnityEngine;

public class TelekineticObject : MonoBehaviour
{
    [Header("Propiedades Físicas")]
    [SerializeField] private bool isRotable = true;
    [SerializeField] private bool useCustomCenterOfMass = false;
    [SerializeField] private Vector3 customCenterOfMass = Vector3.zero;
    [SerializeField] private float stabilityFactor = 1f;
    [SerializeField] private float telekineticResistance = 1f;
    
    private Rigidbody _rigidbody;
    private Vector3 _originalCenterOfMass;
    private RigidbodyConstraints _originalConstraints;
    private float _originalDrag;
    private float _originalAngularDrag;
    private bool _wasKinematic;
    private RigidbodyInterpolation _originalInterpolation;
    private MaterialPropertyBlock _propertyBlock;
    private Renderer[] _renderers;
    
    public Vector3 GrabPoint { get; private set; }
    public Vector3 GrabNormal { get; private set; }
    public float Mass => _rigidbody.mass;
    public Rigidbody Rigidbody => _rigidbody;
    public bool IsRotable => isRotable;
    
    public void SetRotable(bool rotable)
    {
        isRotable = rotable;
    }
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.LogError($"TelekineticObject requiere un Rigidbody en {gameObject.name}");
            return;
        }
        
        _originalCenterOfMass = _rigidbody.centerOfMass;
        _originalConstraints = _rigidbody.constraints;
        _originalDrag = _rigidbody.drag;
        _originalAngularDrag = _rigidbody.angularDrag;
        _wasKinematic = _rigidbody.isKinematic;
        _originalInterpolation = _rigidbody.interpolation;
        
        if (useCustomCenterOfMass)
        {
            _rigidbody.centerOfMass = customCenterOfMass;
        }
        
        _propertyBlock = new MaterialPropertyBlock();
        _renderers = GetComponentsInChildren<Renderer>();
    }
    
    public virtual void Initialize(RaycastHit hit)
    {
        GrabPoint = hit.point;
        GrabNormal = hit.normal;
        
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
        
        _rigidbody.drag = Mathf.Max(_originalDrag, 0.5f);
        _rigidbody.angularDrag = Mathf.Max(_originalAngularDrag, 1f);
        
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        
        _rigidbody.freezeRotation = false;
        
        if (GetComponent<AvoidCollisionWPlayer>() == null)
        {
            gameObject.AddComponent<AvoidCollisionWPlayer>();
        }
    }

    public virtual void Cleanup()
    {
        _rigidbody.centerOfMass = _originalCenterOfMass;
        _rigidbody.constraints = _originalConstraints;
        _rigidbody.drag = _originalDrag;
        _rigidbody.angularDrag = _originalAngularDrag;
        _rigidbody.isKinematic = _wasKinematic;
        _rigidbody.interpolation = _originalInterpolation;
        
        var collisionAvoid = GetComponent<AvoidCollisionWPlayer>();
        if (collisionAvoid != null)
        {
            Destroy(collisionAvoid);
        }
        
        _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, 20f);
        _rigidbody.angularVelocity = Vector3.ClampMagnitude(_rigidbody.angularVelocity, 10f);
    }

    public virtual void SetOutlineActive(bool active, Material outlineMaterial = null)
    {
        if (_renderers == null || _renderers.Length == 0)
        {
            _renderers = GetComponentsInChildren<Renderer>();
        }
        if (_propertyBlock == null)
        {
            _propertyBlock = new MaterialPropertyBlock();
        }
        
        if (_renderers == null) return;
        
        foreach (var renderer in _renderers)
        {
            if (renderer == null) continue;
            
            renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat("_UseOutline", active ? 1f : 0f);
            if (!active)
            {
                _propertyBlock.SetFloat("_OutlineExpand", 0f);
                _propertyBlock.SetFloat("_OutlineExplode", 0f);
            }
            renderer.SetPropertyBlock(_propertyBlock);
        }
    }

    public virtual void SetOutlineExplode(float explode)
    {
        if (_renderers == null || _renderers.Length == 0)
        {
            _renderers = GetComponentsInChildren<Renderer>();
        }
        if (_propertyBlock == null)
        {
            _propertyBlock = new MaterialPropertyBlock();
        }

        if (_renderers == null) return;

        foreach (var renderer in _renderers)
        {
            if (renderer == null) continue;

            renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat("_OutlineExplode", explode);
            renderer.SetPropertyBlock(_propertyBlock);
        }
    }

    public virtual void SetOutlineExpand(float expand)
    {
        if (_renderers == null || _renderers.Length == 0)
        {
            _renderers = GetComponentsInChildren<Renderer>();
        }
        if (_propertyBlock == null)
        {
            _propertyBlock = new MaterialPropertyBlock();
        }

        if (_renderers == null) return;

        foreach (var renderer in _renderers)
        {
            if (renderer == null) continue;

            renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat("_OutlineExpand", expand);
            renderer.SetPropertyBlock(_propertyBlock);
        }
    }
    
    public float GetTelekineticResistance()
    {
        return telekineticResistance;
    }
    
    public float GetStabilityFactor()
    {
        return stabilityFactor;
    }
    
    public Vector3 GetMassDistribution()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        if (colliders.Length == 0) return Vector3.one;
        
        Vector3 totalSize = Vector3.zero;
        int validColliders = 0;
        
        foreach (var col in colliders)
        {
            if (col.isTrigger) continue;
            
            Vector3 size = col.bounds.size;
            totalSize += size;
            validColliders++;
        }
        
        if (validColliders > 0)
        {
            totalSize /= validColliders;
        }
        
        return totalSize.magnitude > 0 ? totalSize : Vector3.one;
    }
    public void ApplyStabilizingForce(Vector3 desiredDirection, float strength)
    {
        Vector3 currentUp = transform.up;
        Vector3 desiredUp = Vector3.up;
        
        Vector3 torqueAxis = Vector3.Cross(currentUp, desiredUp);
        float torqueMagnitude = Vector3.Angle(currentUp, desiredUp) * Mathf.Deg2Rad;
        
        Vector3 stabilizingTorque = torqueAxis * torqueMagnitude * strength * stabilityFactor;
        _rigidbody.AddTorque(stabilizingTorque, ForceMode.Force);
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_rigidbody == null) return;
        
        Gizmos.color = useCustomCenterOfMass ? Color.red : Color.green;
        Vector3 worldCenterOfMass = transform.TransformPoint(_rigidbody.centerOfMass);
        Gizmos.DrawWireSphere(worldCenterOfMass, 0.1f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(worldCenterOfMass, transform.right * 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(worldCenterOfMass, transform.up * 0.5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(worldCenterOfMass, transform.forward * 0.5f);
        
        if (Application.isPlaying)
        {
            UnityEditor.Handles.Label(worldCenterOfMass + Vector3.up * 0.5f, 
                $"Masa: {Mass:F1}kg\nResistencia: {telekineticResistance:F1}\nEstabilidad: {stabilityFactor:F1}");
        }
    }
    
    private void OnValidate()
    {
        if (_rigidbody != null && useCustomCenterOfMass)
        {
            _rigidbody.centerOfMass = customCenterOfMass;
        }
    }
#endif
}
