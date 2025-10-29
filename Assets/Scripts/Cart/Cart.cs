using UnityEngine;
using System.Collections;

public class Cart : MonoBehaviour
{
    [Header("Cart Configuration")]
    [SerializeField] public Transform frontWheelsParent;
    [SerializeField] public Transform backHandle;
    [SerializeField] private bool onlyGrabFromHandle = true;
    [SerializeField] private float wheelRotationSpeed = 360f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 15f;
    [SerializeField] private float turnSpeed = 90f;
    
    [Header("Telekinesis Attachment")]
    [SerializeField] private float attachmentDistance = 2f;
    [SerializeField] private float attachmentHeight = 1f;
    [SerializeField] private float followStrength = 1000f;
    [SerializeField] private float followDamping = 50f;
    [SerializeField] private float rotationStrength = 500f;
    [SerializeField] private float rotationDamping = 25f;
    
    private Rigidbody cartRigidbody;
    private Transform playerTransform;
    private bool isAttachedToPlayer = false;
    private float currentSpeed = 0f;
    private Vector3 lastPosition;
    private TelekineticObject telekineticObject;
    
    private Transform[] frontWheels;
    
    private void Awake()
    {
        cartRigidbody = GetComponent<Rigidbody>();
        if (cartRigidbody == null)
        {
            cartRigidbody = gameObject.AddComponent<Rigidbody>();
        }
        
        cartRigidbody.mass = 8f;
        cartRigidbody.drag = 3f;
        cartRigidbody.angularDrag = 5f;
        cartRigidbody.useGravity = true;
        cartRigidbody.isKinematic = false;
        
        cartRigidbody.centerOfMass = new Vector3(0, -0.2f, 0);
        
        StartCoroutine(InitializeTelekineticObject());
        
        SetupWheels();
        
        lastPosition = transform.position;
        
        var playerController = FindObjectOfType<FaRUtils.FPSController.FaRCharacterController>();
        if (playerController != null)
        {
            playerTransform = playerController.transform;
        }
    }
    
    private void SetupWheels()
    {
        if (frontWheelsParent != null)
        {
            frontWheels = new Transform[frontWheelsParent.childCount];
            for (int i = 0; i < frontWheelsParent.childCount; i++)
            {
                frontWheels[i] = frontWheelsParent.GetChild(i);
            }
        }
    }
    
    private void Update()
    {
        CheckTelekinesisAttachment();
        UpdateWheelRotations();
        CalculateSpeed();
    }
    
    private void FixedUpdate()
    {
        if (isAttachedToPlayer && playerTransform != null)
        {
            ApplyPlayerAttachmentForces();
        }
    }
    
    private void CheckTelekinesisAttachment()
    {
        bool isBeingGrabbed = TelekinesisController.Instance != null && 
                             TelekinesisController.Instance.GetGrabbedRigidbody() == cartRigidbody;
        
        if (isBeingGrabbed && !isAttachedToPlayer)
        {
            AttachToPlayer();
        }
        else if (!isBeingGrabbed && isAttachedToPlayer)
        {
            DetachFromPlayer();
        }
    }
    
    private void AttachToPlayer()
    {
        isAttachedToPlayer = true;
        Debug.Log("Cart grabbed - now responds to player movement!");
        
        cartRigidbody.drag = 0.5f;
        cartRigidbody.angularDrag = 2f;
        
        cartRigidbody.constraints = RigidbodyConstraints.FreezePositionY;
    }
    
    private void DetachFromPlayer()
    {
        isAttachedToPlayer = false;
        Debug.Log("Cart released - returning to normal physics!");
        
        cartRigidbody.drag = 3f;
        cartRigidbody.angularDrag = 5f;
        
        cartRigidbody.constraints = RigidbodyConstraints.None;
    }
    
    private void ApplyPlayerAttachmentForces()
    {
        if (playerTransform == null || backHandle == null) return;
        
        Vector3 playerForward = playerTransform.forward;
        Vector3 desiredCartPosition = playerTransform.position + playerForward * attachmentDistance;
        desiredCartPosition.y = transform.position.y;
        
        Vector3 positionError = desiredCartPosition - transform.position;
        Vector3 followForce = positionError * followStrength;
        Vector3 dampingForce = -cartRigidbody.velocity * followDamping;
        
        Vector3 totalForce = followForce + dampingForce;
        totalForce.y = 0;
        cartRigidbody.AddForce(totalForce, ForceMode.Force);
        
        playerForward.y = 0;
        
        if (playerForward.magnitude > 0.1f)
        {
            Vector3 cartForward = playerForward;
            Quaternion desiredRotation = Quaternion.LookRotation(cartForward, Vector3.up) * Quaternion.Euler(0, -90, 0);
            
            float currentYAngle = transform.eulerAngles.y;
            float desiredYAngle = desiredRotation.eulerAngles.y;
            
            float angleDifference = Mathf.DeltaAngle(currentYAngle, desiredYAngle);
            
            Vector3 torque = Vector3.up * angleDifference * Mathf.Deg2Rad * rotationStrength;
            Vector3 angularDamping = new Vector3(0, -cartRigidbody.angularVelocity.y * rotationDamping, 0);
            
            cartRigidbody.AddTorque(torque + angularDamping, ForceMode.Force);
        }
        
        Vector3 currentUp = transform.up;
        Vector3 desiredUp = Vector3.up;
        Vector3 stabilizingTorque = Vector3.Cross(currentUp, desiredUp) * rotationStrength * 2f;
        stabilizingTorque.y = 0;
        
        cartRigidbody.AddTorque(stabilizingTorque, ForceMode.Force);
    }
    
    private bool IsValidVector3(Vector3 vector)
    {
        return !float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z) &&
               !float.IsInfinity(vector.x) && !float.IsInfinity(vector.y) && !float.IsInfinity(vector.z);
    }
    
    private IEnumerator InitializeTelekineticObject()
    {
        yield return null;
        
        telekineticObject = GetComponent<TelekineticObject>();
        if (telekineticObject == null)
        {
            telekineticObject = gameObject.AddComponent<TelekineticObject>();
        }
    }
    
    private void CalculateSpeed()
    {
        Vector3 currentPosition = transform.position;
        Vector3 movement = currentPosition - lastPosition;
        currentSpeed = movement.magnitude / Time.deltaTime;
        lastPosition = currentPosition;
    }
    
    private void UpdateWheelRotations()
    {
        if (currentSpeed < 0.1f) return;
        
        float rotationAmount = currentSpeed * wheelRotationSpeed * Time.deltaTime;
        
        if (float.IsNaN(rotationAmount) || float.IsInfinity(rotationAmount))
        {
            return;
        }
        
        if (frontWheels != null)
        {
            foreach (Transform wheel in frontWheels)
            {
                if (wheel != null)
                {
                    Vector3 currentRotation = wheel.localEulerAngles;
                    currentRotation.x += rotationAmount;
                    wheel.localEulerAngles = currentRotation;
                }
            }
        }
    }
    
    public bool IsAttachedToPlayer => isAttachedToPlayer;
    public float CurrentSpeed => currentSpeed;
    
    private void OnDrawGizmosSelected()
    {
        if (playerTransform != null && isAttachedToPlayer)
        {
            Vector3 attachmentPos = playerTransform.position + playerTransform.forward * attachmentDistance;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(attachmentPos, 0.3f);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, attachmentPos);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(playerTransform.position, playerTransform.forward * 2f);
            
            if (backHandle != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(backHandle.position, 0.2f);
            }
        }
    }
}