using System.Collections.Generic;
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
    [SerializeField] private float rotationDamping = 25f;
    [SerializeField] private float maxFollowForce = 10000f;
    [SerializeField, Range(0f, 1f)] private float gravityCompensationRatio = 0.9f;
    
    [Header("Configuración de Masa")]
    [SerializeField] private float maxMassForTelekinesis = 20f;
    
    [Header("Comportamiento")]
    [SerializeField] private bool maintainOrientation = true;
    [SerializeField] private float orientationStrength = 500f;
    
    [SerializeField] private bool enableRightClickRotation = true;
    [SerializeField] private float rotationSensitivity = 5f;
    [SerializeField] private float rotationTorqueMultiplier = 2f;
    
    [Header("Efectos Visuales")]
    [SerializeField] private bool showDebugLines = true;
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private bool enableTelekineticRay = true;
    
    private TelekineticRayRenderer rayRenderer;
    
    private TelekineticObject grabbedObject;
    private Vector3 targetPosition;
    private Vector3 grabOffset;
    private float currentHoldDistance;
    private bool isGrabbing = false;
    private bool isRotating = false;
    
    private Quaternion initialGrabRotation;
    private Quaternion targetGrabRotation;
    private float grabOffsetMagnitude;
    private float objectMass;
    private float stabilityFactor;
    private float currentInstability;
    
    public Vector3 StartPoint { get; private set; }
    public Vector3 MidPoint { get; private set; }
    public Vector3 EndPoint { get; private set; }
    
    public bool IsRotating => isGrabbing && isRotating;
    public static bool isRotatingObject => Instance?.IsRotating ?? false;
    
    [Header("Eventos")]
    public UnityEvent<GameObject> OnObjectGrabbed;
    public UnityEvent OnObjectReleased;
    
    [Tooltip("Referencia opcional a Energy GameObject (se usa Energy.instance automáticamente si está en escena)")]
    public GameObject Energia;
    public static bool isGrabbingObject => Instance?.isGrabbing ?? false;
    public static TelekinesisController Instance { get; private set; }
    
    private void Awake()
    {
        Instance = this;
        UpdateCameraReference();
            
        rayRenderer = GetComponent<TelekineticRayRenderer>();
        if (rayRenderer == null && enableTelekineticRay)
        {
            rayRenderer = gameObject.AddComponent<TelekineticRayRenderer>();
        }
    }
    
    private void OnEnable()
    {
        Instance = this;
        UpdateCameraReference();
    }

    private void UpdateCameraReference()
    {
        if (playerCamera == null || !playerCamera)
        {
            playerCamera = Camera.main;
            if (playerCamera == null || !playerCamera)
            {
                var cam = FindObjectOfType<Camera>();
                if (cam != null) playerCamera = cam;
            }
        }
    }
    
    private void Update()
    {
        UpdateCameraReference();
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
    
    private bool isPerformingNoEnergyFeedback = false;
    public bool IsPerformingNoEnergyFeedback => isPerformingNoEnergyFeedback;

    private void HandleInput()
    {
        if (isPerformingNoEnergyFeedback) return;

        bool hasEnergy = Energy.instance == null || Energy.RemainingEnergy >= 1;
        
        if (Input.GetMouseButton(0))
        {
            if (hasEnergy)
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
                    
                    bool canRotate = enableRightClickRotation && grabbedObject != null && grabbedObject.IsRotable;

                    if (canRotate && Input.GetMouseButton(1))
                    {
                        if (Input.GetMouseButtonDown(1) && grabbedObject.Rigidbody != null)
                        {
                            targetGrabRotation = grabbedObject.Rigidbody.rotation;
                        }

                        isRotating = true;

                        Vector2 lookDelta;
                        if (FaRUtils.FPSController.FaRCharacterController.instance != null)
                        {
                            lookDelta = FaRUtils.FPSController.FaRCharacterController.instance.GetPlayerLook();
                        }
                        else
                        {
                            lookDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                        }

                        float mouseX = lookDelta.x * rotationSensitivity * 0.2f;
                        float mouseY = lookDelta.y * rotationSensitivity * 0.2f;

                        Vector3 camUp = playerCamera != null ? playerCamera.transform.up : Vector3.up;
                        Vector3 camRight = playerCamera != null ? playerCamera.transform.right : Vector3.right;

                        Quaternion yaw = Quaternion.AngleAxis(-mouseX, camUp);
                        Quaternion pitch = Quaternion.AngleAxis(mouseY, camRight);

                        targetGrabRotation = yaw * pitch * targetGrabRotation;

                        if (Input.GetKey(KeyCode.Q))
                        {
                            Vector3 camForward = playerCamera != null ? playerCamera.transform.forward : Vector3.forward;
                            targetGrabRotation = Quaternion.AngleAxis(rotationSensitivity * 30f * Time.deltaTime, camForward) * targetGrabRotation;
                        }
                        else if (Input.GetKey(KeyCode.E))
                        {
                            Vector3 camForward = playerCamera != null ? playerCamera.transform.forward : Vector3.forward;
                            targetGrabRotation = Quaternion.AngleAxis(-rotationSensitivity * 30f * Time.deltaTime, camForward) * targetGrabRotation;
                        }
                    }
                    else
                    {
                        isRotating = false;
                    }
                    
                    UpdateTargetPosition();
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                TryPerformNoEnergyFeedback();
            }
        }
        else if (grabbedObject != null)
        {
            ReleaseObject();
        }
    }

    private void TryPerformNoEnergyFeedback()
    {
        if (isPerformingNoEnergyFeedback) return;

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, maxGrabDistance, grabLayerMask))
        {
            Rigidbody rb = hit.rigidbody;
            if (rb != null && !rb.isKinematic)
            {
                if (rb.mass <= maxMassForTelekinesis)
                {
                    StartCoroutine(NoEnergyFeedbackRoutine(rb, hit));
                    return;
                }
            }
        }

        if (Energy.instance != null)
        {
            Energy.instance.TryUseAndAnimateEnergy(1, 5f);
        }
    }

    private System.Collections.IEnumerator NoEnergyFeedbackRoutine(Rigidbody targetRb, RaycastHit hit)
    {
        isPerformingNoEnergyFeedback = true;

        float totalDuration = 1.35f;
        if (FaRUtils.FPSController.FaRCharacterController.instance != null)
        {
            FaRUtils.FPSController.FaRCharacterController.instance.LockMovementFor(totalDuration);
        }

        if (Energy.instance != null)
        {
            Energy.instance.ShowNoEnergyFeedback();
        }

        TelekineticObject teleObj = targetRb.GetComponent<TelekineticObject>();
        if (teleObj == null)
        {
            teleObj = targetRb.gameObject.AddComponent<TelekineticObject>();
        }

        teleObj.SetOutlineActive(true, outlineMaterial);
        teleObj.SetOutlineExpand(0f);
        teleObj.SetOutlineExplode(0f);

        Vector3 initialPos = targetRb.transform.position;
        Quaternion initialRot = targetRb.transform.rotation;
        bool wasKinematic = targetRb.isKinematic;
        targetRb.isKinematic = true;

        Vector3 targetLiftPos = initialPos + Vector3.up * 0.32f;

        float liftTime = 0.35f;
        float elapsed = 0f;
        while (elapsed < liftTime && targetRb != null)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / liftTime);
            targetRb.transform.position = Vector3.Lerp(initialPos, targetLiftPos, t);

            StartPoint = laserStartPoint != null ? laserStartPoint.position : playerCamera.transform.position;
            EndPoint = targetRb.worldCenterOfMass;
            MidPoint = (StartPoint + EndPoint) * 0.5f + Vector3.up * 0.3f;

            yield return null;
        }

        float shakeTime = 0.55f;
        elapsed = 0f;
        while (elapsed < shakeTime && targetRb != null)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / shakeTime;
            
            Vector3 jitter = UnityEngine.Random.insideUnitSphere * Mathf.Lerp(0.02f, 0.08f, progress);
            Quaternion rotJitter = Quaternion.Euler(
                UnityEngine.Random.Range(-8f, 8f) * progress,
                UnityEngine.Random.Range(-8f, 8f) * progress,
                UnityEngine.Random.Range(-8f, 8f) * progress
            );

            targetRb.transform.position = targetLiftPos + jitter;
            targetRb.transform.rotation = initialRot * rotJitter;

            Vector3 rayNoise = UnityEngine.Random.insideUnitSphere * (0.05f * progress);
            StartPoint = laserStartPoint != null ? laserStartPoint.position : playerCamera.transform.position;
            EndPoint = targetRb.worldCenterOfMass + rayNoise;
            MidPoint = (StartPoint + EndPoint) * 0.5f + Vector3.up * (0.3f + Mathf.Sin(elapsed * 25f) * 0.08f);

            teleObj.SetOutlineExpand(Mathf.Lerp(0f, 1.5f, progress));

            yield return null;
        }

        StartPoint = Vector3.zero;
        MidPoint = Vector3.zero;
        EndPoint = Vector3.zero;

        Bounds objBounds = new Bounds(targetRb.worldCenterOfMass, Vector3.one * 0.8f);
        Collider col = targetRb.GetComponent<Collider>();
        if (col != null)
        {
            objBounds = col.bounds;
        }
        else
        {
            Renderer r = targetRb.GetComponentInChildren<Renderer>();
            if (r != null) objBounds = r.bounds;
        }

        teleObj.SetOutlineExpand(0f);
        teleObj.SetOutlineActive(false);

        StartCoroutine(AnimateOutlineExplosionLines(targetRb.worldCenterOfMass, objBounds));

        if (targetRb != null)
        {
            targetRb.transform.rotation = initialRot;
            targetRb.isKinematic = wasKinematic;
            targetRb.velocity = Vector3.down * 1.5f;
        }

        yield return new WaitForSeconds(0.45f);

        isPerformingNoEnergyFeedback = false;
        StartPoint = Vector3.zero;
        MidPoint = Vector3.zero;
        EndPoint = Vector3.zero;
    }

    private class ExplosionLineSegment
    {
        public LineRenderer line;
        public Vector3 p1;
        public Vector3 p2;
        public Vector3 v1;
        public Vector3 v2;
        public Vector3 rotAxis;
        public float rotSpeed;
        public float baseWidth;
    }

    private System.Collections.IEnumerator AnimateOutlineExplosionLines(Vector3 center, Bounds bounds)
    {
        GameObject container = new GameObject("Telekinesis_Outline_Explosion");
        container.transform.position = center;

        Material lineMat = null;
        if (rayRenderer != null)
        {
            LineRenderer parentLine = rayRenderer.GetComponent<LineRenderer>();
            if (parentLine != null) lineMat = parentLine.sharedMaterial;
        }
        if (lineMat == null)
        {
            lineMat = new Material(Shader.Find("Sprites/Default"));
        }

        Vector3 extents = bounds.extents;
        if (extents.sqrMagnitude < 0.05f) extents = Vector3.one * 0.4f;

        List<Vector3[]> edgePairs = new List<Vector3[]>
        {
            new [] { center + new Vector3(-extents.x, -extents.y, -extents.z), center + new Vector3(extents.x, -extents.y, -extents.z) },
            new [] { center + new Vector3(extents.x, -extents.y, -extents.z), center + new Vector3(extents.x, -extents.y, extents.z) },
            new [] { center + new Vector3(extents.x, -extents.y, extents.z), center + new Vector3(-extents.x, -extents.y, extents.z) },
            new [] { center + new Vector3(-extents.x, -extents.y, extents.z), center + new Vector3(-extents.x, -extents.y, -extents.z) },

            new [] { center + new Vector3(-extents.x, extents.y, -extents.z), center + new Vector3(extents.x, extents.y, -extents.z) },
            new [] { center + new Vector3(extents.x, extents.y, -extents.z), center + new Vector3(extents.x, extents.y, extents.z) },
            new [] { center + new Vector3(extents.x, extents.y, extents.z), center + new Vector3(-extents.x, extents.y, extents.z) },
            new [] { center + new Vector3(-extents.x, extents.y, extents.z), center + new Vector3(-extents.x, extents.y, -extents.z) },

            new [] { center + new Vector3(-extents.x, -extents.y, -extents.z), center + new Vector3(-extents.x, extents.y, -extents.z) },
            new [] { center + new Vector3(extents.x, -extents.y, -extents.z), center + new Vector3(extents.x, extents.y, -extents.z) },
            new [] { center + new Vector3(extents.x, -extents.y, extents.z), center + new Vector3(extents.x, extents.y, extents.z) },
            new [] { center + new Vector3(-extents.x, -extents.y, extents.z), center + new Vector3(-extents.x, extents.y, extents.z) }
        };

        List<ExplosionLineSegment> segments = new List<ExplosionLineSegment>();
        Color startPink = new Color(1f, 0.2f, 1f, 1f);
        Color endPink = new Color(0.85f, 0.05f, 0.95f, 0.9f);

        for (int i = 0; i < edgePairs.Count; i++)
        {
            GameObject lineObj = new GameObject("LineSegment_" + i);
            lineObj.transform.SetParent(container.transform);
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();

            lr.material = lineMat;
            lr.positionCount = 2;
            lr.useWorldSpace = true;
            lr.startWidth = 0.055f;
            lr.endWidth = 0.055f;
            lr.startColor = startPink;
            lr.endColor = endPink;
            lr.alignment = LineAlignment.View;
            lr.generateLightingData = false;

            Vector3 a = edgePairs[i][0];
            Vector3 b = edgePairs[i][1];
            Vector3 segCenter = (a + b) * 0.5f;
            Vector3 outwardDir = (segCenter - center).normalized;
            if (outwardDir.sqrMagnitude < 0.01f) outwardDir = UnityEngine.Random.onUnitSphere;

            Vector3 tangent = Vector3.Cross(outwardDir, Vector3.up).normalized;
            if (tangent.sqrMagnitude < 0.01f) tangent = Vector3.Cross(outwardDir, Vector3.forward).normalized;

            float speed = UnityEngine.Random.Range(3.2f, 5.0f);
            float swingStrength = UnityEngine.Random.Range(3.5f, 6.0f) * (UnityEngine.Random.value > 0.5f ? 1f : -1f);
            Vector3 vel = outwardDir * speed + tangent * swingStrength + Vector3.up * UnityEngine.Random.Range(1.2f, 2.8f);

            Vector3 rotAxis = UnityEngine.Random.onUnitSphere;
            float rotSpeed = UnityEngine.Random.Range(500f, 1000f);

            ExplosionLineSegment seg = new ExplosionLineSegment
            {
                line = lr,
                p1 = a,
                p2 = b,
                v1 = vel + Vector3.Cross(rotAxis, (a - segCenter)) * 2.5f,
                v2 = vel + Vector3.Cross(rotAxis, (b - segCenter)) * 2.5f,
                rotAxis = rotAxis,
                rotSpeed = rotSpeed,
                baseWidth = UnityEngine.Random.Range(0.045f, 0.065f)
            };

            lr.SetPosition(0, seg.p1);
            lr.SetPosition(1, seg.p2);
            segments.Add(seg);
        }

        float duration = 0.55f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float alpha = Mathf.Clamp01(1f - t * t);
            float widthFactor = Mathf.Lerp(1f, 0.1f, t);

            for (int i = 0; i < segments.Count; i++)
            {
                ExplosionLineSegment seg = segments[i];
                if (seg.line == null) continue;

                seg.p1 += seg.v1 * Time.deltaTime;
                seg.p2 += seg.v2 * Time.deltaTime;

                seg.v1 += Vector3.down * (2.5f * Time.deltaTime);
                seg.v2 += Vector3.down * (2.5f * Time.deltaTime);
                seg.v1 *= Mathf.Pow(0.82f, Time.deltaTime * 10f);
                seg.v2 *= Mathf.Pow(0.82f, Time.deltaTime * 10f);

                seg.line.SetPosition(0, seg.p1);
                seg.line.SetPosition(1, seg.p2);

                Color c1 = new Color(startPink.r, startPink.g, startPink.b, alpha);
                Color c2 = new Color(endPink.r, endPink.g, endPink.b, alpha);
                seg.line.startColor = c1;
                seg.line.endColor = c2;
                seg.line.startWidth = seg.baseWidth * widthFactor;
                seg.line.endWidth = seg.baseWidth * widthFactor * 0.6f;
            }

            yield return null;
        }

        Destroy(container);
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
        
        if (rigidbody.GetComponent<Cart>() != null || rigidbody.GetComponentInParent<Cart>() != null)
        {
            grabbedObject.SetRotable(false);
        }
        
        grabbedObject.Initialize(hit);
        
        currentHoldDistance = Vector3.Distance(playerCamera.transform.position, rigidbody.worldCenterOfMass);
        currentHoldDistance = Mathf.Clamp(currentHoldDistance, minHoldDistance, maxHoldDistance);
        
        grabOffset = Vector3.zero;
        
        objectMass = rigidbody.mass;
        grabOffsetMagnitude = 0f;
        stabilityFactor = 1f;
        currentInstability = 0f;
        
        initialGrabRotation = rigidbody.rotation;
        targetGrabRotation = rigidbody.rotation;
        isRotating = false;
        
        isGrabbing = true;
        UpdateTargetPosition();
        
        grabbedObject.SetOutlineActive(true, outlineMaterial);
        
        OnObjectGrabbed?.Invoke(rigidbody.gameObject);
        
        if (Energy.instance != null)
        {
            Energy.instance.ShowBarOnly(5f);
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
        
        Cart cart = grabbedObject.GetComponent<Cart>();
        if (cart == null) cart = grabbedObject.GetComponentInParent<Cart>();
        
        if (cart != null && cart.enabled && cart.IsAttachedToPlayer)
        {
            return;
        }
        
        Vector3 currentPosition = rb.worldCenterOfMass;
        Vector3 grabPointError = targetPosition - currentPosition;
        
        float massScaleFactor = Mathf.Clamp(rb.mass / 2f, 0.5f, 2f);
        
        float followForceMultiplier = rb.mass <= 1f ? 0.05f : 0.1f;
        float dampingMultiplier = rb.mass <= 1f ? 0.8f : 0.5f;
        
        Vector3 followForce = grabPointError * followStrength * followForceMultiplier;
        Vector3 dampingForce = -rb.velocity * followDamping * dampingMultiplier;
        
        Vector3 gravityCompensation = -Physics.gravity * rb.mass * gravityCompensationRatio;
        
        Vector3 totalForce = followForce + dampingForce + gravityCompensation;
        
        if (IsValidVector3(totalForce))
        {
            float maxForce = maxFollowForce * 0.3f * massScaleFactor;
            totalForce = Vector3.ClampMagnitude(totalForce, maxForce);
            rb.AddForce(totalForce, ForceMode.Force);
        }
        
        if (maintainOrientation)
        {
            ContainerPhysics container = grabbedObject.GetComponent<ContainerPhysics>();
            bool isAutoTilting = container != null && container.IsAutoTilting;

            if (isRotating)
            {
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetGrabRotation, Time.fixedDeltaTime * 25f));
                rb.angularVelocity = Vector3.zero;
            }
            else if (isAutoTilting)
            {
                // No hacer nada, tiene que hacerlo containerphysics
            }
            else
            {
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetGrabRotation, Time.fixedDeltaTime * 15f));
                rb.angularVelocity = Vector3.zero;
            }
        }
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
        
        grabbedObject.Cleanup();
        grabbedObject = null;
        isGrabbing = false;
        isRotating = false;
        
        OnObjectReleased?.Invoke();
        
        if (Energy.instance != null)
        {
            Energy.UseEnergy(1);
        }
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
        
        if (isPerformingNoEnergyFeedback)
        {
            // Points are updated in NoEnergyFeedbackRoutine
        }
        else if (isGrabbing && grabbedObject != null)
        {
            MidPoint = targetPosition;
            Vector3 currentGrabPoint = grabbedObject.Rigidbody.worldCenterOfMass + grabbedObject.Rigidbody.rotation * grabOffset;
            EndPoint = currentGrabPoint;
        }
        else
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
            
            if (Physics.Raycast(ray, out RaycastHit hit, maxGrabDistance, grabLayerMask))
            {
                MidPoint = ray.GetPoint(Vector3.Distance(StartPoint, hit.point) * 0.7f);
                EndPoint = hit.point;
            }
            else
            {
                MidPoint = Vector3.zero;
                EndPoint = Vector3.zero;
            }
        }
        
        if (enableTelekineticRay)
        {
            if (rayRenderer == null)
            {
                rayRenderer = GetComponent<TelekineticRayRenderer>();
            }
            
            if (rayRenderer != null)
            {
                rayRenderer.SetRayActive(isGrabbing || isPerformingNoEnergyFeedback);
            }
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
