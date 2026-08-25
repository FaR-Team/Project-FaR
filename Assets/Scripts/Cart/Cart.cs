using UnityEngine;
using System.Collections;

public class Cart : MonoBehaviour
{
    [Header("Cart Configuration")]
    [SerializeField] public Transform frontWheelsParent;
    [SerializeField] public Transform backHandle;
    [SerializeField] private float wheelRotationSpeed = 360f;
    
    [Header("Telekinesis Attachment")]
    [SerializeField] private float attachmentDistance = 2f;
    [SerializeField] private float followStrength = 1000f;
    [SerializeField] private float followDamping = 50f;
    [SerializeField] private float rotationStrength = 500f;
    [SerializeField] private float rotationDamping = 25f;
    [SerializeField] private float modelYRotationOffset = -90f;
    
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
        if (isAttachedToPlayer)
        {
            ApplyPlayerAttachmentForces();
        }
    }
    
    private void CheckTelekinesisAttachment()
    {
        bool isBeingGrabbed = false;

        TelekinesisController tc = TelekinesisController.Instance;
        if (tc == null || !tc)
        {
            tc = FindObjectOfType<TelekinesisController>();
        }

        if (tc != null && tc)
        {
            isBeingGrabbed = tc.GetGrabbedRigidbody() == cartRigidbody;
        }

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
        
        cartRigidbody.drag = 1f;
        cartRigidbody.angularDrag = 3f;
        
        cartRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }
    
    private void DetachFromPlayer()
    {
        isAttachedToPlayer = false;
        Debug.Log("Cart released - returning to normal physics!");
        
        cartRigidbody.drag = 3f;
        cartRigidbody.angularDrag = 5f;
        
        cartRigidbody.constraints = RigidbodyConstraints.None;
    }

    private Transform GetActiveCameraTransform()
    {
        if (Camera.main != null && Camera.main)
            return Camera.main.transform;

        if (FaRUtils.FPSController.FaRCharacterController.instance != null && FaRUtils.FPSController.FaRCharacterController.instance)
            return FaRUtils.FPSController.FaRCharacterController.instance.transform;

        var cam = FindObjectOfType<Camera>();
        if (cam != null && cam)
            return cam.transform;

        var player = FindObjectOfType<FaRUtils.FPSController.FaRCharacterController>();
        if (player != null && player)
            return player.transform;

        return null;
    }
    
    private void ApplyPlayerAttachmentForces()
    {
        Transform camTransform = GetActiveCameraTransform();
        if (camTransform == null) return;
        
        Vector3 camForward = camTransform.forward;
        camForward.y = 0;
        if (camForward.sqrMagnitude > 0.001f)
            camForward.Normalize();
        else
            camForward = Vector3.forward;

        Vector3 desiredCartPosition = camTransform.position + camForward * attachmentDistance;
        desiredCartPosition.y = transform.position.y;
        
        Vector3 positionError = desiredCartPosition - transform.position;
        positionError = Vector3.ClampMagnitude(positionError, 3f);

        Vector3 followForce = positionError * followStrength;
        Vector3 dampingForce = -cartRigidbody.velocity * followDamping;
        
        Vector3 totalForce = followForce + dampingForce;
        totalForce.y = 0;
        totalForce = Vector3.ClampMagnitude(totalForce, 2500f);
        
        cartRigidbody.AddForce(totalForce, ForceMode.Force);
        
        Quaternion desiredRotation = Quaternion.LookRotation(camForward, Vector3.up) * Quaternion.Euler(0, modelYRotationOffset, 0);
        Quaternion targetUprightRotation = Quaternion.Euler(0f, desiredRotation.eulerAngles.y, 0f);
        
        cartRigidbody.MoveRotation(Quaternion.Slerp(cartRigidbody.rotation, targetUprightRotation, Time.fixedDeltaTime * 15f));
        cartRigidbody.angularVelocity = Vector3.zero;
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
        telekineticObject.SetRotable(false);
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
        if (currentSpeed < 0.01f) return;
        
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