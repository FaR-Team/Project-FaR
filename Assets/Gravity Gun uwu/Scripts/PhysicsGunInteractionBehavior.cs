using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class PhysicsGunInteractionBehavior : MonoBehaviour
{
    [Header("LayerMask"), Tooltip("La capa de la cual el rayo puede agarrar objetos")]
    [SerializeField]
    private LayerMask                                       _grabLayer;
    [SerializeField]
    private Camera                                          _camera;


    public GameObject               Energia;

    public Rigidbody               _grabbedRigidbody;
    
    public bool                     isGrabbingObject;

    private Transform               _grabbedTransform; 

    private Vector3                 _hitOffsetLocal;

    private float                   _currentGrabDistance;

    private RigidbodyInterpolation  _initialInterpolationSetting;

    private Quaternion              _rotationDifference;

    [SerializeField]
    private Transform               _laserStartPoint        = null;

    private Vector3                 _rotationInput          = Vector3.zero;

    [Header("Opciones de Rotación")]
    [Tooltip("Es relativo al transform del jugador ah")]
    public Transform                PlayerTransform;
    [SerializeField]

    public  float                   SnapRotationDegrees     = 45f;
    [SerializeField]
    private float                   _snappedRotationSens    = 15f;
    [SerializeField]
    private float                   _rotationSpeed          = 5f;

    private Quaternion              _desiredRotation        = Quaternion.identity;

    //private bool                    _ContadorActivo         = false;

    private bool                    m_UserRotation;

    private bool                    _userRotation
    {
        get
        {
            return m_UserRotation;
        }
        set
        {
            if (m_UserRotation != value)
            {
                m_UserRotation = value;
                OnRotation.Invoke(value);
            }
        }
    }

    private bool                    m_RotacionSnap;

    private bool                    _RotacionSnap
    {
        get
        {
            return m_RotacionSnap;
        }
        set
        {
            if (m_RotacionSnap != value)
            {
                m_RotacionSnap = value;
                OnRotationSnapped.Invoke(value);
            }
        }
    }

    private bool                    m_RotationAxis;

    private bool                    _rotationAxis
    {
        get
        {
            return m_RotationAxis;
        }
        set
        {
            if (m_RotationAxis != value)
            {
                m_RotationAxis = value;
                OnAxisChanged.Invoke(value);
            }
        }
    }

    private Vector3                 _lockedRot;

    private Vector3                 _forward;
    private Vector3                 _up;
    private Vector3                 _right;

    

    private Vector3                 _scrollWheelInput       = Vector3.zero;

    [Header("Movimiento con la ruedita"), Space(5)]
    [SerializeField]
    private float                   _scrollWheelSensitivity = 5f;
    [SerializeField, Tooltip("La distancia mínima que puede estar el objeto relativo al jugador")] 
    private float                   _minObjectDistance      = 2.5f;
    [SerializeField, Tooltip("La distancia máxima donde se puede agarrar un objeto")]
    private float                   _maxGrabDistance        = 50f;
    private bool                    _distanceChanged;

   
    private Vector3                 _zeroVector3            = Vector3.zero;
    private Vector3                 _oneVector3             = Vector3.one;
    private Vector3                 _zeroVector2            = Vector2.zero;

    private bool                    _justReleased;
    private bool                    _wasKinematic;
    public bool                     _yaAnimo                = false;
   
    [Serializable]
    public class BoolEvent : UnityEvent<bool> { };
    [Serializable]
    public class GrabEvent : UnityEvent<GameObject> { };

    [Header("Eventos"), Space(10)]
    public BoolEvent                OnRotation;
    public BoolEvent                OnRotationSnapped;
    public BoolEvent                OnAxisChanged;

    public GrabEvent                OnObjectGrabbed;

    
    public Vector3                  CurrentForward              { get { return _forward; } }
    public Vector3                  CurrentUp                   { get { return _up; } }
    public Vector3                  CurrentRight                { get { return _right; } }
    public Transform                CurrentGrabbedTransform     { get { return _grabbedTransform; } }

 
    public Vector3                  StartPoint                  { get; private set; }
    public Vector3                  MidPoint                    { get; private set; }
    public Vector3                  EndPoint                    { get; private set; } 
    public float                    timer                        = 5;

    private void Start()
    {
        if(_camera == null)
        {
            Debug.LogError($"{nameof(PhysicsGunInteractionBehavior)} falta Camara", this);
            return;
        }

        if(PlayerTransform == null) 
        {
            PlayerTransform = this.transform;
            Debug.Log($"Como {nameof(PlayerTransform)} es nulo, se asignó a this.transform", this);
        }

        Energia.GetComponent<Animation>().Play("Salir uwuw");
    }

	private void Update ()
    {
        if (Energia.GetComponent<Energy>().EnergiaActual >= 1){
            if (!Input.GetMouseButton(0))
            {
                if (_grabbedRigidbody != null)
                {
                    ReleaseObject();
                }
                _justReleased = false;
                return;
            }
        }else if (Input.GetMouseButtonDown(0)){
            Ray ray = CenterRay();
            RaycastHit hit;
                       

#if UNITY_EDITOR
            Debug.DrawRay(ray.origin, ray.direction * _maxGrabDistance, Color.blue, 0.01f);
#endif
            if (Physics.Raycast(ray, out hit, _maxGrabDistance, _grabLayer))
            {
                if (Energia.GetComponent<Energy>()._ContadorActivo == false)
                {
                    Energia.GetComponent<Animation>().Play("Entrar uwuw");
                    StartCoroutine(Energia.GetComponent<Energy>().walter());
                    Energia.GetComponent<Energy>()._ContadorActivo = true;
                    Energia.GetComponent<Energy>().timer = 1;
                    Energia.GetComponent<Energy>()._yaAnimo = false;
                }
                else if (Energia.GetComponent<Energy>()._ContadorActivo == true)
                {
                    Energia.GetComponent<Energy>().timer = 2;
                    StartCoroutine(Energia.GetComponent<Energy>().Walicho());
                }
            }
            return;
        }else{
            return;
        }

        if (_grabbedRigidbody == null && !_justReleased)
        {

            Ray ray = CenterRay();
            RaycastHit hit;
                       

#if UNITY_EDITOR
            Debug.DrawRay(ray.origin, ray.direction * _maxGrabDistance, Color.blue, 0.01f);
#endif
            if (Physics.Raycast(ray, out hit, _maxGrabDistance, _grabLayer))
            {
               
                if (hit.rigidbody != null /*&& !hit.rigidbody.isKinematic*/)
                {
                   
                    _grabbedRigidbody                   = hit.rigidbody;
                    _wasKinematic                       = _grabbedRigidbody.isKinematic;
                    _grabbedRigidbody.isKinematic       = false;
                    _grabbedRigidbody.freezeRotation    = true;
                    _initialInterpolationSetting        = _grabbedRigidbody.interpolation;
                    _rotationDifference                 = Quaternion.Inverse(PlayerTransform.rotation) * _grabbedRigidbody.rotation;
                    _hitOffsetLocal                     = hit.transform.InverseTransformVector(hit.point - hit.transform.position);
                    _currentGrabDistance                = hit.distance; // Vector3.Distance(ray.origin, hit.point);
                    _grabbedTransform                   = _grabbedRigidbody.transform;
                    isGrabbingObject = true;
                  
                    _grabbedRigidbody.interpolation     = RigidbodyInterpolation.Interpolate;
                    
                    _grabbedRigidbody.gameObject.AddComponent<AvoidCollisionWPlayer>();

                    OnObjectGrabbed.Invoke(_grabbedRigidbody.gameObject);
                    if (Energia.GetComponent<Energy>()._ContadorActivo == false)
                    {
                        Energia.GetComponent<Animation>().Play("Entrar uwuw");
                        Energia.GetComponent<Energy>()._ContadorActivo = true;
                        Energia.GetComponent<Energy>().timer = 5;
                        Energia.GetComponent<Energy>()._yaAnimo = false;
                    }
                    else if (Energia.GetComponent<Energy>()._ContadorActivo == true)
                    {
                        Energia.GetComponent<Energy>().timer = 5;
                    }

#if UNITY_EDITOR
                    Debug.DrawRay(hit.point, hit.normal * 10f, Color.red, 10f);
#endif
                }
            }
        }
        else if(_grabbedRigidbody != null)
        {
            

            var direction = Input.GetAxis("Mouse ScrollWheel");

            if (Mathf.Abs(direction) > 0 && CheckObjectDistance(direction))
            {
                _distanceChanged    = true;
                _scrollWheelInput   = PlayerTransform.forward * _scrollWheelSensitivity * direction;
            } 
            else
            {
                _scrollWheelInput = _zeroVector3;
            }

        }
	}

    private void FixedUpdate()
    {
        if (_grabbedRigidbody)
        {

            Ray ray = CenterRay();

            UpdateRotationAxis();

#if UNITY_EDITOR
            Debug.DrawRay(_grabbedTransform.position, _up * 5f      , Color.green);
            Debug.DrawRay(_grabbedTransform.position, _right * 5f   , Color.red);
            Debug.DrawRay(_grabbedTransform.position, _forward * 5f , Color.blue);
#endif

            var intentionalRotation         = Quaternion.AngleAxis(_rotationInput.z, _forward) * Quaternion.AngleAxis(_rotationInput.y, _right) * Quaternion.AngleAxis(-_rotationInput.x, _up) * _desiredRotation;
            var relativeToPlayerRotation    = PlayerTransform.rotation * _rotationDifference;

            if (_userRotation && _RotacionSnap)
            {

                _lockedRot += _rotationInput;    


                if (Mathf.Abs(_lockedRot.x) > _snappedRotationSens || Mathf.Abs(_lockedRot.y) > _snappedRotationSens || Mathf.Abs(_lockedRot.z) > _snappedRotationSens)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        if (_lockedRot[i] > _snappedRotationSens)
                        {
                            _lockedRot[i] += SnapRotationDegrees;
                        }
                        else if (_lockedRot[i] < -_snappedRotationSens)
                        {
                            _lockedRot[i] += -SnapRotationDegrees;
                        }
                        else
                        {
                            _lockedRot[i] = 0;
                        }
                    }

                    var q = Quaternion.AngleAxis(-_lockedRot.x, _up) * Quaternion.AngleAxis(_lockedRot.y, _right) * Quaternion.AngleAxis(_lockedRot.z, _forward) * _desiredRotation;

                    var newRot = q.eulerAngles;

                    newRot.x = Mathf.Round(newRot.x / SnapRotationDegrees) * SnapRotationDegrees;
                    newRot.y = Mathf.Round(newRot.y / SnapRotationDegrees) * SnapRotationDegrees;
                    newRot.z = Mathf.Round(newRot.z / SnapRotationDegrees) * SnapRotationDegrees;

                    _desiredRotation = Quaternion.Euler(newRot);

                    _lockedRot = _zeroVector2;
                }
            }
            else
            {  
                _desiredRotation = _userRotation ? intentionalRotation : relativeToPlayerRotation;
            }
            
            _grabbedRigidbody.angularVelocity   = _zeroVector3;
            _rotationInput                      = _zeroVector2;
            _rotationDifference                 = Quaternion.Inverse(PlayerTransform.rotation) * _desiredRotation;

            
            var holdPoint           = ray.GetPoint(_currentGrabDistance) + _scrollWheelInput;
            var centerDestination = holdPoint - _grabbedTransform.TransformVector(_hitOffsetLocal);

#if UNITY_EDITOR
            Debug.DrawLine(ray.origin, holdPoint, Color.blue, Time.fixedDeltaTime);
#endif
           
            var toDestination = centerDestination - _grabbedTransform.position;


            var force = toDestination / Time.fixedDeltaTime * 0.3f / _grabbedRigidbody.mass;


            _grabbedRigidbody.velocity = _zeroVector3;
            _grabbedRigidbody.AddForce(force, ForceMode.VelocityChange);

 

            
            if (_distanceChanged)
            {
                _distanceChanged = false;
                _currentGrabDistance = Vector3.Distance(ray.origin, holdPoint);
            }

  
            StartPoint  = _laserStartPoint.transform.position;
            MidPoint    = holdPoint;
            EndPoint    = _grabbedTransform.TransformPoint(_hitOffsetLocal);
            
        }
    }

    private void RotateGrabbedObject()
    {
        if (_grabbedRigidbody == null)
            return;

        _grabbedRigidbody.MoveRotation(Quaternion.Lerp(_grabbedRigidbody.rotation, _desiredRotation, Time.fixedDeltaTime * _rotationSpeed));
    }


    private void UpdateRotationAxis()
    {
        if (!_RotacionSnap)
        {
            _forward    = PlayerTransform.forward;
            _right      = PlayerTransform.right;
            _up         = PlayerTransform.up;

            return;
        }

        if (_rotationAxis)
        {
            _forward    = _grabbedTransform.forward;
            _right      = _grabbedTransform.right;
            _up         = _grabbedTransform.up;

            return;
        }

        NearestTranformDirection(_grabbedTransform, PlayerTransform, ref _up, ref _forward, ref _right);
    }

    private void NearestTranformDirection(Transform transformToCheck, Transform referenceTransform, ref Vector3 up, ref Vector3 forward, ref Vector3 right)
    {
        var directions = new List<Vector3>()
        {
            transformToCheck.forward,
            -transformToCheck.forward,
            transformToCheck.up,
            -transformToCheck.up,
            transformToCheck.right,
            -transformToCheck.right,
        };


        up = GetDirectionVector(directions, referenceTransform.up);
       
        directions.Remove(up);
        directions.Remove(-up);
            
        forward = GetDirectionVector(directions, referenceTransform.forward);
        
        directions.Remove(forward);
        directions.Remove(-forward);

        right = GetDirectionVector(directions, referenceTransform.right);

    }

    private Vector3 GetDirectionVector(List<Vector3> directions, Vector3 direction)
    {
        var maxDot  = -Mathf.Infinity;
        var ret     = Vector3.zero;

        for (var i = 0; i < directions.Count; i++)
        {
            var dot = Vector3.Dot(direction, directions[i]);

            if (dot > maxDot)
            {
                ret     = directions[i];
                maxDot  = dot;
            }
        }

        return ret;
    }     


    private Ray CenterRay()
    {
        return _camera.ViewportPointToRay(_oneVector3 * 0.5f);
    }


    private bool CheckObjectDistance(float direction)
    {
        var pointA      = PlayerTransform.position;
        var pointB      = _grabbedRigidbody.position;

        var distance    = Vector3.Distance(pointA, pointB);

        if (direction > 0)
            return distance <= _maxGrabDistance;

        if (direction < 0)
            return distance >= _minObjectDistance;

        return false;
    }

    private void ReleaseObject()
    {
        Energia.GetComponent<Energy>().UseEnergy(1);

        AvoidCollisionWPlayer colis = _grabbedRigidbody.GetComponent<AvoidCollisionWPlayer>();
        colis.player = null;
        Destroy(colis);

        _grabbedRigidbody.MoveRotation(_desiredRotation);
        
        _grabbedRigidbody.isKinematic               = _wasKinematic;
        _grabbedRigidbody.interpolation             = _initialInterpolationSetting;
        _grabbedRigidbody.freezeRotation            = false;
        _grabbedRigidbody                           = null;
        _scrollWheelInput                           = _zeroVector3;
        _grabbedTransform                           = null;
        StartCoroutine(WaitToSetIsGrabbing(0.1f, false));
        _userRotation                               = false;
        _RotacionSnap                               = false;
        StartPoint                                  = _zeroVector3;
        MidPoint                                    = _zeroVector3;
        EndPoint                                    = _zeroVector3;
        _justReleased                               = true;

        OnObjectGrabbed.Invoke(null);
    }

    IEnumerator WaitToSetIsGrabbing(float time, bool isGrabbing)
    {
        yield return new WaitForSeconds(time);
        isGrabbingObject = isGrabbing;
    }
}