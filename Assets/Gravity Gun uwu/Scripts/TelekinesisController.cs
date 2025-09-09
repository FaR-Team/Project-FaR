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
    
    [Header("Comportamiento")]
    [SerializeField] private bool maintainOrientation = true;
    [SerializeField] private float orientationStrength = 500f;
    [SerializeField] private float smoothingFactor = 0.85f;
    [SerializeField] private bool useMouseSmoothing = true;
    
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
        
        initialGrabRotation = rigidbody.rotation;
        grabRotationOffset = Quaternion.identity;
        
        smoothedTargetPosition = targetPosition;
        
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
        
        if (useMouseSmoothing)
        {
            smoothedTargetPosition = Vector3.Lerp(smoothedTargetPosition, targetPosition, 
                Time.fixedDeltaTime * (1f - smoothingFactor) * 20f);
        }
        else
        {
            smoothedTargetPosition = targetPosition;
        }
        
        Vector3 desiredPosition = smoothedTargetPosition - grabOffset;
        Vector3 currentPosition = rb.worldCenterOfMass;
        Vector3 positionError = desiredPosition - currentPosition;
        
        Vector3 followForce = positionError * followStrength;
        Vector3 dampingForce = -rb.velocity * followDamping;
        Vector3 totalForce = followForce + dampingForce;

        totalForce /= Mathf.Sqrt(rb.mass);
        
        totalForce = Vector3.ClampMagnitude(totalForce, maxFollowForce);
        
        rb.AddForceAtPosition(totalForce, currentPosition, ForceMode.Force);
        
        if (maintainOrientation)
        {
            Quaternion desiredRotation = initialGrabRotation * grabRotationOffset;
            Quaternion currentRotation = rb.rotation;
            
            Quaternion rotationError = desiredRotation * Quaternion.Inverse(currentRotation);
            
            rotationError.ToAngleAxis(out float angle, out Vector3 axis);
            
            if (angle > 180f) angle -= 360f;
            
            Vector3 torque = axis * angle * Mathf.Deg2Rad * orientationStrength;
            Vector3 angularDamping = -rb.angularVelocity * rotationDamping;
            
            rb.AddTorque(torque + angularDamping, ForceMode.Force);
        }
        else
        {
            Vector3 grabPoint = grabbedObject.GrabPoint;
            Vector3 grabPointToCenter = currentPosition - grabPoint;
            Vector3 desiredGrabPoint = smoothedTargetPosition;
            Vector3 currentGrabPoint = currentPosition + grabPointToCenter;
            Vector3 grabPointError = desiredGrabPoint - currentGrabPoint;
            
            Vector3 torque = Vector3.Cross(grabPointToCenter, grabPointError) * rotationStrength;
            Vector3 angularDamping = -rb.angularVelocity * rotationDamping;
            
            rb.AddTorque(torque + angularDamping, ForceMode.Force);
        }
    }
    
    private void ReleaseObject()
    {
        if (grabbedObject == null) return;
        
        grabbedObject.SetOutlineActive(false);
        
        var releasedObject = grabbedObject.gameObject;
        grabbedObject.Cleanup();
        grabbedObject = null;
        isGrabbing = false;
        
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
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showDebugLines || !isGrabbing || grabbedObject == null) return;
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(StartPoint, MidPoint);
        Gizmos.DrawLine(MidPoint, EndPoint);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(grabbedObject.Rigidbody.worldCenterOfMass, 0.1f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(grabbedObject.GrabPoint, 0.05f);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(targetPosition, 0.1f);
    }
#endif
}
