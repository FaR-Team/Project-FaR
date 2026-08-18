using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

[RequireComponent(typeof(Rigidbody))]
public class ContainerPhysics : MonoBehaviour
{
    [Header("Configuración del Contenedor")]
    [SerializeField, Tooltip("Cantidad actual de contenido (0-1)")]
    [Range(0f, 1f)]
    private float _currentContent = 1f;
    
    [SerializeField, Tooltip("Capacidad máxima del contenedor")]
    private float _maxCapacity = 100f;
    
    [SerializeField, Tooltip("BoxCollider que define la forma del contenedor para el derrame")]
    private BoxCollider _emissionSource;
    
    [Header("Límites de Estabilidad")]
    [SerializeField, Tooltip("Ángulo máximo de inclinación antes de derramar (grados)")]
    [Range(15f, 90f)]
    private float _maxTiltAngle = 45f;
    
    [SerializeField, Tooltip("Velocidad máxima antes de que el contenido se agite")]
    private float _maxVelocity = 9f;
    
    [SerializeField, Tooltip("Velocidad angular máxima antes de derramar")]
    private float _maxAngularVelocity = 6f;
    
    [Header("Pérdida de Contenido")]
    [SerializeField, Tooltip("Velocidad de pérdida por inclinación excesiva")]
    private float _spillRateByTilt = 0.5f;
    
    [SerializeField, Tooltip("Velocidad de pérdida por movimiento excesivo")]
    private float _spillRateByMovement = 0.3f;
    
    [SerializeField, Tooltip("Tiempo mínimo entre derrames")]
    private float _spillCooldown = 0.1f;

    [SerializeField, Tooltip("Puntos extras calculados en los edges superiores del box collider")]
    private int pointsPerEdge = 3;

    [Header("Ajustes de Gravedad (diagnóstico)")]
    [SerializeField, Tooltip("Multiplicador de gravedad aplicado al Rigidbody. 1 = gravedad del proyecto, >1 = más fuerte. 0 = desactivado.")]
    private float _gravityMultiplier = 1f;
    public float ContentPercentage => _currentContent;
    public float CurrentAmount => _currentContent * _maxCapacity;
    public bool IsEmpty => _currentContent <= 0.01f;
    public bool IsFull => _currentContent >= 0.99f;

    private Rigidbody _rigidbody;
    private float _lastSpillTime;
    
    private float _currentTiltAngle;
    private float _currentVelocityMagnitude;
    private float _currentAngularVelocityMagnitude;
    
    private bool _isUnstable;
    
    public event System.Action<float> OnContentChanged;
    public event System.Action<Vector3, float, Vector3> OnContentSpilled;
    public event System.Action OnContainerEmpty;

    private bool _enableAutoTilt = true;
    [SerializeField] private LayerMask _autoTiltLayer = 0;

    [SerializeField] private float _autoTiltRange = 2.5f;

    private float _autoTiltAngle = 75f;

    private float _autoTiltSpeed = 4f;

    private Vector3 _predestinatedPourRotation = new Vector3(75f, 0f, 0f);

    [SerializeField] private float _autoTiltHoldTime = 0.1f;

    private Quaternion _initialRotation;
    private Quaternion _targetRotation;
    private float _autoTiltTimer = 0f;
    private Dirt _lockedDirtTarget;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _initialRotation = transform.rotation;
        _targetRotation = _initialRotation;
    }

    private void FixedUpdate()
    {
        ApplyGravityMultiplierIfNeeded();
        CalculatePhysicsState();
        CheckForHoverAndAutoTilt();
        CheckStabilityConditions();
        
        if (_isUnstable && !IsEmpty)
        {
            ProcessContentLoss();
        }
    }

    private void ApplyGravityMultiplierIfNeeded()
    {
        if (Mathf.Approximately(_gravityMultiplier, 1f) || _rigidbody == null || !_rigidbody.useGravity) return;

        Vector3 extraGravity = Physics.gravity * (_gravityMultiplier - 1f) * _rigidbody.mass;
        _rigidbody.AddForce(extraGravity, ForceMode.Force);
    }

    private void CalculatePhysicsState()
    {
        Vector3 upDirection = transform.up;
        _currentTiltAngle = Vector3.Angle(upDirection, Vector3.up);
        
        _currentVelocityMagnitude = _rigidbody.velocity.magnitude;
        _currentAngularVelocityMagnitude = _rigidbody.angularVelocity.magnitude;
    }

    private void CheckStabilityConditions()
    {
        bool tiltExceeded = _currentTiltAngle > _maxTiltAngle;
        bool velocityExceeded = _currentVelocityMagnitude > _maxVelocity;
        bool angularVelocityExceeded = _currentAngularVelocityMagnitude > _maxAngularVelocity;

        _isUnstable = tiltExceeded || velocityExceeded || angularVelocityExceeded;
    }

    public bool IsGrabbed { get; set; }

    public bool IsBeingHeld()
    {
        if (IsGrabbed) return true;
        if (TelekinesisController.Instance != null && TelekinesisController.Instance.GetGrabbedRigidbody() == _rigidbody) return true;
        return false;
    }

    private bool _wasHeldLastFrame = false;

    private void CheckForHoverAndAutoTilt()
    {
        if (!_enableAutoTilt || IsEmpty) return;

        bool currentlyHeld = IsBeingHeld();

        if (!currentlyHeld)
        {
            _lockedDirtTarget = null;
            _wasHeldLastFrame = false;
            return;
        }

        if (!_wasHeldLastFrame)
        {
            _initialRotation = transform.rotation;
            _targetRotation = transform.rotation;
            _wasHeldLastFrame = true;
        }

        Vector3 center = _emissionSource != null ? _emissionSource.bounds.center : transform.position;

        if (_lockedDirtTarget != null)
        {
            if (!_lockedDirtTarget.gameObject.activeInHierarchy || Vector3.Distance(center, _lockedDirtTarget.transform.position) > _autoTiltRange + 0.6f)
            {
                _lockedDirtTarget = null;
            }
        }

        if (_lockedDirtTarget == null)
        {
            float closestDist = float.MaxValue;
            RaycastHit[] hits = Physics.SphereCastAll(center, 0.5f, Vector3.down, _autoTiltRange, ~0, QueryTriggerInteraction.Ignore);
            foreach (var hit in hits)
            {
                if (hit.collider.transform.IsChildOf(transform)) continue;

                Dirt dirt = hit.collider.GetComponentInParent<Dirt>();
                if (dirt != null)
                {
                    float dist = Vector3.Distance(center, dirt.transform.position);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        _lockedDirtTarget = dirt;
                    }
                }
            }
        }

        if (_lockedDirtTarget != null)
        {
            Quaternion predestinatedRotation = _initialRotation * Quaternion.Euler(_predestinatedPourRotation);

            _targetRotation = Quaternion.Slerp(_targetRotation, predestinatedRotation, Time.fixedDeltaTime * _autoTiltSpeed);
            _autoTiltTimer = _autoTiltHoldTime;
        }
        else
        {
            if (_autoTiltTimer > 0f)
            {
                _autoTiltTimer -= Time.fixedDeltaTime;
            }
            else
            {
                _targetRotation = Quaternion.Slerp(_targetRotation, _initialRotation, Time.fixedDeltaTime * _autoTiltSpeed);
            }
        }

        float step = Mathf.Clamp01(Time.fixedDeltaTime * _autoTiltSpeed * 1.5f);
        if (_rigidbody != null && !_rigidbody.isKinematic)
        {
            _rigidbody.MoveRotation(Quaternion.Slerp(_rigidbody.rotation, _targetRotation, step));
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, step);
        }
    }
    

    private void ProcessContentLoss()
    {
        if (Time.fixedTime - _lastSpillTime < _spillCooldown) return;
        
        float spillAmount = 0f;
        
        if (_currentTiltAngle > _maxTiltAngle)
        {
            float tiltFactor = (_currentTiltAngle - _maxTiltAngle) / (90f - _maxTiltAngle);
            spillAmount += _spillRateByTilt * tiltFactor * Time.fixedDeltaTime;
        }
        
        if (_currentVelocityMagnitude > _maxVelocity || _currentAngularVelocityMagnitude > _maxAngularVelocity)
        {
            float velocityFactor = Mathf.Max(
                (_currentVelocityMagnitude - _maxVelocity) / _maxVelocity,
                (_currentAngularVelocityMagnitude - _maxAngularVelocity) / _maxAngularVelocity
            );
            spillAmount += _spillRateByMovement * velocityFactor * Time.fixedDeltaTime;
        }
        
        if (spillAmount > 0f)
        {
            SpillContent(spillAmount);
            _lastSpillTime = Time.fixedTime;
        }
    }

    private void SpillContent(float amount)
    {
        float previousContent = _currentContent;
        _currentContent = Mathf.Max(0f, _currentContent - amount / _maxCapacity);        
        if (_currentContent != previousContent)
        {
            OnContentChanged?.Invoke(_currentContent);
            OnContentSpilled?.Invoke(GetLowestEdgePoint(_emissionSource, pointsPerEdge), amount, GetSpillDirection());
            
            if (IsEmpty && !previousContent.Equals(0f))
            {
                OnContainerEmpty?.Invoke();
            }
        }
    }

    private Vector3 GetSpillPosition()
    {
        Bounds bounds = _emissionSource != null ? _emissionSource.bounds : GetComponent<Collider>().bounds;
        
        if (_currentTiltAngle < _maxTiltAngle * 0.8f)
        {
            return new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
        }
        
        Vector3 tiltDirection = Vector3.Cross(Vector3.up, transform.up).normalized;
        
        Vector3 spillPoint = CalculateSpillPointOnBoxSide(bounds, tiltDirection);
        
        return spillPoint;
    }

    private Vector3 CalculateSpillPointOnBoxSide(Bounds bounds, Vector3 tiltDirection)
    {
        Vector3 center = bounds.center;
        Vector3 size = bounds.size;
        
        float dotX = Vector3.Dot(tiltDirection, Vector3.right);
        float dotZ = Vector3.Dot(tiltDirection, Vector3.forward);
        
        Vector3 spillPoint = center;
        
        if (Mathf.Abs(dotX) > Mathf.Abs(dotZ))
        {
            spillPoint.x += Mathf.Sign(dotX) * size.x * 0.5f;
            spillPoint.y = bounds.max.y - size.y * 0.1f;
        }
        else
        {
            spillPoint.z += Mathf.Sign(dotZ) * size.z * 0.5f;
            spillPoint.y = bounds.max.y - size.y * 0.1f;
        }
        
        return spillPoint;
    }
    
    Vector3 GetLowestEdgePoint(BoxCollider box, int samplesPerEdge)
    {
        Vector3 half = box.size * 0.5f;
        Vector3 c = box.center;
        Transform t = box.transform;

        Vector3[] topCorners = new Vector3[4];
        topCorners[0] = c + new Vector3(-half.x, half.y, -half.z);
        topCorners[1] = c + new Vector3( half.x, half.y, -half.z);
        topCorners[2] = c + new Vector3( half.x, half.y,  half.z);
        topCorners[3] = c + new Vector3(-half.x, half.y,  half.z);

        Vector3 lowest = Vector3.positiveInfinity;
        float lowestY = float.PositiveInfinity;

        for (int i = 0; i < 4; i++)
        {
            Vector3 a = topCorners[i];
            Vector3 b = topCorners[(i + 1) % 4];

            for (int s = 0; s <= samplesPerEdge; s++)
            {
                float tLerp = s / (float)samplesPerEdge;
                Vector3 localPoint = Vector3.Lerp(a, b, tLerp);
                Vector3 worldPoint = t.TransformPoint(localPoint);

                if (worldPoint.y < lowestY)
                {
                    lowestY = worldPoint.y;
                    lowest = worldPoint;
                }
            }
        }

        return lowest;
    }

    private Vector3 GetSpillDirection()
    {
        if (_currentTiltAngle < _maxTiltAngle * 0.8f)
        {
            return Vector3.down;
        }
        
        Vector3 tiltDirection = Vector3.Cross(Vector3.up, transform.up).normalized;
        
        Vector3 spillDirection = tiltDirection + Vector3.down * 0.5f;
        
        if (_rigidbody.velocity.magnitude > 0.1f)
        {
            Vector3 velocityDirection = _rigidbody.velocity.normalized;
            velocityDirection.y = 0;
            spillDirection += velocityDirection * 0.4f;
        }
        
        return spillDirection.normalized;
    }

    public bool AddContent(float amount)
    {
        if (IsFull) return false;
        
        float previousContent = _currentContent;
        _currentContent = Mathf.Min(1f, _currentContent + amount / _maxCapacity);
        
        if (_currentContent != previousContent)
        {
            OnContentChanged?.Invoke(_currentContent);
            return true;
        }
        
        return false;
    }

    public bool RemoveContent(float amount)
    {
        if (IsEmpty) return false;
        
        float previousContent = _currentContent;
        _currentContent = Mathf.Max(0f, _currentContent - amount / _maxCapacity);
        
        if (_currentContent != previousContent)
        {
            OnContentChanged?.Invoke(_currentContent);
            
            if (IsEmpty)
            {
                OnContainerEmpty?.Invoke();
            }
            
            return true;
        }
        
        return false;
    }

    public void SetContentPercentage(float percentage)
    {
        _currentContent = Mathf.Clamp01(percentage);
        OnContentChanged?.Invoke(_currentContent);
    }

    public string GetStatusInfo()
    {
        return $"Contenido: {(_currentContent * 100):F1}% | " +
               $"Inclinación: {_currentTiltAngle:F1}° | " +
               $"Estado: {(_isUnstable ? "INESTABLE" : "ESTABLE")}";
    }

    public string GetSpillInfo()
    {
        if (!_isUnstable) return "Estable - No derramándose";
        
        Vector3 spillDir = GetSpillDirection();
        return $"Derramándose hacia: ({spillDir.x:F2}, {spillDir.y:F2}, {spillDir.z:F2})";
    }

    public void SetEmissionSource(BoxCollider emissionSource)
    {
        _emissionSource = emissionSource;
    }

    public BoxCollider GetEmissionSource()
    {
        return _emissionSource;
    }

    [ContextMenu("Auto-Find Emission Source")]
    public void AutoFindEmissionSource()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            _emissionSource = boxCollider;
            Debug.Log($"Emission source asignado automáticamente: {boxCollider.name}");
            return;
        }
        
        boxCollider = GetComponentInChildren<BoxCollider>();
        if (boxCollider != null)
        {
            _emissionSource = boxCollider;
            Debug.Log($"Emission source encontrado en hijo: {boxCollider.name}");
            return;
        }
        
        Debug.LogWarning("No se encontró ningún BoxCollider para usar como emission source");
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = _currentTiltAngle > _maxTiltAngle ? Color.red : Color.green;
        Vector3 upDir = transform.up * 2f;
        Gizmos.DrawRay(transform.position, upDir);
        
        if (_emissionSource != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(_emissionSource.bounds.center, _emissionSource.bounds.size);
            
            UnityEditor.Handles.color = Color.blue;
            UnityEditor.Handles.Label(_emissionSource.bounds.center + Vector3.up * (_emissionSource.bounds.size.y * 0.6f), 
                "Emission Source");
        }
        
        if (Application.isPlaying && _isUnstable && !IsEmpty)
        {
            Vector3 spillPos = GetLowestEdgePoint(_emissionSource, pointsPerEdge);
            Vector3 spillDir = GetSpillDirection();
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(spillPos, 0.1f);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(spillPos, spillDir * 1.5f);
        }

        if (Application.isPlaying)
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up * 3f, GetStatusInfo());
        }
        
        if (_enableAutoTilt)
        {
            Vector3 center = _emissionSource != null ? _emissionSource.bounds.center : transform.position;
            Gizmos.color = new Color(0f, 0.5f, 1f, 0.15f);
            Gizmos.DrawSphere(center, _autoTiltRange);
            Gizmos.color = new Color(0f, 0.5f, 1f, 0.8f);
            Gizmos.DrawWireSphere(center, _autoTiltRange);
        }
    }
#endif
}