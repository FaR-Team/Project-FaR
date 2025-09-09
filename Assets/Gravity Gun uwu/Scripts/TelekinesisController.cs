using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TelekinesisController : MonoBehaviour
{
    [Header("Configuración Básica")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask grabLayerMask = 1;
    [SerializeField] private Transform laserStartPoint;
    
    [Header("Rango y Distancia")]
    [SerializeField] private float maxGrabDistance = 20f;
    [SerializeField] private float minHoldDistance = 2f;
    [SerializeField] private float maxHoldDistance = 15f;
    [SerializeField] private float scrollSensitivity = 2f;
    
    [Header("Fuerzas")]
    [SerializeField] private float followStrength = 2000f;
    [SerializeField] private float followDamping = 50f;
    [SerializeField] private float rotationStrength = 1000f;
    [SerializeField] private float rotationDamping = 25f;
    [SerializeField] private float maxFollowForce = 10000f;
    
    [Header("Configuración de Masa")]
    [SerializeField] private AnimationCurve massCompensationCurve = AnimationCurve.Linear(0.1f, 1f, 10f, 0.3f);
    [SerializeField] private float lightObjectThreshold = 1f;
    [SerializeField] private float heavyObjectThreshold = 5f;
    [SerializeField] private float maxMassForTelekinesis = 20f;
    [SerializeField] private bool useAdaptiveForces = true;
    [SerializeField] private float massBasedDamping = 1f;
    
    [Header("Sistema de Agarre")]
    [SerializeField] private float maxGrabOffsetDistance = 2f;
    [SerializeField] private AnimationCurve stabilityByOffset = AnimationCurve.EaseInOut(0f, 1f, 1f, 0.2f);
    [SerializeField] private AnimationCurve stabilityByMass = AnimationCurve.EaseInOut(0.1f, 1f, 10f, 0.1f);
    [SerializeField] private float gravityCompensation = 0.8f;
    [SerializeField] private float instabilityTorqueMultiplier = 5f;
    [SerializeField] private float wobbleForce = 100f;
    [SerializeField] private float maxWobbleSpeed = 2f;
    
    [Header("Comportamiento")]
    [SerializeField] private bool maintainOrientation = true;
    [SerializeField] private float orientationStrength = 500f;
    [SerializeField] private float smoothingFactor = 0.85f;
    [SerializeField] private bool useMouseSmoothing = true;
    [SerializeField] private float initialGrabDuration = 0.5f;
    [SerializeField] private AnimationCurve initialGrabCurve = AnimationCurve.EaseInOut(0f, 0.1f, 1f, 1f);
    
    [Header("Efectos Visuales")]
    [SerializeField] private bool showDebugLines = true;
    [SerializeField] private Material outlineMaterial;
    
    private TelekineticObject grabbedObject;
    private Vector3 targetPosition;
    private Vector3 grabOffset;
    private float currentHoldDistance;
    private bool isGrabbing = false;
    
    private Vector3 smoothedTargetPosition;
    private Quaternion initialGrabRotation;
    private Quaternion grabRotationOffset;
    private float grabStartTime;
    private bool isInitialGrab = false;
    
    private float grabOffsetMagnitude;
    private float objectMass;
    private float stabilityFactor;
    private Vector3 wobbleAccumulator;
    private float currentInstability;
    
    public Vector3 StartPoint { get; private set; }
    public Vector3 MidPoint { get; private set; }
    public Vector3 EndPoint { get; private set; }
    
    [Header("Eventos")]
    public UnityEvent<GameObject> OnObjectGrabbed;
    public UnityEvent OnObjectReleased;
    
    public GameObject Energia;
    public static bool isGrabbingObject => Instance?.isGrabbing ?? false;
    public static TelekinesisController Instance { get; private set; }
    
    private void Awake()
    {
        Instance = this;
        
        if (playerCamera == null)
            playerCamera = Camera.main;
    }
    
    private void Update()
    {
        HandleInput();
        UpdateVisualPoints();
    }
    
    private void FixedUpdate()
    {
        if (grabbedObject != null)
        {
            ApplyTelekineticForces();
        }
    }
    
    private void HandleInput()
    {
        bool hasEnergy = Energia == null || Energy.RemainingEnergy >= 1;
        
        if (Input.GetMouseButton(0) && hasEnergy)
        {
            if (grabbedObject == null)
            {
                TryGrabObject();
            }
            else
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                if (Mathf.Abs(scroll) > 0.01f)
                {
                    currentHoldDistance = Mathf.Clamp(
                        currentHoldDistance + scroll * scrollSensitivity,
                        minHoldDistance,
                        maxHoldDistance
                    );
                }
                
                UpdateTargetPosition();
            }
        }
        else if (grabbedObject != null)
        {
            ReleaseObject();
        }
    }
    
    private void TryGrabObject()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
        
        if (Physics.Raycast(ray, out RaycastHit hit, maxGrabDistance, grabLayerMask))
        {
            Rigidbody rb = hit.rigidbody;
            if (rb != null && !rb.isKinematic)
            {
                if (rb.mass > maxMassForTelekinesis)
                {
                    return;
                }
                
                GrabObject(rb, hit);
            }
        }
    }
    
    private void GrabObject(Rigidbody rigidbody, RaycastHit hit)
    {
        grabbedObject = rigidbody.GetComponent<TelekineticObject>();
        if (grabbedObject == null)
        {
            grabbedObject = rigidbody.gameObject.AddComponent<TelekineticObject>();
        }
        
        grabbedObject.Initialize(hit);
        
        currentHoldDistance = Vector3.Distance(playerCamera.transform.position, hit.point);
        currentHoldDistance = Mathf.Clamp(currentHoldDistance, minHoldDistance, maxHoldDistance);
        
        Vector3 centerOfMass = rigidbody.worldCenterOfMass;
        Vector3 grabPoint = hit.point;
        
        grabOffset = grabPoint - centerOfMass;
        
        objectMass = rigidbody.mass;
        grabOffsetMagnitude = grabOffset.magnitude;
        stabilityFactor = 1f;
        currentInstability = 0f;
        wobbleAccumulator = Vector3.zero;
        
        initialGrabRotation = rigidbody.rotation;
        grabRotationOffset = Quaternion.identity;
        
        smoothedTargetPosition = rigidbody.worldCenterOfMass + grabOffset;
        
        grabStartTime = Time.time;
        isInitialGrab = true;
        
        isGrabbing = true;
        UpdateTargetPosition();
        
        grabbedObject.SetOutlineActive(true, outlineMaterial);
        
        OnObjectGrabbed?.Invoke(rigidbody.gameObject);
        
        if (Energia != null)
        {
            if (Energy._ContadorActivo == false)
            {
                Energy._animationComp.Play("Entrar uwuw");
                StartCoroutine(Energy.Walter());
                Energy._ContadorActivo = true;
                Energy.timer = 5;
                Energy._yaAnimo = false;
            }
            else
            {
                Energy.timer = 5;
            }
        }
    }
    
    private void UpdateTargetPosition()
    {
        if (grabbedObject == null) return;
        
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
        targetPosition = ray.GetPoint(currentHoldDistance);
    }
    
    private void ApplyTelekineticForces()
    {
        if (grabbedObject == null) return;
        
        Rigidbody rb = grabbedObject.Rigidbody;
        if (rb == null) return;
        
        Vector3 currentPosition = rb.worldCenterOfMass;
        Vector3 currentGrabPoint = currentPosition + rb.rotation * grabOffset;
        Vector3 grabPointError = targetPosition - currentGrabPoint;
        
        float massScaleFactor = Mathf.Clamp(rb.mass / 2f, 0.5f, 2f);
        
        float followForceMultiplier = rb.mass <= 1f ? 0.05f : 0.1f;
        float dampingMultiplier = rb.mass <= 1f ? 0.8f : 0.5f;
        
        Vector3 followForce = grabPointError * followStrength * followForceMultiplier;
        Vector3 dampingForce = -rb.velocity * followDamping * dampingMultiplier;
        
        Vector3 gravityCompensation = -Physics.gravity * rb.mass * 0.9f;
        
        Vector3 totalForce = followForce + dampingForce + gravityCompensation;
        
        if (IsValidVector3(totalForce))
        {
            float maxForce = maxFollowForce * 0.3f * massScaleFactor;
            totalForce = Vector3.ClampMagnitude(totalForce, maxForce);
            rb.AddForceAtPosition(totalForce, currentGrabPoint, ForceMode.Force);
        }
        
        if (maintainOrientation)
        {
            Quaternion desiredRotation = initialGrabRotation;
            Quaternion rotationError = desiredRotation * Quaternion.Inverse(rb.rotation);
            
            rotationError.ToAngleAxis(out float angle, out Vector3 axis);
            if (float.IsNaN(angle) || float.IsInfinity(angle) || !IsValidVector3(axis))
            {
                return;
            }
            
            if (angle > 180f) angle -= 360f;
            
            float torqueMultiplier = rb.mass <= 1f ? 0.05f : 0.1f;
            Vector3 torque = axis * angle * Mathf.Deg2Rad * orientationStrength * torqueMultiplier;
            Vector3 angularDamping = -rb.angularVelocity * rotationDamping * (rb.mass <= 1f ? 2f : 1f);
            
            if (IsValidVector3(torque) && IsValidVector3(angularDamping))
            {
                Vector3 totalTorque = torque + angularDamping;
                if (IsValidVector3(totalTorque))
                {
                    rb.AddTorque(totalTorque, ForceMode.Force);
                }
            }
        }
        
        Debug.Log($"Mass: {rb.mass:F1} - Forces - Follow: {followForce.magnitude:F2}, Damping: {dampingForce.magnitude:F2}, Gravity: {gravityCompensation.magnitude:F2}, Total: {totalForce.magnitude:F2}");
    }
    
    private bool IsValidVector3(Vector3 vector)
    {
        return !float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z) &&
               !float.IsInfinity(vector.x) && !float.IsInfinity(vector.y) && !float.IsInfinity(vector.z);
    }
    
    private void ReleaseObject()
    {
        if (grabbedObject == null) return;
        
        grabbedObject.SetOutlineActive(false);
        
        var releasedObject = grabbedObject.gameObject;
        grabbedObject.Cleanup();
        grabbedObject = null;
        isGrabbing = false;
        isInitialGrab = false;
        
        OnObjectReleased?.Invoke();
        
        if (Energia != null)
        {
            Energy.UseEnergy(1);
        }
        
        StartCoroutine(ResetGrabbingFlag());
    }
    
    private IEnumerator ResetGrabbingFlag()
    {
        yield return new WaitForSeconds(0.1f);
    }
    
    private void UpdateVisualPoints()
    {
        if (laserStartPoint != null)
        {
            StartPoint = laserStartPoint.position;
        }
        else
        {
            StartPoint = playerCamera.transform.position;
        }
        
        if (isGrabbing && grabbedObject != null)
        {
            MidPoint = targetPosition;
            EndPoint = grabbedObject.GrabPoint;
        }
        else
        {
            MidPoint = Vector3.zero;
            EndPoint = Vector3.zero;
        }
    }
    
    public Rigidbody GetGrabbedRigidbody() => grabbedObject?.Rigidbody;
    public Transform GetGrabbedTransform() => grabbedObject?.transform;
    public bool HasGrabbedObject => grabbedObject != null;
    
    public float CurrentStability => stabilityFactor;
    public float CurrentInstability => currentInstability;
    public float GrabbedObjectMass => objectMass;
    public float GrabOffsetDistance => grabOffsetMagnitude;
    public bool IsObjectTooHeavy(float mass) => mass > maxMassForTelekinesis;
    public bool IsGrabUnstable => currentInstability > 0.5f;
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showDebugLines || !isGrabbing || grabbedObject == null) return;
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(StartPoint, MidPoint);
        Gizmos.DrawLine(MidPoint, EndPoint);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(grabbedObject.Rigidbody.worldCenterOfMass, 0.1f);
        
        Vector3 currentGrabPoint = grabbedObject.Rigidbody.worldCenterOfMass + grabbedObject.Rigidbody.rotation * grabOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(currentGrabPoint, 0.05f);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(targetPosition, 0.1f);
        
        Gizmos.color = Color.Lerp(Color.red, Color.green, stabilityFactor);
        Gizmos.DrawLine(grabbedObject.Rigidbody.worldCenterOfMass, currentGrabPoint);
        
        UnityEditor.Handles.Label(grabbedObject.transform.position + Vector3.up * 2f, 
            $"Mass: {objectMass:F1}kg\nStability: {stabilityFactor:F2}\nInstability: {currentInstability:F2}\nOffset: {grabOffsetMagnitude:F2}m");
    }
#endif
}
